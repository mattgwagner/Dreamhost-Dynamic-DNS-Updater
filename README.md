Dreamhost-Dynamic-DNS-Updater
=============================

A small Windows Service that will update Dreamhost's DNS entries to point to your home machine.

## Building ##

The project is built using C# in Visual Studio 2012. You can also build the project using `msbuild` via the `build.cmd` script in the root of the project. You need to have NuGet Pacakge Restore enabled to allow NuGet to download the supporting libraries (TopShelf and Quartz.net in particular). 

Simple logging is doing via NLog to `Logging.log` where the executable is run, but can be configured via *NLog.config*.

## Installation ##

After the project is built, add your DreamHost API key and host information into the app.config file.

Run `DHDns.Service.exe install` from the command line to install the Windows Service.

## Notes ##

Currently, there is no real exception handling in place and the log configuration is the only real output of the service.