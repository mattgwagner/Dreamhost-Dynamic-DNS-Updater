Dreamhost-Dynamic-DNS-Updater
=============================

A small Windows Service that will update Dreamhost's DNS entries to point to your home machine.

## Building ##

[![Build status](https://ci.appveyor.com/api/projects/status/rb58kg4hcaqlx3qq?svg=true)](https://ci.appveyor.com/project/mattgwagner/dreamhost-dynamic-dns-updater)

The project is built using C# in Visual Studio 2013. You can also build the project using `msbuild` via the `build.cmd` script in the root of the project.

Simple logging is doing via NLog to `Logging.log` where the executable is run, but can be configured via *NLog.config*.

## Installation ##
First, in the Dreamhost Panel, create the custom DNS entry for the domain you want to have resolve to your dynamic IP. This is found under Domains > Manage Domains. Then select the DNS link under the domain you want (eg example.com). Fill out the "Add a custom DNS record to..." section and submit.
TIP: It should be an A record, and the value should be a valid IP address `xxx.xxx.xxx.xxx`.
TIP: To ensure the service works, set the initial IP to something *other* than your actual IP. This way you can confirm the service is working by seeing that the fake IP is updated with your actual IP.

After the project is built, add your DreamHost API key and host information into the `Service/App.config` file. The API key is located at [https://panel.dreamhost.com/?tree=home.api]. The API key used needs to have access to "All dns functions" as a minimum.

Run `install.cmd` from the root of the directory from the command line to install the Windows Service.

## Uninstalling ##

To remove the service once it has been installed or if the API key or hostname changes, `run sc.exe delete DreamHostDynamicDNSUpdater` from the command prompt with administrator privileges.

## Notes ##

Currently, there is no real exception handling in place and the log configuration is the only real output of the service.

If you find this application useful, drop me an email and let me know since there aren't any statistics from GitHub's downloads. If you find a bug, pull requests are the preferred method of assistance!
