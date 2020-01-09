&dotnet publish ProjectGenerator/ProjectGenerator.csproj --configuration Release --runtime win-x64 --output publish /p:PublishSingleFile=true

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/NativeProjects/NativeProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\NativeProjects\bin\Release\netstandard2.1\win-x64\publish\NativeProjects.dll" -Destination "publish\plugins"

&dotnet publish Plugins/OneCoreProjects/OneCoreProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\OneCoreProjects\bin\Release\netstandard2.1\win-x64\publish\OneCoreProjects.dll" -Destination "publish\plugins"

&dotnet publish Plugins/ProjectGeneratorPluginProjects/ProjectGeneratorPluginProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\ProjectGeneratorPluginProjects\bin\Release\netstandard2.1\win-x64\publish\ProjectGeneratorPluginProjects.dll" -Destination "publish\plugins"

&dotnet publish Plugins/FluffyProjects/FluffyProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\FluffyProjects\bin\Release\netstandard2.1\win-x64\publish\FluffyProjects.dll" -Destination "publish\plugins"
