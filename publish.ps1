&dotnet publish ProjectGenerator/ProjectGenerator.csproj --configuration Release --runtime win-x64 --output publish  /p:PublishSingleFile=true

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/ProjectGeneratorPluginProjects/ProjectGeneratorPluginProjects.csproj --configuration Release --runtime win-x64 --output "publish\plugins\ProjectGeneratorPluginProjects"
