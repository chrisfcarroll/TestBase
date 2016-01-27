@Echo Off
if .%1 == . Goto Usage
pushd %~p0

NuGet.exe setApiKey %1

cd TestBase
NuGet.exe Pack TestBase.csproj  -Symbols
if errorlevel 1 goto Error

cd ..\TestBase-Mvc
NuGet.exe Pack TestBase-Mvc.csproj -IncludeReferencedProjects -Symbols
if errorlevel 1 goto Error

popd
NuGet.exe push TestBase\TestBase.%2.nupkg
NuGet.exe push TestBase-Mvc\TestBase-Mvc.%2.nupkg

goto :eof
:Error
echo.
echo Aborted due to error whilst packing.
popd
:Usage
Echo.
Echo PackAndPush ^<NugetApiKey^> ^<VersionToPush^>