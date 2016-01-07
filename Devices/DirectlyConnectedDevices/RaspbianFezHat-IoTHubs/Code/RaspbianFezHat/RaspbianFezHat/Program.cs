namespace RaspbianFezHat
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using GHIElectronics.Mono.Shields;

    public class Program
    {
        private static SimpleLogger logger = new SimpleLogger();
        private static FEZHAT hat;
        private static ConnectTheDotsHelper ctdHelper;
        private static System.Timers.Timer telemetryTimer;

        public static void Main(string[] args)
        {
            // Hard coding guid for sensors. Not an issue for this particular application which is meant for testing and demos
            List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor> {
                new ConnectTheDotsSensor("2298a348-e2f9-4438-ab23-82a3930662ab", "Light", "L"),
                new ConnectTheDotsSensor("d93ffbab-7dff-440d-a9f0-5aa091630201", "Temperature", "C"),
            };

            Program.ctdHelper = new ConnectTheDotsHelper(
                logger: Program.logger,
                iotHubHost: "IOT_HOST",
                deviceName: "DEVICE_ID",
                deviceKey: "DEVICE_KEY",
                organization: "YOUR_ORGANIZATION_OR_SELF",
                location: "YOUR_LOCATION",
                eventHubMessageSubject: "Raspberry PI",
                sensorList: sensors);

            if (ctdHelper.IsConnectionReady)
            {
                // Setup the FEZ HAT driver
                Program.SetupHatAsync().Wait();

                // Initialize Receiver
                using (CancellationTokenSource cancelSource = new CancellationTokenSource())
                {
                    var receiver = ReceiveCommandsAsync(cancelSource.Token);

                    Console.WriteLine("Reading data from FEZ HAT sensors. Press Enter to Stop");
                    Console.ReadLine();
                    Console.WriteLine("Closing...");

                    // Stops receiver
                    Program.logger.Info("Stoping receiver...");
                    cancelSource.Cancel();

                    Program.telemetryTimer.Enabled = false;
                    Program.telemetryTimer.Dispose();
                    Program.hat.Dispose();

                    // Closes the connections with Azure (both send and receive links)
                    Program.ctdHelper.Dispose();

                    // Waits until the receiver stops
                    receiver.Wait();
                }
            }
            else
            {
                Console.WriteLine("An error ocurred while connecting to Azure. See previous errors for details.");
                Console.WriteLine("Press Enter to exit");
                Program.ctdHelper.Dispose();
            }
        }

        private static async Task SetupHatAsync()
        {
            // Initialize Fez Hat
            logger.Info("Initializing FEZ HAT");
            hat = await FEZHAT.CreateAsync();

            // Initialize Telemetry Timer
            telemetryTimer = new System.Timers.Timer(2000); 
            telemetryTimer.Elapsed += TelemetryTimer_Elapsed;
            telemetryTimer.Enabled = true;

            Program.logger.Info("FEZ HAT Initialized");
        }

        private static async Task ReceiveCommandsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var command = await Program.ctdHelper.ReceiveCommandsAsync();
                if (command != string.Empty)
                {
                    if (command != string.Empty)
                    {
                        Program.logger.Info("Command Received: {0}", command);
                        switch (command.ToUpperInvariant())
                        {
                            case "RED":
                                hat.D2.Color = new FEZHAT.Color(255, 0, 0);
                                break;
                            case "GREEN":
                                hat.D2.Color = new FEZHAT.Color(0, 255, 0);
                                break;
                            case "BLUE":
                                hat.D2.Color = new FEZHAT.Color(0, 0, 255);
                                break;
                            case "OFF":
                                hat.D2.TurnOff();
                                break;
                            default:
                                Program.logger.Info("Unrecognized command: {0}", command);
                                break;
                        }
                    }
                }
            }
        }

        private static void TelemetryTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            // Light Sensor
            ConnectTheDotsSensor lightSensor = ctdHelper.Sensors.Find(item => item.MeasureName == "Light");
            lightSensor.Value = hat.GetLightLevel();
            Program.ctdHelper.SendSensorData(lightSensor);

            // Temperature Sensor
            var tempSensor = ctdHelper.Sensors.Find(item => item.MeasureName == "Temperature");
            tempSensor.Value = hat.GetTemperature();
            Program.ctdHelper.SendSensorData(tempSensor);

            Program.logger.Info("Temperature: {0} °C, Light {1}", tempSensor.Value.ToString("N2", CultureInfo.InvariantCulture), lightSensor.Value.ToString("P2", CultureInfo.InvariantCulture));
        }
    }
}
