@echo off
dotnet build
dotnet publish -c Release -f net45 ../NetCache.BuildTask
dotnet publish -c Release -f netcoreapp2.1 ../NetCache.BuildTask
dotnet build -c Release
pause
