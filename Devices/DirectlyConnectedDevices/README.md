For Microsoft Field IoT Labs, you'll want to use one of the following labs. Each folder contains a readme file with further step-by-step instructions. Important links for further information and exploration are included at the bottom.

These labs include simplified versions of the client code, designed to help the lab participants focus on accomplishing their goal of getting data to Azure, and analyzing it. They may be built upon using other pieces from the larger Connect the Dots, or expanded to use additional sensors included in the FEZ HAT.

**Be sure you are in the IoT-Field-Labs branch when viewing and downloading code.**

#New IoT Hub-based labs
The latest version of these labs use IoT Hub and support bi-directional communication. These labs are also updated with the latest Azure Portal.

**NOTE: Before connecting anything to the Pi or Fez HAT, either add 11mm or 12mm standoffs between them, or add some electrical tape to cover the top of the HDMI connector. Pressing down on the HAT so that the through-hole solder joints make contact with the HDMI connector will send voltage to ground, toasting the HAT and possibly the Pi.**

##WindowsIoTCorePi2FezHat-IoTHubs
This is the version using Windows 10 IoT Core on a Raspberry Pi 2. It requires a Windows 10 PC for development. Products and services demonstrated include Windows UWP development, IoT Hubs, Bi-Directional Communication, Azure Web Apps, Power BI, and more.

[View lab here](WindowsIoTCorePi2FezHat-IoTHubs)

##RaspbianFezHat-IoTHubs
This is the version using Raspbian and Mono/C# for development using a PC and a Raspberry Pi 2. It does not require Windows 10, but currently does rely on Visual Studio for development. Products and services demonstrated include Raspberry Pi Raspbian Linux, IoT Hubs, Bi-Directional Communication, Azure Web Apps, Power BI, and more.

[View lab here](RaspbianFezHat-IoTHubs)




#Older Event Hub-based labs
These are here in case you want to use Event Hubs rather than the new IoT Hub. If you aren't sure which to pick, use the above IoT Hub version.

**NOTE: Before connecting anything to the Pi or Fez HAT, either add 11mm or 12mm standoffs between them, or add some electrical tape to the top of the HDMI connector. Pressing down on the HAT so that the through-hole solder joints make contact with the HDMI connector will send voltage to ground, toasting the HAT and possibly the Pi.**

##WindowsIoTCorePi2FezHat
This is the version using Windows 10 IoT Core on a Raspberry Pi 2. It requires a Windows 10 PC for development. Products and services demonstrated include Windows UWP development, Event Hubs, Stream Analytics, Azure Web Apps, Power BI, and more.

[View lab here](WindowsIoTCorePi2FezHat)

##RaspbianFezHat
This is the version using Raspbian and Mono/C# for development using a PC and a Raspberry Pi 2. It does not reuqire Windows 10, but currently does rely on Visual Studio for development. Products and services demonstrated include Raspberry Pi Raspbian Linux, Event Hubs, Stream Analytics, Azure Web Apps, Power BI, and more.

[View lab here](RaspbianFezHat)

##Hardware

In all labs above, the hardware used is a Raspberry Pi 2 and a GHI FEZ HAT. See individual labs for more information.

![](WindowsIoTCorePi2FezHat/Images/fezhat-connected-to-raspberri-pi-2.png?raw=true)

##Important Windows Links

[UWP Developer Center](http://dev.windows.com)

[Windows on Devices](http://windowsondevices.com)

[Windows Embedded/IoT](http://www.microsoft.com/windowsembedded/en-us/windows-embedded.aspx)


##Important Raspberry Pi Links

[Raspberry Pi downloads page](https://www.raspberrypi.org/downloads/)

##Important Azure IoT Suite Links

Note that these are Azure IoT Suite documents. The labs do not currently use IoT Suite, but much of the information is relevant, and can be used to build upon what you have learned in these labs.

[Azure IoT Suite Content](http://www.internetofyourthings.com/)

[Azure IoT Developer Center](https://azure.microsoft.com/en-us/develop/iot/)

[Microsoft Azure Certified for IoT](https://azure.microsoft.com/en-us/marketplace/certified-iot-program/)


###Tutorial Videos

[Introducing Azure IoT Suite](https://azure.microsoft.com/en-us/documentation/videos/azurecon-2015-introducing-the-microsoft-azure-iot-suite/)

[Introducing Azure IoT Hub](https://azure.microsoft.com/en-us/documentation/videos/azurecon-2015-overview-of-azure-iot-hub/)

[Connect your devices with Azure IoT client libraries](https://azure.microsoft.com/en-us/documentation/videos/azurecon-2015-connect-your-iot-devices-with-azure-iot-client-libraries/)


###Documentation

[Azure IoT Suite](http://www.microsoft.com/en-us/server-cloud/internet-of-things/azure-iot-suite.aspx)

[Provisioning Remote Monitoring](http://www.microsoft.com/en-us/server-cloud/internet-of-things/getting-started.aspx)

[Including a physical device in remote monitoring](https://azure.microsoft.com/en-us/documentation/articles/iot-suite-connecting-devices/)

