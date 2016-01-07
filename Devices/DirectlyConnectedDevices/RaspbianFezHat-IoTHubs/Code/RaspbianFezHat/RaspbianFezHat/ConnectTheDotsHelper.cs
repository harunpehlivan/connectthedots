namespace RaspbianFezHat
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Amqp;
    using Amqp.Framing;
    using Amqp.Types;
    using Newtonsoft.Json;

    public class ConnectTheDotsHelper : IDisposable
    {
        private const int PORT = 5671;

        private static readonly long UtcReference = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;

        private SimpleLogger logger;

        private string deviceId;
        private string deviceIP;
        private string eventHubMessageSubject;

        // We have several threads that will use the same SenderLink object
        // we will protect the access using InterLock.Exchange 0 for false, 1 for true. 
        private int sendingMessage = 0;

        // Variables for AMQPs connection
        private Connection connection = null;
        private Session session = null;
        private SenderLink sender = null;
        private ReceiverLink receiveLink = null;

        private Address appAMQPAddress;
        private string appEHTarget;
        private string deviceName;
        private string deviceKey;
        private string iotHubHost;

        private bool disposedValue = false; // To detect redundant calls (disposable pattern)

        public ConnectTheDotsHelper(
            SimpleLogger logger,
            string iotHubHost = "",
            string deviceName = "",
            string deviceKey = "",
            string organization = "",
            string location = "",
            string eventHubMessageSubject = "",
            List<ConnectTheDotsSensor> sensorList = null)
        {
            this.IsConnectionReady = false;
            this.logger = logger;
            this.Organization = organization;
            this.Location = location;
            this.Sensors = sensorList;
            this.eventHubMessageSubject = eventHubMessageSubject;

            this.iotHubHost = iotHubHost;
            this.deviceName = deviceName;
            this.deviceKey = deviceKey;

            // Get device IP
            IPHostEntry hostInfoIP = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress address = hostInfoIP.AddressList[0];
            this.deviceIP = address.ToString();
            this.DisplayName = hostInfoIP.HostName;

            var httpUtility = new HttpUtility();

            this.appAMQPAddress = new Address(this.iotHubHost, PORT, null, null); 
            this.appEHTarget = Fx.Format("/devices/{0}/messages/events", deviceName);

            this.ApplySettingsToSensors();

            this.InitAMQPConnection(false);
        }

        public ConnectTheDotsHelper(
            string iotHubHost = "",
            string deviceName = "",
            string deviceKey = "",
            string organization = "",
            string location = "",
            List<ConnectTheDotsSensor> sensorList = null) : this(
                new SimpleLogger(),
                iotHubHost,
                deviceName,
                deviceName,
                deviceKey,
                organization,
                location)
        {
        }

        // For Disposable pattern
        ~ConnectTheDotsHelper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(false);
        }

        public bool IsConnectionReady { get; private set; }

        // App Settings variables
        public string DisplayName { get; set; }

        public string Organization { get; set; }

        public string Location { get; set; }

        public List<ConnectTheDotsSensor> Sensors { get; set; }

        public void SendSensorData(ConnectTheDotsSensor sensor)
        {
            sensor.TimeCreated = DateTime.UtcNow.ToString("o");
            this.SendAmqpMessage(sensor.ToJson());
        }

        /// <summary>
        ///  Apply settings to sensors collection
        /// </summary>
        public void ApplySettingsToSensors()
        {
            foreach (ConnectTheDotsSensor sensor in this.Sensors)
            {
                sensor.DisplayName = this.DisplayName;
                sensor.Location = this.Location;
                sensor.Organization = this.Organization;
            }
        }

        public async Task<string> ReceiveCommandsAsync()
        {
            Message receivedMessage = await this.receiveLink.ReceiveAsync();
            if (receivedMessage != null)
            {
                this.receiveLink.Accept(receivedMessage);
                return Encoding.UTF8.GetString((byte[])receivedMessage.Body);
            }
            else
            {
                return string.Empty;
            }
        }

        #region IDisposable Support
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (this.sender != null) this.sender.Close(5000);
                if (this.receiveLink != null) this.receiveLink.Close(5000);
                if (this.session != null) this.session.Close();
                if (this.connection != null) this.connection.Close();
                
                this.disposedValue = true;
            }
        }
        #endregion

        private static bool PutCbsToken(Connection connection, string host, string shareAccessSignature, string audience)
        {
            bool result = true;
            Session session = new Session(connection);

            string cbsReplyToAddress = "cbs-reply-to";
            var cbsSender = new SenderLink(session, "cbs-sender", "$cbs");
            var cbsReceiver = new ReceiverLink(session, cbsReplyToAddress, "$cbs");

            // construct the put-token message
            var request = new Message(shareAccessSignature);
            request.Properties = new Properties();
            request.Properties.MessageId = Guid.NewGuid().ToString();
            request.Properties.ReplyTo = cbsReplyToAddress;
            request.ApplicationProperties = new ApplicationProperties();
            request.ApplicationProperties["operation"] = "put-token";
            request.ApplicationProperties["type"] = "azure-devices.net:sastoken";
            request.ApplicationProperties["name"] = audience;
            cbsSender.Send(request);

            // receive the response
            var response = cbsReceiver.Receive();
            if (response == null || response.Properties == null || response.ApplicationProperties == null)
            {
                result = false;
            }
            else
            {
                int statusCode = (int)response.ApplicationProperties["status-code"];
                string statusCodeDescription = (string)response.ApplicationProperties["status-description"];

                // !Accepted && !OK
                if (statusCode != (int)202 && statusCode != (int)200) 
                {
                    result = false;
                }
            }

            // the sender/receiver may be kept open for refreshing tokens
            cbsSender.Close();
            cbsReceiver.Close();
            session.Close();

            return result;
        }

        private static string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            /* http://msdn.microsoft.com/en-us/library/azure/dn170477.aspx
            ** the canonical Uri scheme is http because the token is not amqp specific
            ** signature is computed from joined encoded request Uri string and expiry string
            */

            string expiry = ((long)(DateTime.UtcNow - new DateTime(UtcReference, DateTimeKind.Utc) + tokenTimeToLive).TotalSeconds).ToString();
            string encodedUri = HttpUtility.UrlEncode(resource);
            using (var sha1 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(sharedAccessKey)))
            {
                var hmac = sha1.ComputeHash(Encoding.UTF8.GetBytes(encodedUri + "\n" + expiry));
                string sig = Convert.ToBase64String(hmac);

                if (keyName != null)
                {
                    return Fx.Format(
                    "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                    encodedUri,
                    HttpUtility.UrlEncode(sig),
                    HttpUtility.UrlEncode(expiry),
                    HttpUtility.UrlEncode(keyName));
                }
                else
                {
                    return Fx.Format(
                        "SharedAccessSignature sr={0}&sig={1}&se={2}",
                        encodedUri,
                        HttpUtility.UrlEncode(sig),
                        HttpUtility.UrlEncode(expiry));
                }
            }
        }

        /// <summary>
        /// Callback function used to report on AMQP message send 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="outcome"></param>
        /// <param name="state"></param>
        private void SendOutcome(Message message, Outcome outcome, object state)
        {
            if (outcome is Accepted)
            {
                ////#if DEBUG
                this.logger.Info("Sent message at {0}", message.ApplicationProperties["time"]);
                ////#endif
#if LOG_MESSAGE_RATE
                g_messageCount++;
#endif
            }
            else
            {
                this.logger.Error("Error sending message {0} - {1}, outcome {2}", message.ApplicationProperties["time"], message.Properties.Subject, outcome);
                this.logger.Error("Error sending to {0} at {1}", this.appEHTarget, this.appAMQPAddress);
            }
        }

        /// <summary>
        /// Initialize AMQP connection
        /// we are using the connection to send data to Azure IoT Hubs
        /// Connection information is retreived from the app configuration file
        /// </summary>
        /// <returns>
        /// true when successful
        /// false when unsuccessful
        /// </returns>
        private bool InitAMQPConnection(bool reset)
        {
            this.IsConnectionReady = false;

            if (reset)
            {
                // If the reset flag is set, we need to kill previous connection 
                try
                {
                    this.logger.Info("Resetting connection to Azure IoT Hub");
                    this.logger.Info("Closing any existing senderLink, session and connection.");
                    if (this.receiveLink != null) this.receiveLink.Close();
                    if (this.sender != null) this.sender.Close();
                    if (this.session != null) this.session.Close();
                    if (this.connection != null) this.connection.Close();
                }
                catch (Exception e)
                {
                    this.logger.Error("Error closing AMQP connection to Azure IoT Hub: {0}", e.Message);
                }
            }

            this.logger.Info("Initializing connection to Azure IoT Hub");

            // Initialize AMQPS connection
            try
            {
                this.connection = new Connection(this.appAMQPAddress);

                string audience = Fx.Format("{0}/devices/{1}", this.iotHubHost, this.deviceName);
                string resourceUri = Fx.Format("{0}/devices/{1}", this.iotHubHost, this.deviceName);

                this.logger.Info("Obtaining SAS Token");

                string sasToken = GetSharedAccessSignature(string.Empty, this.deviceKey, resourceUri, new TimeSpan(1, 0, 0));
                bool cbs = PutCbsToken(this.connection, this.iotHubHost, sasToken, audience);

                if (cbs)
                {
                    this.logger.Info("SAS Token successfully validated");
                    this.session = new Session(this.connection);
                    this.sender = new SenderLink(this.session, "send-link", this.appEHTarget);

                    string entity = Fx.Format("/devices/{0}/messages/deviceBound", this.deviceName);
                    this.receiveLink = new ReceiverLink(this.session, "receive-link", entity);
                }
                else
                {
                    this.logger.Error("SAS Token couldn't be obtained");
                    return false;
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Error connecting to Azure IoT Hub: {0}", e.Message);
                if (this.receiveLink != null) this.receiveLink.Close();
                if (this.sender != null) this.sender.Close();
                if (this.session != null) this.session.Close();
                if (this.connection != null) this.connection.Close();
                return false;
            }

            this.IsConnectionReady = true;
            this.logger.Info("Connection to Azure IoT Hub initialized.");
            return true;
        }

        /// <summary>
        /// Send a string as an AMQP message to Azure IoT Hub
        /// </summary>
        /// <param name="valuesJson">
        /// String to be sent as an AMQP message to IoT Hub
        /// </param>
        private void SendAmqpMessage(string valuesJson)
        {
            Message message = new Message();

            // If there is no value passed as parameter, do nothing
            if (valuesJson == null) return;

            try
            {
                // Deserialize Json message
                var sample = JsonConvert.DeserializeObject<Dictionary<string, object>>(valuesJson);
                if (sample == null)
                {
                    this.logger.Info("Error parsing JSON message {0}", valuesJson);
                    return;
                }
#if DEBUG
                this.logger.Info("Parsed data from serial port: {0}", valuesJson);
                this.logger.Info("Device GUID: {0}", sample["guid"]);
                this.logger.Info("Subject: {0}", this.eventHubMessageSubject);
                this.logger.Info("dspl: {0}", sample["displayname"]);
#endif

                // Convert JSON data in 'sample' into body of AMQP message
                // Only data added by gateway is time of message (since sensor may not have clock) 
                this.deviceId = Convert.ToString(sample["guid"]);      // Unique identifier from sensor, to group items in iot hub

                message.Properties = new Properties()
                {
                    Subject = this.eventHubMessageSubject,              // Message type (e.g. "wthr") defined in sensor code, sent in JSON payload
                    CreationTime = DateTime.UtcNow, // Time of data sampling
                };

                message.MessageAnnotations = new MessageAnnotations();

                // Event Hub partition key: device id - ensures that all messages from this device go to the same partition and thus preserve order/co-location at processing time
                message.MessageAnnotations[new Symbol("x-opt-partition-key")] = this.deviceId;
                message.ApplicationProperties = new ApplicationProperties();
                message.ApplicationProperties["time"] = message.Properties.CreationTime;
                message.ApplicationProperties["from"] = this.deviceId; // Originating device
                message.ApplicationProperties["dspl"] = sample["displayname"] + " (" + this.deviceIP + ")";      // Display name for originating device defined in sensor code, sent in JSON payload

                if (sample != null && sample.Count > 0)
                {
#if! SENDAPPPROPERTIES

                    var outDictionary = new Dictionary<string, object>(sample);
                    outDictionary["Subject"] = message.Properties.Subject; // Message Type
                    outDictionary["time"] = message.Properties.CreationTime;
                    outDictionary["from"] = this.deviceId; // Originating device
                    outDictionary["dspl"] = sample["displayname"] + " (" + this.deviceIP + ")";      // Display name for originating device
                    message.Properties.ContentType = "text/json";
                    message.BodySection = new Data() { Binary = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(outDictionary)) };
#else
                    foreach (var sampleProperty in sample)
				    {
					    message.ApplicationProperties [sample.Key] = sample.Value;
				    }
#endif
                }
                else
                {
                    // No data: send an empty message with message type "weather error" to help diagnose problems "from the cloud"
                    message.Properties.Subject = "wthrerr";
                }
            }
            catch (Exception e)
            {
                this.logger.Error("Error when deserializing JSON data received over serial port: {0}", e.Message);
                return;
            }

            // Send to the cloud asynchronously
            // Obtain handle on AMQP sender-link object
            if (0 == Interlocked.Exchange(ref this.sendingMessage, 1))
            {
                bool amqpConnectionIssue = false;
                try
                {
                    // Message send function is asynchronous, we will receive completion info in the SendOutcome function
                    this.sender.Send(message, this.SendOutcome, null);
                }
                catch (Exception e)
                {
                    // Something went wrong let's try and reset the AMQP connection
                    this.logger.Error("Exception while sending AMQP message: {0}", e.Message);
                    amqpConnectionIssue = true;
                }

                Interlocked.Exchange(ref this.sendingMessage, 0);

                // If there was an issue with the AMQP connection, try to reset it
                while (amqpConnectionIssue)
                {
                    amqpConnectionIssue = !this.InitAMQPConnection(true);
                    Thread.Sleep(200);
                }
            }

#if LOG_MESSAGE_RATE
            if (g_messageCount >= 500)
            {
                float secondsElapsed = ((float)stopWatch.ElapsedMilliseconds) / (float)1000.0;
                if (secondsElapsed > 0)
                {
                    Console.WriteLine("Message rate: {0} msg/s", g_messageCount / secondsElapsed);
                    g_messageCount = 0;
                    stopWatch.Restart();
                }
            }
#endif
        }
    }
}
