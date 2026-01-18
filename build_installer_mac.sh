#!/bin/bash

# Define paths
INNO_SETUP_URL="https://files.jrsoftware.org/is/6/innosetup-6.2.2.exe"
INNO_INSTALLER="innosetup.exe"
ISCC_PATH="$HOME/.wine/drive_c/Program Files (x86)/Inno Setup 6/ISCC.exe"

echo "----------------------------------------------------------------"
echo "   Windows UWF Manager - Cross-Compile Installer (macOS/Linux)"
echo "----------------------------------------------------------------"

# 1. Check for Wine
if ! command -v wine &> /dev/null; then
    echo "❌ Wine is not installed."
    echo "   To install on macOS: brew install --cask wine-stable"
    echo "   After installing, run 'winecfg' once to set up the prefix."
    exit 1
fi
echo "✅ Wine is installed."

# 2. Check for Inno Setup Compiler
if [ ! -f "$ISCC_PATH" ]; then
    echo "⚠️  Inno Setup Compiler not found at default location."
    echo "   $ISCC_PATH"
    echo ""
    read -p "   Do you want to download and install Inno Setup now? (y/n) " -n 1 -r
    echo ""
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "   Downloading Inno Setup..."
        curl -L -o "$INNO_INSTALLER" "$INNO_SETUP_URL"
        echo "   Starting Installer via Wine..."
        echo "   👉 PLEASE INSTALL TO DEFAULT LOCATION (C:\Program Files (x86)\Inno Setup 6)"
        wine "$INNO_INSTALLER"
        rm "$INNO_INSTALLER"
        
        # Re-check
        if [ ! -f "$ISCC_PATH" ]; then
            echo "❌ Still count not find ISCC.exe. Please verify installation path."
            exit 1
        fi
    else
        echo "   Skipping installation. Cannot build installer without Inno Setup."
        exit 1
    fi
fi
echo "✅ Inno Setup found."

# 3. Build the Application first
echo "🔨 Building Application..."
./build.sh
if [ $? -ne 0 ]; then
    echo "❌ App build failed."
    exit 1
fi

# 4. Compile Installer
echo "📦 Creating Installer with Inno Setup..."
# Clean up previous build to avoid "Sharing violation" (Error 32) in Wine
rm -f "Installer/UwfManagerSetup.exe"

wine "$ISCC_PATH" "setup.iss"

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ INSTALLER CREATED SUCCESSFULLY!"
    echo "   Location: Installer/UwfManagerSetup.exe"
else
    echo "❌ Installer compilation failed."
fi
