# Windows UWF Manager

A simple, modern GUI application to manage the **Unified Write Filter (UWF)** on Windows IoT/Enterprise.
This application allows administrators to easily "Lock" (enable UWF) or "Unlock" (disable UWF) the system, ensuring data persistence control with a single click.

## Prerequisites

1. **Supported OS**: Windows 10/11 Enterprise, Education, or IoT Enterprise.
   * *Note*: UWF is not available on Windows Home or Pro editions.
2. **UWF Feature Enabled**:
   * Run the following command as Administrator to install the feature:
     ```powershell
     dism /online /enable-feature /featurename:Client-UnifiedWriteFilter /all
     ```
3. **.NET 8 Runtime**: Required to run the application (or .NET 8 SDK to build).

## How to Build

### Using Command Line (Recommended)
To create a self-contained single-file executable (easy to distribute):

```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```
The executable `UwfManager.exe` will be located in:
`UwfManager\bin\Release\net8.0-windows\win-x64\publish\`

### Using Visual Studio
1. Open `UwfManager.sln` in **Visual Studio 2022+**.
2. Build the solution (**Ctrl+Shift+B**).

## Creating the Installer
This project includes a setup script for **Inno Setup**.

1. Download and install [Inno Setup](https://jrsoftware.org/isdl.php).
2. Build the project using the "Command Line" instruction above.
3. Open `setup.iss` with Inno Setup Compiler.
4. Click **Compile**.
5. The installer `UwfManagerSetup.exe` will be generated in the `Installer\` directory.

## Usage
1. Right-click `UwfManager.exe` (or the installed app shortcut) and **Run as Administrator**.
2. The Status indicator shows if the system is currently **Locked** (Protected) or **Unlocked** (Editable).
3. Click the toggle button to change the state.
4. A system **Restart** is required for changes to take effect.
