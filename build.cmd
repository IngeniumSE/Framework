@echo off

set target=%1
if "%target%" == "" set target=Default

cd build\apps\build

dotnet run --target %target%