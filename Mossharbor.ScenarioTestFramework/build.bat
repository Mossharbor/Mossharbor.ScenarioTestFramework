REM msbuild /t:pack /p:NuspecFile=Package.nuspec Mossharbor.ScenarioTestFramework.csproj 
dotnet build Mossharbor.ScenarioTestFramework.csproj
dotnet pack -p:NuspecFile=.\Package.nuspec
