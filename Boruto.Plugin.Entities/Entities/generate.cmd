@echo off
call pac modelbuilder build --settingsTemplateFile builderSettings.json --outdirectory .
call ..\..\Bor\Bin\Debug\Bor.exe model