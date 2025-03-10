@echo off
set Runtimes=win-x64 win-x86 win-arm64

for %%r in (%Runtimes%) do (
    echo Publishing for %%r...
    dotnet publish -c Release -r %%r --self-contained true /p:PublishSingleFile=true
)

echo All builds completed!
pause