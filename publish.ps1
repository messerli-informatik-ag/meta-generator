&dotnet publish ProjectGenerator/ProjectGenerator.csproj --configuration Debug --runtime win-x64 --output publish

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/NativeProjects/NativeProjects.csproj --configuration Debug --runtime win-x64 --output "publish\plugins\NativeProjects"

&dotnet publish Plugins/OneCoreProjects/OneCoreProjects.csproj --configuration Debug --runtime win-x64 --output "publish\plugins\OneCoreProjects"

&dotnet publish Plugins/ProjectGeneratorPluginProjects/ProjectGeneratorPluginProjects.csproj --configuration Debug --runtime win-x64 --output "publish\plugins\ProjectGeneratorPluginProjects"
