@echo off
pushd %~dp0
md nuget
cd nuget
..\tools\nuget.exe pack ..\Serialization\Serialization.nuspec
popd
