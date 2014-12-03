@echo Off

set EnableNuGetPackageRestore=true
set targets=%1

if "%targets%" == "" (
	set targets=Build
)

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild "Dreamhost_DNS_Updater.sln" /v:minimal /maxcpucount /target:%targets%

pause