&dotnet publish MetaGenerator/MetaGenerator.csproj --configuration Release --runtime win-x64 --output publish  /p:PublishSingleFile=true

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/MetaGeneratorProjectPlugin/MetaGeneratorProjectPlugin.csproj --configuration Release --runtime win-x64 --output "publish\plugins\MetaGeneratorProjectPlugin"

&dotnet publish Plugins/NativeProjectsPlugin/NativeProjectsPlugin.csproj --configuration Debug --runtime win-x64 --output "publish\plugins\NativeProjectsPlugin"

&dotnet publish Plugins/MesserliOneRepositoryPlugin/MesserliOneRepositoryPlugin.csproj --configuration Release --runtime win-x64 --output "publish\plugins\MesserliOneRepositoryPlugin"

&dotnet publish Plugins/ManagedWrapperProjectsPlugin/ManagedWrapperProjectsPlugin.csproj --configuration Release --runtime win-x64 --output "publish\plugins\ManagedWrapperProjectsPlugin"
