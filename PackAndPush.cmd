if .%1 == . Goto Usage
pushd %~p0

.nuget\NuGet.exe setApiKey %1

cd TestBase
..\.nuget\NuGet.exe Pack TestBase.csproj  -Symbols
if errorlevel 1 goto Error

cd ..\TestBase-Mvc
..\.nuget\NuGet.exe Pack TestBase-Mvc.csproj -IncludeReferencedProjects 
if errorlevel 1 goto Error

popd
.nuget\NuGet.exe push TestBase\TestBase.%2.nupkg
.nuget\NuGet.exe push TestBase-Mvc\TestBase-Mvc.%2.nupkg

goto :eof
:Error
echo Aborted due to error whilst packing.
popd
:Usage
Echo PackAndPush ^<NugetApiKey^> ^<VersionToPush^>