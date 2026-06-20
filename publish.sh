#!/usr/bin/env bash
dotnet test X4SectorCreator.Tests/X4SectorCreator.Tests.csproj
dotnet publish X4SectorCreator/X4SectorCreator.csproj -c Release -r linux-x64 -f net9.0 --self-contained true '/p:PublishSingleFile=true'
dotnet publish X4SectorCreator/X4SectorCreator.csproj -c Release -r win-x64 -f net9.0-windows --self-contained true '/p:PublishSingleFile=true'
