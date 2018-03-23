@echo off
setlocal
set Configuration=Release
::set ApplicationVersion=0.2.46.0
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin
msbuild "..\KsWare.AppVeyorClient\KsWare.AppVeyorClient.csproj" /verbosity:normal /target:publish
::msdeploy.exe -verb:sync -source:dirPath="..\bin\Release" -dest:dirPath="..\deploy" -enableRule:DoNotDeleteRule
pause
endlocal