Dreamhost-Dynamic-DNS-Updater
=============================

A small Windows Service that will update Dreamhost's DNS entries to point to your home machine.

== Installation ==

Build the solution either via Visual Studio or via MSBuild. This will require allowing NuGet package restore.

Add your DreamHost API key and host information into the app.config file.

Run `DHDns.Service.exe install` from the command line to install the Windows Service.