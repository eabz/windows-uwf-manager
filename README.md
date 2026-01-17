# Windows UWF Manager

## Overview
A simple, modern GUI application to manage the **Unified Write Filter (UWF)** on Windows IoT/Enterprise.
This application allows a user to "Lock" the system (enable UWF) so changes are discarded on reboot, or "Unlock" the system (disable UWF) to make permanent changes.

## Prerequisites
1. **Windows 10/11 Enterprise, Education, or IoT Enterprise**.
   - *Note*: UWF is **not** available on Windows Home or Pro.
2. **UWF Feature Installed**.
   - Enable via "Turn Windows features on or off" > "Device Lockdown" > "Unified Write Filter".
   - Or run: `dism /online /enable-feature /featurename:Client-UnifiedWriteFilter /all`
3. **.NET 8 Runtime** (or the SDK to build).

## How to Build
Since this project was generated on a non-Windows environment, you will need to copy the `UwfManager` folder to a Windows machine.

1. Open `UwfManager.sln` in **Visual Studio 2022** (or later).
2. Right-click the project `UwfManager` and select **Restore NuGet Packages** (if needed).
3. Build the solution (Ctrl+Shift+B).

## How to Run
1. Navigate to the build output folder (e.g., `bin\Debug\net8.0-windows`).
2. Right-click `UwfManager.exe` and select **Run as Administrator**.
   - *Important*: The app requires Admin privileges to interact with UWF commands.
3. Use the GUI to check status and toggle the filter.

## Troubleshooting
- **"Failed to start uwfmgr"**: Ensure the UWF feature is installed on Windows.
- **"Access Denied"**: Make sure you are running as Administrator.
- **Status not updating**: Try restarting the application or the computer.
