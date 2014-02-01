Dreamhost-Dynamic-DNS-Updater
=============================

A small Windows Service that will update Dreamhost's DNS entries to point to your home machine.

## Building ##

The project is built using C# in Visual Studio 2013. You can also build the project using `msbuild` via the `build.cmd` script in the root of the project.

Simple logging is doing via NLog to `Logging.log` where the executable is run, but can be configured via *NLog.config*.

## Installation ##

After the project is built, add your DreamHost API key and host information into the app.config file.

Run `install.cmd` from the root of the directory from the command line to install the Windows Service.

## Notes ##

Currently, there is no real exception handling in place and the log configuration is the only real output of the service.

If you find this application useful, drop me an email and let me know since there aren't any statistics from GitHub's downloads. If you find a bug, pull requests are the preferred method of assistance!