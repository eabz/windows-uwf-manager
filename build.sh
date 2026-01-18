#!/bin/bash
echo "Building Windows UWF Manager for win-x64..."

# Publish the single-file executable
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build Successful!"
    echo "Executable is located at:"
    echo "UwfManager/bin/Release/net8.0-windows/win-x64/publish/UwfManager.exe"
    echo ""
    echo "To create the Installer:"
    echo "1. Copy the 'publish' folder or just the .exe to a Windows machine."
    echo "2. Install Inno Setup."
    echo "3. Compile 'setup.iss'."
else
    echo "❌ Build Failed."
fi
