#!/usr/bin/env bash
set -euo pipefail

project="X4SectorCreator/X4SectorCreator.csproj"
tests="X4SectorCreator.Tests/X4SectorCreator.Tests.csproj"

dotnet test "$tests"
dotnet build "$project" -c Release -f net9.0 -r linux-x64
dotnet build "$project" -c Release -f net9.0-windows -r win-x64

dotnet publish "$project" -c Release -r linux-x64 -f net9.0 --self-contained true '/p:PublishSingleFile=true'
dotnet publish "$project" -c Release -r win-x64 -f net9.0-windows --self-contained true '/p:PublishSingleFile=true'
