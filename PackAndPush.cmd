if .%1 == . Goto Usage
pushd %~d0

.nuget\NuGet.exe setApiKey %1

cd TestBase
..\.nuget\NuGet.exe Pack TestBase.csproj
nuget push TestBase.%2.nupkg

cd TestBase-Mvc
..\.nuget\NuGet.exe Pack TestBase-Mvc.csproj
nuget push TestBase-Mvc.%2.nupkg -IncludeReferencedProjects
popd

goto :eof

:Usage
Echo PackAndPush ^<NugetApiKey^> ^<VersionToPush^>