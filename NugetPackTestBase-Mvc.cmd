@Echo On
if .%1 == . echo No API key given: will pack not push

if .%msbuild% == . set msbuild="B:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"

pushd %~p0
cd TestBase-Mvc
%msbuild%
NuGet.exe Pack -Symbols
if errorlevel 1 goto Error
popd

if .%1 == . got :eof
NuGet.exe setApiKey %1
NuGet.exe push TestBase\TestBase.%2.nupkg
NuGet.exe push TestBase-Mvc\TestBase-Mvc.%2.nupkg

goto :eof
:Error
echo.
echo Error whilst packing.
popd
:Usage
Echo.
Echo PackAndPush ^<NugetApiKey^> ^<VersionToPush^>