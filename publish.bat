@echo off
set Runtimes=win-x64

for %%r in (%Runtimes%) do (
    echo Publishing for %%r...
    dotnet test %%r.Tests/%%r.Tests.csproj
    dotnet publish -c Release -r %%r --self-contained true /p:PublishSingleFile=true
)

echo All builds completed!
pause
