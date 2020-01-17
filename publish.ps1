&dotnet publish MetaGenerator/MetaGenerator.csproj --configuration Release --runtime win-x64 --output publish  /p:PublishSingleFile=true

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/MetaGeneratorProjectPlugin/MetaGeneratorProjectPlugin.csproj --configuration Release --runtime win-x64 --output "publish\plugins\MetaGeneratorProjectPlugin"
