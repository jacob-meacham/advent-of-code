#!/bin/bash

# Build script for Advent of Code 2023 .NET projects
# Usage: ./build.sh [RID]
# Examples:
#   ./build.sh linux-x64
#   ./build.sh osx-x64
#   ./build.sh win-x64
#   ./build.sh          # defaults to current platform

set -e

# Detect default RID if not provided
if [ -z "$1" ]; then
    OS="$(uname -s)"
    ARCH="$(uname -m)"
    
    case "$OS" in
        Linux*)
            if [ "$ARCH" = "x86_64" ]; then
                RID="linux-x64"
            elif [ "$ARCH" = "arm64" ] || [ "$ARCH" = "aarch64" ]; then
                RID="linux-arm64"
            else
                RID="linux-x64"  # fallback
            fi
            ;;
        Darwin*)
            if [ "$ARCH" = "x86_64" ]; then
                RID="osx-x64"
            elif [ "$ARCH" = "arm64" ]; then
                RID="osx-arm64"
            else
                RID="osx-x64"  # fallback
            fi
            ;;
        MINGW*|MSYS*|CYGWIN*)
            RID="win-x64"
            ;;
        *)
            echo "Unknown OS: $OS. Please specify RID explicitly."
            exit 1
            ;;
    esac
    echo "Auto-detected RID: $RID"
else
    RID="$1"
fi

echo "Building for RID: $RID"

# Build each project individually (solutions don't support RID directly)
for day_dir in Day*/; do
    if [ -f "${day_dir}${day_dir%/}.csproj" ]; then
        echo "Building ${day_dir%/}..."
        dotnet build "${day_dir}${day_dir%/}.csproj" -c Release -p:RuntimeIdentifier="$RID" -p:SelfContained=true -p:PublishSingleFile=true
    fi
done

# Also build Utilities if it exists
if [ -f "Utilities/Utilities.csproj" ]; then
    echo "Building Utilities..."
    dotnet build Utilities/Utilities.csproj -c Release -p:RuntimeIdentifier="$RID" -p:SelfContained=true -p:PublishSingleFile=true
fi

echo "Build complete for $RID"

