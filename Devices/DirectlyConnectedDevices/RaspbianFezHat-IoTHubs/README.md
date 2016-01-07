Raspberry Pi 2 with FEZ Hat on Raspbian Hands-on Lab
========================================

ConnectTheDots will help you get tiny devices connected to Microsoft Azure, and to implement great IoT solutions taking advantage of Microsoft Azure advanced analytic services such as Azure Stream Analytics and Azure Machine Learning.

> This lab is stand-alone, but is used at Microsoft to accompany a presentation about Azure, Windows 10 IoT Core, Raspbian, and our IoT services. If you wish to follow this on your own, you are encouraged to do so. If not, consider attending a Microsoft-led IoT lab in your area.


In this lab you will use a [Raspberry Pi 2](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/) device running [Raspbian](https://www.raspberrypi.org/downloads/raspbian/) and a [FEZ HAT](https://www.ghielectronics.com/catalog/product/500) sensor hat. Using an Application, the sensors get the raw data and format it into a JSON string. That string is then shuttled off to the [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/), where it gathers the data and is then displayed in a chart using Power BI.

This lab includes the following tasks:

1. [Setup Environment](#Task1)
	1. [Software](#Task11)
	2. [Devices](#Task12)
	3. [Azure Account](#Task13)
	4. [Device Registration](#Task14)
2. [Creating an Application](#Task2)
	1. [Deploying a Simple Application](#Task21)
	2. [Reading the FEZ HAT sensors](#Task22)
	3. [Connecting the Application to Azure](#Task23)
3. [Consuming the IoT Hub data](#Task3)
	2. [Using Power BI](#Task32)
	3. [Consuming the IoT Hub data from a Website](#Task33)
4. [Sending commands to your devices](#Task4)
5. [Summary](#Summary)

<a name="Task1" />
##Setup##
The following sections are intended to setup your environment to be able to create and run your solutions in Raspbian.

<a name="Task11" />
###Setting up your Software###
To setup your Raspbian development PC, you first need to install the following:

- Visual Studio 2015 or above – [Community Edition](http://www.visualstudio.com/downloads/download-visual-studio-vs) is sufficient.

- SSH Client, such as [Putty](http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html).

- (optional) FTP Client of your choice. You can use Windows FTP Client.

<a name="Task12" />
###Setting up your Devices###

For this project, you will need the following:

- [Raspberry Pi 2 Model B](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)
- [GHI FEZ HAT](https://www.ghielectronics.com/catalog/product/500)

To setup your devices perform the following steps:

1. Plug the **GHI FEZ HAT** into the **Raspberry Pi 2**. 

	![fezhat-connected-to-raspberri-pi-2](Images/fezhat-connected-to-raspberri-pi-2.png?raw=true)
	
	_The FEZ hat connected to the Raspberry Pi 2 device_

2. Install the Raspbian Operating System. You can follow the [installation guide](https://www.raspberrypi.org/documentation/installation/installing-images/README.md) in the [raspberrypi.org](https://www.raspberrypi.org/) site.
	1. Download the lastest version of **Raspbian** from [here](https://www.raspberrypi.org/downloads/raspbian/).
	
		> **Note:** The following steps uses the official supported version of Raspbian Based on Debian Jessie (Kernel version: 4.1). To use a different version of the operating system you might need to follow additional steps.
	
	2. Download **Win32DiskImager** from the [Sourceforge Project page](http://sourceforge.net/projects/win32diskimager/).
	3. Copy the **Raspbian** image into the SD Card using **Win32DiskImager**. You can follow the instructions [here](https://www.raspberrypi.org/documentation/installation/installing-images/windows.md) 
	
		> **Note:** If you have a display, keyboard and mouse you are ready to go and you can skip the pre-installation steps and connect your PC to the device using SSH.
		> 
		> Eject the SD Card from your PC and insert it in the Raspberry Pi. Then Connect the Raspberry Pi to a power supply, keyboard, monitor, and Ethernet cable (or Wi-Fi dongle) with an Internet connection and find the IP address hovering the mouse on the upper right corner of the desktop. Then jump to the step #6 (you may also want to take a look at step #4 where remote access is explained).
		>
		> Otherwise follow the next instructions to set a fixed IP so it can be accessed with a remote console.
	
3. Setup a fixed IP address for your device and connect it directly to your development PC.
	1. Find the drive where the SD Card is mounted in your desktop PC.
	2. Edit the **cmdline.txt** file (make a backup copy first) and add the _fixed IP address_ at the end of the line (be sure not to add extra lines).
	
		> **Note:** For network settings where the IP address is obtained automatically, use an address in the range 169.254.X.X (169.254.0.0 – 169.254.255.255). For network settings where the IP address is fixed, use an address which matches the laptop/computer's address except the last octet.

		`ip=169.254.0.2`
		
	3. Connect the Raspberry Pi to a power supply and use the Ethernet cable to connect your device and your development PC. You can do it by plugging in one end of the spare Ethernet cable to the extra Ethernet port on your PC, and the other end of the cable to the Ethernet port on your Raspberry Pi device.	
	
5. To connect to your Raspberry Pi from your Desktop computer you will need a SSH (Secure SHell) client. PuTTY will be used for this section since it's for Windows plattform and it's free and open source. If you are using Mac OSX or another operating system, choose an appropriate SSH client, or use what is installed by default.

	1. Download PuTTY from [here](http://www.chiark.greenend.org.uk/~sgtatham/putty/download.html "Download PuTTY"). Downloading the **putty.exe** file is enough.

	1. PuTTY doesn't need to be installed. Just move the **putty.exe** file to some place in your file system.
	
	1. Open PuTTY and enter your device's IP in the **Host Name** field and select **SSH** as the **Connection Type**. Click on **Open**. Leave the other fields with their default values, including the **Port**
	
		![Open PuTTY](Images/open-putty.png?raw=true)

	1. Log in using the user you configured during the setup (usually **pi**):
	
		![Raspberry Pi Login](Images/raspberry-pi-login.png?raw=true)
	
		Find more information about using PuTTY in the [PuTTY Documentation Page](http://www.chiark.greenend.org.uk/~sgtatham/putty/docs.html "PuTTY Documentation Page").
	
5. Now, to connect your device to Internet you can either set up a wireless connection or change the device name so it can be easily discoverable in your wired network:
	
	- If you have a WiFi dongle you can set up your Internet connection as a wireless network. Connect your WiFi Dongle to the same network than your development PC. Follow instructions [here](https://www.raspberrypi.org/documentation/configuration/wireless/wireless-cli.md).
		1. Open a SSH console and execute the following command:
			
			````
			$ sudo iwlist wlan0 scan
			````
			
			which will list all available WiFi networks along with other useful information.
			
			![Scan available WiFi networks](Images/scan-available-wifi-networks.png?raw=true)
			
			Look out for:
			- ESSID:"XXX". This is the name of the WiFi network. 
			- IE: IEEE 802.11i/WPA2 Version 1. This is the authentication used; in this case it is WPA2, the newer and more secure wireless standard which replaces WPA1. **This guide should work for WPA or WPA2, but may not work for WPA2 enterprise**; for WEP hex keys see the last example here <http://netbsd.gw.com/cgi-bin/man-cgi?wpa_supplicant.conf+5+NetBSD-current>.
			
			You will also need the password for the WiFi network. For most home routers this is located on a sticker on the back of the router. For Microsoft-run labs, the WiFi information for the room will be provided to you.
			
		2. To add the network details to your Raspberry Pi open the **wpa-supplicant** configuration file. Run the following command to edit the file:
		
			````
			$ sudo nano /etc/wpa_supplicant/wpa_supplicant.conf 
			````
		
			Go to the bottom of the file and add the following:
			
			````
			network={ 
				ssid="The_ESSID_from_earlier" 
				psk="Your_wifi_password" 
			}
			````

			![Editting wpa-supplicant](Images/editting-wpa-supplicant.png?raw=true)
			
		3. Save the file by pressing **ctrl+x** then **y**, then finally press **Enter**. 
		
			At this point, wpa-supplicant will normally notice a change has occurred within a few seconds, and it will try and connect to the network. If it does not, either manually restart the interface with `sudo ifdown wlan0` and `sudo ifup wlan0`, or reboot your Raspberry Pi with `sudo reboot`. 
			
		4. Verify if it has successfully connected using `ifconfig wlan0`. If the **inet addr** field has an address beside it, the Raspberry Pi has connected to the network. If not, check your password and ESSID are correct
		
			![Verify wireless connection](Images/verify-wireless-connection.png?raw=true)
		
	- If you don't have a WiFi dongle, you can rename your device so it can be easily discoverable through your wired network.
		1. With the device connected to your desktop computer through the Ethernet cable, run the Raspberry Pi Software Configuration tool. Open a remote SSH console and type:
		
			````
			$ sudo raspi-config
			````		

		2. Select **Advanced Options** and hit Enter
		
			![Raspi-Config Menu](Images/raspi-config-menu.png?raw=true)
			
		3. Select **Host Name** and hit Enter
		
			![Raspi-Config Advanced Options](Images/raspi-config-advanced-options.png?raw=true)
			
		4. Dismiss the hostname label warning message by hitting Enter and Edit your device name:
		
			![Raspi-Config Edit the Hostname](Images/raspi-config-edit-the-hostname.png?raw=true)
		
		5. Once you're done, hit Enter to confirm the change. The tool will take you to the Raspi-Config home screen. Hit **Finish** to save the changes (Use the Tab key to move to the buttons section).
		
		6. Select **Yes** in the following screen to reboot the device
		
			![Raspi-Config Reboot message](Images/raspi-config-reboot-message.png?raw=true)
			
		7. After rebooting disconnect your ethernet cable from your desktop PC and plug it into your wired network.
		
		8. Open a new SSH Connection using the name you just configured as hostname to check that everything is working as expected:
		
			![Connect to MyRaspberryPi](Images/connect-to-myraspberrypi.png?raw=true)


9. Install the **Mono Framework** as stated [here](http://www.raspberry-sharp.org/eric-bezine/2012/10/mono-framework/installing-mono-raspberry-pi/). To do this, run the following commands.

	````
	$ sudo apt-get update
	$ sudo apt-get install mono-complete
	````
10. Lastly, you will install and configure an FTP server to copy files from the development computer. To do this, perform the following steps.
	1. Install the FTP server by running the following command.
	
		````
		$ sudo apt-get install vsftpd
		````

	2. Configure the server by editing the config file. To do it, run the following command:
	
		````	
		$ sudo nano /etc/vsftpd.conf
		````
		
		Ensure that the following settings are set to **YES**.
		
		````	
		local_enable=YES
		write_enable=YES
		````
		
	
	3. Restart the server by running the following:

		````	
		$ sudo /etc/init.d/vsftpd restart
		````	

<a name="Task13" />
### Setting up your Azure Account
You will need a Microsoft Azure subscription ([free trial subscription] (http://azure.microsoft.com/en-us/pricing/free-trial/) is sufficient)

#### Creating an IoT Hub

1. Enter the Azure portal, by browsing to http://portal.azure.com
2. Create a new IoT Hub. To do this, click **New** in the jumpbar, then click **Internet of Things**, then click **Azure IoT Hub**.

	![Creating a new IoT Hub](Images/creating-a-new-iot-hub.png?raw=true "Createing a new IoT Hub")

	_Creating a new IoT Hub_

3. Configure the **IoT hub** with the desired information:
 - Enter a **Name** for the hub e.g. _iot-sample_,
 - Select a **Pricing and scale tier** (_F1 Free_ tier is enough),
 - Create a new resource group, or select and existing one. For more information, see [Using resource groups to manage your Azure resources](https://azure.microsoft.com/en-us/documentation/articles/resource-group-portal/).
 - Select the **Region** such as _East US_ where the service will be located.

	![new iot hub settings](Images/new-iot-hub-settings.png?raw=true "New IoT Hub settings")

	_New IoT Hub Settings_

4. It can take a few minutes for the IoT hub to be created. Once it is ready, open the blade of the new IoT hub, take note of the URI and select the key icon at the top to access to the shared access policy settings:

	![IoT hub shared access policies](Images/iot-hub-shared-access-policies.png?raw=true)

5. Select the Shared access policy called **iothubowner**, and take note of the **Primary key** and **connection string** on the right blade.

	![Get IoT Hub owner connection string](Images/get-iot-hub-owner-connection-string.png?raw=true)

#### Creating a Stream Analitycs Job

To create a Stream Analytics Job, perform the following steps.

1. In the Azure Management Portal, click **NEW**, then click **DATA SERVICES**, then **STREAM ANALYTICS**, and finally **QUICK CREATE**.
2. Enter the **Job Name**, select a **Region**, such as _East US_; and the enter a **NEW STORAGE ACCOUNT NAME** if this is the first storage account in the region used for Stream Analitycs; if not you have to select the one already used for that matter.
3. Click **CREATE A STREAM ANALITYCS JOB**.

	![Creating a Stream Analytics Job](Images/createStreamAnalytics.png?raw=true)

	_Creating a Stream Analytics Job_


4. After the _Stream Analytics_ is created, in the left pane click **STORAGE**, then click the account you used in the previous step, and click **MANAGE ACCESS KEYS**. Write down the **STORAGE ACCOUNT NAME** and the **PRIMARY ACCESS KEY** as you will use those value later.

	![manage access keys](Images/manage-access-keys.png?raw=true)

	_Managing Access Keys_

<a name="Task14" />
### Registering your device
You must register your device in order to be able to send and receive information from the Azure IoT Hub. This is done by registering a [Device Identity](https://azure.microsoft.com/en-us/documentation/articles/iot-hub-devguide/#device-identity-registry) in the IoT Hub.

1. Open the Device Explorer app and fill the **IoT Hub Connection String** field with the connection string of the IoT Hub you created in previous steps and click on **Update**.

	![Configure Device Explorer](Images/configure-device-explorer.png?raw=true)

2. Go to the **Management** tab and click on the **Create** button. The Create Device popup will be displayed. Fill the **Device ID** field with a new Id for your device (_myFirstDevice_ for example) and click on **Create**:

	![Creating a Device Identity](Images/creating-a-device-identity.png?raw=true)

3. Once the device identity is created, it will be displayed in the grid. Right click on the identity you just created, select **Copy connection string for selected device** and take note of the value copied to your clipboard, since it will be required to connect your device with the IoT Hub.

	![Copying Device connection information](Images/copying-device-connection-information.png?raw=true)

	> **Note:** The device identities registration can be automated using the Azure IoT Hubs SDK. An example of how to do that can be found [here](https://azure.microsoft.com/en-us/documentation/articles/iot-hub-csharp-csharp-getstarted/#create-a-device-identity).

<a name="Task2" />
##Creating an Application##
Now that the device is configured, you will see how to create an application to read the value of the FEZ HAT sensors, and then send those values to an Azure IoT Hub.

<a name="Task21" />
###Deploying a Simple Application###

There are several ways to deploy a .Net application to a Raspberry Pi running a Linux based operating system. For the purpose of this lab the simple copy approach will be used which implies the following steps:

- Write and Build the code using Visual Studio Community Edition
- Copy binary files to the device
- Run the app using Mono

<a name="Task211" />
#### Write the code ####
1. Open the Visual Studio 2015 (Community Edition is sufficient) and create a new Console Application.
 	1. Click on **File** / **New** / **Project...**


		![Raspbian File New Project](Images/raspbian-file-new-project.png?raw=true)

	1. Select a Visual C# Console Application and name it **RaspbianFezHat**:
	
		![Create a new Console Application](Images/create-a-new-console-application.png?raw=true)

1. Open the **Program.cs** file and replace the **Main** method for this one:

	````C#
	static void Main(string[] args)
	{
	    Console.WriteLine("Running on {0}", Environment.OSVersion);

	    Console.WriteLine("Press a key to continue");
	    Console.ReadKey(true);
	}
	````
	
1. To deploy the application just Build your solution by clicking on **Build / Rebuild** Solution

	![Build Hello World](Images/build-hello-world.png?raw=true)

	And once the app has been successfully built, find the generated binaries in the **bin\** folder inside the solution folder:
	
	![Find binary files](Images/find-binary-files.png?raw=true)
	
1. If you want you can run the application in your development machine, since it's a regular Windows console app. Just double-click in the **RaspbianFezHat.exe** file:

	![Run Hello World app in Windows](Images/run-hello-world-app-in-windows.png?raw=true)

<a name="Task212" />
#### Copy the binary files ####
1. Now, to run the app in the Raspberry Pi you need to copy the binaries to the device using the FTP service you configured in the Setup section. Copy all the files from the **bin\Debug** folder.

1. Open a new File Explorer, type **ftp://\<your-device-ip-or-name\>** in the address bar and hit enter. Enter a valid username and password if you are asked for your credential. 
	
	![FTP Ask for user credentials](Images/ftp-ask-for-user-credentials.png?raw=true)

	> **Note:** A FTP Client can also be used to transfer the files from your development PC to your Raspberry Pi.

	
1. After the connection with the FTP is established you can move the files to the device. Create a new folder **/files/HelloWorld** first and then paste the copied files there.

<a name="Task213" />
#### Run the App using Mono ####
1. Once the files were successfully transfered you are ready to run the application in the Raspberry Pi. Login in a Raspbian console. If you want to connect to the device with a remote computer use **PuTTY** or other SSH client.

1. Login in the device with the same user used to connect to the FTP server:

	![Raspberry Pi Login](Images/raspberry-pi-login.png?raw=true)
	
1. Change the current directory to the folder where you copied the files by running the following command:

	````Shell
	$ cd files/HelloWorld/
	````

1. To run the app you just copied execute the following command:

	````Shell
	$ sudo mono RaspbianFezHat.exe
	````

	![Run Hello World app in Raspbian](Images/run-hello-world-app-in-raspbian.png?raw=true)

<a name="Task22" />
###Reading the FEZ HAT sensors###
Now that you know how to deploy an application to a Raspbian device, you will see how it can be modified to read real data from one of the FEZ HAT sensors and show that information on the console.

<a name="Task221" />
#### Adding FEZ HAT driver references ####
Before you can write code to read data from the hat sensors you need to add the references to the FEZ HAT drivers which are included in the **Assets/FezHatDrivers** folder.

1. Copy all the assemblies in the **Assets/FezHatDrivers** folder to your file system.

1. Add a reference to every assembly in that folder. In the Visual Studio's Solution Explorer right click on the project's name and click in **Add** / **Reference...**

	![Add FEZ HAT References](Images/add-fez-hat-references.png?raw=true)
	
1. In the **Reference Manager** window click on the **Browse** button at the bottom of the popup and select all the files in your local folder.

	![Browse References](Images/browse-references.png?raw=true)
	![Select all assemblies](Images/select-all-assemblies.png?raw=true)
	
1. Click **OK** and the assemblies will be added to the project. 

<a name="Task222" />
#### Adding code to read sensor data ####
Now you will see how simple is to pull data out of the hat sensors using the assemblies you referenced in the previous step.

1. Open the **Program.cs** class and add the following line to the **Using** section:

	````C#
	using GHIElectronics.Mono.Shields;
	````

1. Declare the following variable in the **Program** class which will contain a reference to the FEZ HAT driver:

	````C#
	private static FEZHAT hat;
	````

1. Add the following method to initialize the driver:

	````C#
	private static async Task SetupHatAsync()
	{
	    // Initialize Fez Hat
	    hat = await FEZHAT.CreateAsync();
	}
````

	
1. Replace the **Main** method with the following code:
	<!-- mark:5,6,11 -->
	````C#
	static void Main(string[] args)
	{
	    Console.WriteLine("Running on {0}", Environment.OSVersion);

	    Program.SetupHatAsync().Wait();	    
	    Console.WriteLine("The temperature is {0} °C", hat.GetTemperature().ToString("N2", System.Globalization.CultureInfo.InvariantCulture));
		
	    Console.WriteLine("Press a key to continue");
	    Console.ReadKey(true);
	    
	    hat.Dispose();
	}
	````

<a name="Task223" />
#### Changing the target .NET framework ####
Before the application can be deployed one last task needs to be done. As some of the .Net 4.6 framework components are yet to be implemented in Mono, the application won't run in that version. To fix this just change the project's Target Framework before building it.

1. Right-click on the project name and select **Properties**.

1. Select **Application** in the left pane

1. Select **.NET Framework 4.5** in the **Target Framework** dropdown list.

	![Changing Project Target Framework](Images/changing-project-target-framework.png?raw=true)

<a name="Task224" />
#### Deploying the app ####
Build and deploy the application to the Raspberry Pi following the same directions as the previous step ([Deploying a Simple Application](#Task21)). If you wish you can create a new remote folder to avoid overwritting the Simple application you deployed before. 

![Running Temperature Sensor](Images/running-temperature-sensor.png?raw=true)
	
<a name="Task23" />
### Connecting the Application to Azure ###
In the following section you will learn how to use the ConnectTheDots architecture to send the information gathered from the FEZ HAT sensors to Azure.

> **Note:** At the time this lab was written the [Azure IoT device SDK for .NET](https://github.com/Azure/azure-iot-sdks/blob/master/csharp/device/readme.md) wasn't available for Raspbian (running over Mono). The code presented in the following sections is an adaptation of the ConnectTheDots project using the [AMQP .Net Lite library](https://github.com/Azure/amqpnetlite) to stablish the connection with the Azure IoT Hub.


<a name="Task231" />
#### Adding ConnectTheDots infrastructure ####
The fist thing you need to do to connect to Azure is to add the following classes:

- **ConnectTheDotsSensor.cs** which represents an individual ConnectTheDots sensor
- **ConnectTheDotsHelper.cs** which contains the methods needed to send sensor information to an Azure IoT Hub.

Follow these instructions:

1. In Visual Studio right-click on the project name and select **Add** / **Class...**

	![Add new class](Images/add-new-class.png?raw=true)
	
1. Name it **ConnectTheDotsSensor** and click **Add**:

	![Adding ConnectTheDotsSensor class](Images/adding-connectthedotssensor-class.png?raw=true)
	
1. Replace the content of the class with the following code

	````C#
	namespace RaspbianFezHat
	{
		using Newtonsoft.Json;

		/// <summary>
		/// Class to manage sensor data and attributes 
		/// </summary>
		public class ConnectTheDotsSensor
		{
			/// <summary>
			/// Default parameterless constructor needed for serialization of the objects of this class
			/// </summary>
			public ConnectTheDotsSensor()
			{
			}

			/// <summary>
			/// Construtor taking parameters guid, measurename and unitofmeasure
			/// </summary>
			/// <param name="guid"></param>
			/// <param name="measurename"></param>
			/// <param name="unitofmeasure"></param>
			public ConnectTheDotsSensor(string guid, string measurename, string unitofmeasure)
			{
				this.GUID = guid;
				this.MeasureName = measurename;
				this.UnitOfMeasure = unitofmeasure;
			}

			[JsonProperty("guid")]
			public string GUID { get; set; }

			[JsonProperty("displayname")]
			public string DisplayName { get; set; }

			[JsonProperty("organization")]
			public string Organization { get; set; }

			[JsonProperty("location")]
			public string Location { get; set; }

			[JsonProperty("measurename")]
			public string MeasureName { get; set; }

			[JsonProperty("unitofmeasure")]
			public string UnitOfMeasure { get; set; }

			[JsonProperty("timecreated")]
			public string TimeCreated { get; set; }

			[JsonProperty("value")]
			public double Value { get; set; }

			/// <summary>
			/// ToJson function is used to convert sensor data into a JSON string to be sent to Azure IoT Hub
			/// </summary>
			/// <returns>JSon String containing all info for sensor data</returns>
			public string ToJson()
			{
				string json = JsonConvert.SerializeObject(this);

				return json;
			}
		}
	}
	````

1. Following the same steps, create a new class named **ConnectTheDotsHelper** and replace its content with the following code:

	````C#
	namespace RaspbianFezHat
	{
		using System;
		using System.Collections.Generic;
		using System.Globalization;
		using System.Net;
		using System.Text;
		using System.Threading;
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
	#if !SENDAPPPROPERTIES

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
	````

<a name="Task232" />
#### Adding a simple logger ####
In order to show the output (information or errors) of the application in the console you will implement a simple logger class. Create a class with the name **SimpleLogger** and replace its content with this code:

````C#
namespace RaspbianFezHat
{
    using System;

    public class SimpleLogger
    {
        public void Info(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void Error(string format, params object[] args)
        {
            Console.WriteLine("ERROR: " + format, args);
        }
    }
}
````

<a name="Task233" />
#### Adding required references ####
In this section you will add the references to the components required for the application to work:

1. **Newtonsoft.Json NuGet Package**: Open the NuGet Package Manager Console (**Tools** / **NuGet Package Manager** / **Package Manager Console**) and execute the following command:

	````PowerShell
	Install-Package Newtonsoft.Json
	````

	Which will install the library used to serialize the sensor information objects before send the information to Azure.
	
1. **AMQPNetLite**: Using the same NuGet Package Console, execute the following command to install the AMQPNetLite library which will be used to send messages to Azure using the [AMQP protocol](https://www.amqp.org/):

	````PowerShell
	Install-Package AMQPNetLite -version 1.1.1
	````

	> **Note:** We used the specific version 1.1.1. since currently the last version 1.2.1 doesn't work with Mono (it uses a Socket constructor not supported by Mono).
	
1. **System.Web**: Add the reference to this library by right-clicking in the Project name and selecting **Add** / **Reference**.

	Select **Assemblies** on the left pane and then select **System.Web** in the Assemblies list and click **OK**:
	
	![Referencing System.Web assembly](Images/referencing-systemweb-assembly.png?raw=true)

<a name="Task234" />
#### Adding code to Send data to Azure ####
Now you are ready to use the ConnectTheDots plattform to collect the data from the sensors and send them to an Azure IoT Hub.

1. Open the **Program.cs** file and add the following code to the _using_ section:

	````C#
	using System.Globalization;
	using System.Timers;
	````

2. Add the following variable declarations to the class declaration section:

	````C#
	private static SimpleLogger logger = new SimpleLogger();
	private static ConnectTheDotsHelper ctdHelper;
	private static System.Timers.Timer telemetryTimer;
	````
	
	The **telemetryTimer** will be used to poll the hat sensors at regular basis. For every _tick_ of the timer the value of the sensors will be get and sent to Azure. **logger** and **ctdHelper** are instances of the recently created **SimpleLogger** and **ConnectTheDotsHelper** classes respectively.

1. Replace the **SetupHatAsync** method with the following code:

	````C#
	private static async Task SetupHatAsync()
	{
		// Initialize Fez Hat
		logger.Info("Initializing FEZ HAT");
		hat = await FEZHAT.CreateAsync();

		// Initialize Timer
		telemetryTimer = new System.Timers.Timer(2000); 
		telemetryTimer.Elapsed += TelemetryTimer_Elapsed;
		telemetryTimer.Enabled = true;

		Program.logger.Info("FEZ HAT Initialized");
	}
	````

	Which adds code to initialize the timer to tick every 2 seconds.
	
1. Replace the **Main** method with the following code:

	````C#
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

			Console.WriteLine("Reading data from FEZ HAT sensors. Press Enter to Stop");
			Console.ReadLine();
			Console.WriteLine("Closing...");
			Program.telemetryTimer.Enabled = false;
			Program.telemetryTimer.Dispose();
			Program.hat.Dispose();
			Program.ctdHelper.Dispose();
		}
		else
		{
			Console.WriteLine("An error ocurred while connecting to Azure. See previous errors for details.");
			Console.WriteLine("Press Enter to exit");
			Program.ctdHelper.Dispose();
		}
	}
	````

	The first line initializes the ConnectTheDots sensor list. In this case two sensor objects are created (one for the temperature sensor and another for the light)
	<!-- mark:3,4 -->
	````C#
	// Hard coding guid for sensors. Not an issue for this particular application which is meant for testing and demos
	List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor> {
		new ConnectTheDotsSensor("2298a348-e2f9-4438-ab23-82a3930662ab", "Light", "L"),
		new ConnectTheDotsSensor("d93ffbab-7dff-440d-a9f0-5aa091630201", "Temperature", "C"),
	};
	````

	The next statement initializes the ConnectTheDotsHelper object, which receives, among other parameters, the IoT Hub connection settings. 
	
	````C#
	Program.ctdHelper = new ConnectTheDotsHelper(
		logger: Program.logger,
		iotHubHost: "IOT_HOST",
		deviceName: "DEVICE_ID",
		deviceKey: "DEVICE_KEY",
		organization: "YOUR_ORGANIZATION_OR_SELF",
		location: "YOUR_LOCATION",
		eventHubMessageSubject: "Raspberry PI",
		sensorList: sensors);
	````

	In order to allow the application to send data to the **IoT Hub**, the following information must be provided:

	- **IoT Host**: Is the host name given to your IoT Hub. In the **Azure Management Portal** open the blade of your IoT Hub and you will see the **Hostname**.
	
		![IoT Hub hostname](Images/iot-hub-hostname.png?raw=true)
	
	- **Device ID**: Is the name choosen by you during the [Device Registration](#Task14).
	- **Device Key**: Is the primary key given during your device registration. Both values can be get from the **Management** tab of the **Device Explorer** application:
	
		![Get Device's connection information](Images/iot-hub-get-device-connection-info.png?raw=true)
		
		> **Note:** All the connection settings can also be extracted from the Device's connection string, which has the following structure: 
		````	
		HostName=<IOT_HOST>;DeviceId=<DEVICE_ID>;SharedAccessKey=<DEVICE_KEY>
		````

	The following statement is the call to the FEZ HAT initializer
	
	````C#
	// Setup the FEZ HAT driver
	Program.SetupHatAsync().Wait();
	````

	And the last piece of code is for disposing the objects used.
	
	````C#
	Console.WriteLine("Reading data from FEZ HAT sensors. Press Enter to Stop");
	Console.ReadLine();
	Console.WriteLine("Closing...");
	Program.telemetryTimer.Enabled = false;
	Program.telemetryTimer.Dispose();
	Program.hat.Dispose();
	Program.ctdHelper.Dispose();
	````
	
1. Create a new method called **TelemetryTimer_Elapsed** with the following code:

	````C#
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
	````

	This method will be executed every time the timer ticks, and will poll the value of the hat's temperature and light sensors, send them to the Azure IoT Hub and show the value obtained in the console.
	
	For every sensor, Light and Temperature, the following tasks are performed:
	
	- Gets the _ConnectTheDots_ sensor from the _sensors_ collection
	- The temperature and light are polled out from the hat's sensors using the driver object initialized in previous steps
	- The values obtained are sent to Azure using the _ConnectTheDots_'s helper object **ctdHelper** 

<a name="Task235" />
#### Syncing Trusted Root Certificates ####
In order to perform a connection using a secure protocol (SSL), Mono requires to trust in the remote site. To ensure that the connection with the Azure IoT Hub is successfully stablished, run the following command in a Raspbian console, which will download the trusted root certificates from the Mozilla LXR web site into the Mono certificate store:

````Shell
$ sudo mozroots --import --sync
````

<a name="Task236" />
#### Deploying the app ####
Build and deploy the application to the Raspberry Pi following the same directions as [Deploying a Simple Application](#Task21) section. If you wish you can create a new remote folder to avoid overwritting the applications you deployed before. 

![Running Final Application](Images/running-final-application.png?raw=true)

<a name="Task3" />
## Consuming the IoT Hub data
You have seen how to use the Device Explorer to peek the data being sent to the Azure IoT Hub. However, the Azure IoT suite offers many different ways to generate meaningful information from the data gathered by the devices. In the following sections you will explore two of them: You will see how the Azure Services Bus Messaging system can be used in a Website (part of the ConnectTheDots project), and how to use Azure Stream Analytics in combination with Microsoft Power BI to consume the data and to generate meaningful reports.

<a name="Task32" />
### Using Power BI

One of the most interesting ways to use the information received from the connected device/s is to get near real-time analysis using the **Microsoft Power BI** tool. In this section you will see how to configure this tool to get an online dashboard showing summarized information about the different sensors.

<a name="Task321" />
#### Setting up a Power BI account
If you don't have a Power BI account already, you will need to create one (a free account is enough to complete this lab). If you already have an account set you can skip this step.

1. Go to the [Power BI website](https://powerbi.microsoft.com/) and follow the sign-up process.

	> **Note:** At the moment this lab was written, only users with corporate email accounts are allowed to sign up. Free consumer email accounts (like Outlook, Hotmail, Gmail, Yahoo, etc.) can't be used.

2. You will be asked to enter your email address. Then a confirmation email will be sent. After following the confirmation link, a form to enter your personal information will be displayed. Complete the form and click Start.

	The preparation of your account will take several minutes, and when it's ready you will see a screen similar to the following:

	![Power BI Welcome screen](Images/power-bi-welcome-screen.png?raw=true)

	_Power BI welcome screen_

Now that your account is set, you are ready to set up the data source that will feed the Power BI dashboard.

<a name="Task3220" />
##### Create a Service Bus Consumer Group
In order to allow several consumer applications to read data from the IoT Hub independently at their own pace a Consumer Group must be configured for each one. If all of the consumer applications (the Device Explorer, Stream Analytics / Power BI, the Web site you will configure in the next section) read the data from the default consumer group, one application will block the others.

To create a new Consumer Group for the IoT Hub that will be used by the Stream Analytics job you are about to configure, follow these steps:

- Open the Azure Portal (portal.azure.com), and select the IoT Hub you created.
- From the settings blade, click on **Messaging**
- At the bottom of the Messaging blade, type the name of the new Consumer Group "PowerBI"
- From the top menu, click on the Save icon

![Create Consumer Group](Images/create-consumer-group.png?raw=true)

<a name="Task322" />
#### Setting the data source
In order to feed the Power BI reports with the information gathered by the hats and to get that information in near real-time, **Power BI** supports **Azure Stream Analytics** outputs as data source. The following section will show how to configure the Stream Analytics job created in the Setup section to take the input from the IoT Hub and push that summarized information to Power BI.

<a name="Task3221" />
##### Stream Analytics Input Setup
Before the information can be delivered to **Power BI**, it must be processed by a **Stream Analytics Job**. To do so, an input for that job must be provided. As the Raspberry devices are sending information to an IoT Hub, it will be set as the input for the job.

1. Go to the Azure management portal and select the **Stream Analytics** service. There you will find the Stream Analytics job created during the [Azure services setup](#Task13). Click on the job to enter the Stream Analytics configuration screen.

	![Stream Analytics configuration](Images/stream-analytics-configuration.png?raw=true)

	_Stream Analytics Configuration_

2. As you can see, the Start button is disabled since the job is not configured yet. To set the job input click on the **INPUTS** tab and then in the **Add an input** button.

3. In the **Add an input to your job** popup, select the **Data Stream** option and click **Next**. In the following step, select the option **IoT Hub** and click **Next**. Lastly, in the **IoT Hub Settings** screen, provide the following information:

	- **Input Alias:** _TelemetryHub_
	- **Subscription:** Use IoT Hub from Current Subscription (you can use an IoT Hub from another subscription too by selecting the other option)
	- **Choose an IoT Hub:** _sample-iot_ (or the name used during the IoT Hub creation)
	- **IoT Hub Shared Access Policy Name:** _iothubowner_
	- **IoT Hub Consumer Group:** _powerbi_

	![Stream Analytics Input configuration](Images/stream-analytics-input-configuration.png?raw=true)

	_Stream Analytics Input Configuration_

4. Click **Next**, and then **Complete** (leave the Serialization settings as they are).

<a name="Task3222" />
##### Stream Analytics Output Setup
The output of the Stream Analytics job will be Power BI.

1. To set up the output, go to the Stream Analytics Job's **OUTPUTS** tab, and click the **ADD AN OUTPUT** link.

2. In the **Add an output to your job** popup, select the **POWER BI** option and the click the **Next button**.

3. In the following screen you will setup the credentials of your Power BI account in order to allow the job to connect and send data to it. Click the **Authorize Now** link.

	![Stream Analytics Output configuration](Images/steam-analytics-output-configuration.png?raw=true)

	_Stream Analytics Output Configuration_

	You will be redirected to the Microsoft login page.

4. Enter your Power BI account email and password and click **Continue**. If the authorization is successful, you will be redirected back to the **Microsoft Power BI Settings** screen.

5. In this screen you will enter the following information:

	- **Output Alias**: _PowerBI_
	- **Dataset Name**: _Raspberry_
	- **Table Name**: _Telemetry_
	- **Group Name**: _My Workspace_

	![Power BI Settings](Images/power-bi-settings.png?raw=true)

	_Power BI Settings_

6. Click the checkmark button to create the output.

<a name="Task3223" />
##### Stream Analytics Query configuration
Now that the job's inputs and outputs are already configured, the Stream Analytics Job needs to know how to transform the input data into the output data source. To do so, you will create a new Query.

1. Go to the Stream Analytics Job **QUERY** tab and replace the query with the following statement:

	````SQL
	SELECT
		iothub.iothub.connectiondeviceid displayname,
		location,
		guid,
		measurename,
		unitofmeasure,
		Max(timecreated) timecreated,
		Avg(value) AvgValue
	INTO
		[PowerBI]
	FROM
		[TelemetryHUB] TIMESTAMP by timecreated
	GROUP BY
		iothub.iothub.connectiondeviceid, location, guid, measurename, unitofmeasure,
		TumblingWindow(Second, 10)
	````

	The query takes the data from the input (using the alias defined when the input was created **TelemetryHUB**) and inserts into the output (**PowerBI**, the alias of the output) after grouping it using 10 seconds chunks.

2. Click on the **SAVE** button and **YES** in the confirmation dialog.

<a name="Task3234" />
##### Starting the Stream Analytics Job
Now that the job is configured, the **START** button is enabled. Click the button to start the job and then select the **JOB START TIME** option in the **START OUTPUT** popup. After clicking **OK** the job will be started.

Once the job starts it creates the Power BI datasource associated with the given subscription.

<a name="Task324" />
#### Setting up the Power BI dashboard
1. Now that the datasource is created, go back to your Power BI session, and go to **My Workspace** by clicking the **Power BI** link.

	After some minutes of the job running you will see that the dataset that you configured as an output for the Job, is now displayed in the Power BI workspace Datasets section.

	![Power BI new datasource](Images/power-bi-new-datasource.png?raw=true)

	_Power BI: New Datasource_

	> **Note:** The Power BI dataset will only be created if the job is running and if it is receiving data from the IoT Hub input, so check that the App is running and sending data to Azure to ensure that the dataset be created. To check if the Stream Analytics job is receiving and processing data you can check the Azure management Stream Analytics monitor.

2. Once the datasource becomes available you can start creating reports. To create a new Report click on the **Raspberry** datasource:

	![Power BI Report Designer](Images/power-bi-report-designer.png?raw=true)

	_Power BI: Report Designer_

	The Report designer will be opened showing the list of fields available for the selected datasource and the different visualizations supported by the tool.

3. To create the _Average Light by time_ report, select the following fields:

	- avgvalue
	- timecreated

	As you can see the **avgvalue** field is automatically set to the **Value** field and the **timecreated** is inserted as an axis. Now change the chart type to a **Line Chart**:

	![Select Line Chart](Images/select-line-chart.png?raw=true)

	_Selecting the Line Chart_

4. Then you will set a filter to show only the Light sensor data. To do so drag the **measurename** field to the **Filters** section and then select the **Light** value:

	![Select Report Filter](Images/select-report-filter.png?raw=true)
	![Select Light sensor values](Images/select-light-sensor-values.png?raw=true)

	_Selecting the Report Filters_

5. Now the report is almost ready. Click the **SAVE** button and set _Light by Time_ as the name for the report.

	![Light by Time Report](Images/light-by-time-report.png?raw=true)

	_Light by Time Report_

6. Now you will create a new Dashboard, and pin this report to it. Click the plus sign (+) next to the **Dashboards** section to create a new dashboard. Set _Raspberry Telemetry_ as the **Title** and press Enter. Now, go back to your report and click the pin icon to add the report to the recently created dashboard.

	![Pin a Report to the Dashboard](Images/pin-a-report-to-the-dashboard.png?raw=true)

	_Pinning a Report to the Dashboard_

1. To create a second chart with the information of the average Temperature follow these steps:
	1. Click on the **Raspberry** datasource to create a new report.
	2. Select the **avgvalue** field
	3. Drag the **measurename** field to the filters section and select **Temperature**
	4. Now change the visualization to a **gauge** chart:

		![Change Visualization to Gauge](Images/change-visualization-to-gauge.png?raw=true "Gauge visualization")

		_Gauge visualization_

	5. Change the **Value** from **Sum** to **Average**

		![Change Value to Average](Images/change-value-to-average.png?raw=true)

		_Change Value to Average_

		Now the Report is ready:

		![Gauge Report](Images/gauge-report.png?raw=true)

		_Gauge Report_

	6. Save and then Pin it to the Dashboard.

7. Following the same directions, create a _Temperature_ report and add it to the dashboard.
8. Lastly, edit the reports name in the dashboard by clicking the pencil icon next to each report.

	![Edit Report Title](Images/edit-report-title.png?raw=true)

	_Editing the Report Title_

	After renaming both reports you will get a dashboard similar to the one in the following screenshot, which will be automatically refreshed as new data arrives.

	![Final Power BI Dashboard](Images/final-power-bi-dashboard.png?raw=true)

	_Final Power BI Dashboard_

<a name="Task33" />
### Consuming the IoT Hub data from a Website

1. Before starting you need to create Consumer Groups to avoid colliding with the Stream Analytics job, in the same way you did in the [Using Power BI section](Task3220). The website needs two different consumer groups:
  - _website_
  - _local_ (used when debugging)
  
	![Adding website consumer groups](Images/adding-website-consumer-groups.png?raw=true)
  
2. Also take note of the **Event Hub-compatible name** and **Event Hub-compatible endpoint** values in the _Messaging_ blade

3. Browse to the **Assets/WebSite** folder and open the Web Site project (_ConnectTheDotsWebSite.sln_) in Visual Studio.

4. Edit the _Web.config_ file and add the corresponding values for the following keys:
	- **Microsoft.ServiceBus.EventHubDevices**: Event hub-compatible name.
	- **Microsoft.ServiceBus.ConnectionStringDevices**: Event hub-compatible connection string which is composed by the **Event hub-compatible endpoint** and the **_iothubowner_ Shared access policy Primary Key**.
	![IoT Hub shared access policy primary key](Images/iot-hub-shared-access-policy-primary-key.png?raw=true)
	
	- **Microsoft.Storage.ConnectionString**: Storage account endpoint, in this case use the **storage account name** and **storage account primary key** to complete the endpoint.

5. Deploy the website to an Azure Web Site. To do this, perform the following steps.
	1. In Visual Studio, right-click on the project name and select **Publish**.
	2. Select **Microsoft Azure Web Apps**.

		![Selecting Publish Target](Images/selecting-publish-target.png?raw=true)

		_Selecting Publish target_

	3. Click **New** and use the following configuration.

		- **Web App name**: Pick something unique.
		- **App Service plan**: Select an App Service plan in the same region used for _Stream Analytics_ or create a new one using that region.
		- **Region**: Pick same region as you used for _Stream Analytics_.
		- **Database server**: No database.

	4. Click **Create**. After some time the website will be created in Azure.

		![Creating a New Web App](Images/creating-a-new-web-app.png?raw=true)

		_Creating a new Web App on Microsoft Azure_

	3. Click **Publish**.

		> **Note:** You might need to install **WebDeploy** extension if you are having an error stating that the Web deployment task failed. You can find WebDeploy [here](http://www.iis.net/downloads/microsoft/web-deploy).


6. After you deployed the site, it is required that you enable **Web sockets**. To do this, perform the following steps:
	1. Browse to https://portal.azure.com and select your _Azure Web App.
	2. Click **All settings**.
	3. Click **Applicattion settings**
	4. Then set **Web sockets** to **On** and click **Save**.

		![Enabling Web Sockets](Images/enabling-web-sockets.png?raw=true)

		_Enabling Web Sockets in your website_

7. Browse to your recently deployed Web Application. You will see something like in the following screenshot. There will be 2 real-time graphics representing the values read from the temperature and light sensors. Take into account that the app must be running and sending information to the IoT Hub in order to see the graphics.

	![Web Site Consuming the IoT Hub Data](Images/web-site-consuming-the-event-hub-data.png?raw=true)

	_Web Site Consuming the IoT Hub data_

	> **Note:** At the bottom of the page you should see “**Connected**.”. If you see “**ERROR undefined**” you likely didn’t enable **WebSockets** for the Azure Web Site.

<a name="Task4">
## Sending commands to your devices
Azure IoT Hub is a service that enables reliable and secure bi-directional communications between millions of IoT devices and an application back end. In this section you will see how to send cloud-to-device messages to your device to command it to change the color of one of the FEZ HAT leds, using the Device Explorer app as the back end.

1. Open the app you created before and add the following line to the **using** section of the **ConnectTheDotsHelper.cs** file

	````C#
	using System.Threading.Tasks;
	````

2. Then add the following method:

	````C#
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
	````

	The _ReceiveAsync_ method returns the received message at the time that it is received by the device. The call to _Accept()_ notifies IoT Hub that the message has been successfully processed and that it can be safely removed from the device queue. If something happened that prevented the device app from completing the processing of the message, IoT Hub will deliver it again.
	
2. Now you will add the logic to process the messages received. Open the **Program.cs** file and add the following line to the **using** section: 

	````C#
	using System.Threading;
	````

3. Add the following method, which will be on charge of processing the commands:

	````C#
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
							System.Diagnostics.Debug.WriteLine("Unrecognized command: {0}", command);
							break;
					}
				}
			}
		}
	}
	````

	On every iteration it awaits until a message is received (or after the AMQP default timeout period), and according to the text of the command, it set the value of the _hat.D2.Color_ attribute to change the color of the FEZ HAT's LED D2. When the "OFF" command is received the _TurnOff()_ method is called, which turns the LED off. The cancellation token is used to indicate the receiver that it should stop receiving messages.
	
4. Lastly, you will add the call to the **ReceiveCommandsAsync** method. To do so you will need to slightly modify the way the app is closed because a cancellation token must be provided to the _Receiver_. Locate the following code in the **Main** method: 

	````C#
	Console.WriteLine("Reading data from FEZ HAT sensors. Press Enter to Stop"); 
	Console.ReadLine(); 
	Console.WriteLine("Closing..."); 
	Program.telemetryTimer.Enabled = false; 
	Program.telemetryTimer.Dispose(); 
	Program.hat.Dispose(); 
	Program.ctdHelper.Dispose(); 
	````

	And replace it with these lines:
	
	````C#
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
	````

	The cancellation token is canceled as part of the stopping process (right after the user hits _Enter_ in the console) and it's used to tell the _Receiver_ that it should stop trying to receive messages from the IoT hub. As the _Receiver_ executes in an asynchronous operation the _Main_ flow must be blocked until it finishes, to ensure the proper process termination. 
	
5. Deploy and run the app on the device and open the Device Explorer app.

6. Once it's loaded (and configured to point to your IoT hub), go to the **Messages To Device** tab, check the **Monitor Feedback Endpoint** option, select your device from the **Device ID** dropdown list and write your command in the **Message** field. Click on **Send**

	![Sending cloud-to-device message](Images/sending-cloud-to-device-message.png?raw=true)

7. After a few seconds the message will be processed by the device and the LED will turn on in the color you selected. The feedback will also be reflected in the Device Explorer screen after a few seconds.

	![cloud-to-device message received](Images/cloud-to-device-message-received.png?raw=true)

	
<a name="Summary" />
##Summary##
In this lab, you have learned how to create a simple app that reads from the sensors of a FEZ hat connected to a Raspberry Pi 2 running Raspbian, and upload those readings to an Azure IoT Hub. You also learned how to read and consume the information in the IoT Hub using Power BI to get near real-time analysis of the information gathered from the FEZ hat sensors and to create simple reports and how to consume it using a website. You also saw how to use the IoT Hubs Cloud-To-Device messages feature to send simple commands to your devices.
