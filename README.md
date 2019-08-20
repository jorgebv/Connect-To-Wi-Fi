# Connect-To-Wi-Fi
Source code to the Windows Store App "Connect To Wi-Fi"

This application is designed to exercise a few of the connection methods on the [Windows.Devices.WiFi.WiFiAdapter API](https://docs.microsoft.com/en-us/uwp/api/windows.devices.wifi.wifiadapter).

This application supports connection via username/password, via WPS PIN, and via WPS push-button. In some ways, it is more capable than the Windows UI since it allows flexibility in choosing the WPS connection method whereas the Windows UI will default to WPS push-button if it is available. However, this application is in no ways complete and lacks many basic features present in the Windows UI. It is not intended as a fully-featured replacement. It is only intended as a test tool in very specific situations.

The source code does not demonstrate best programming practices and was written quickly to accomplish a singular task. It should not be used as a reference on proper user interface programming guidelines or design.
