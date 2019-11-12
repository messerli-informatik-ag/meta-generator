&dotnet publish ProjectGenerator/ProjectGenerator.csproj --configuration Release --runtime win-x64 --output publish /p:PublishSingleFile=true

New-Item -Path "publish" -Name "plugins" -ItemType "directory" -Force

#Plugins

&dotnet publish Plugins/NativeProjects/NativeProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\NativeProjects\bin\Release\netstandard2.1\win-x64\publish\NativeProjects.dll" -Destination "publish\plugins"

&dotnet publish Plugins/OneCoreProjects/OneCoreProjects.csproj --configuration Release --runtime win-x64
Copy-Item "Plugins\OneCoreProjects\bin\Release\netstandard2.1\win-x64\publish\OneCoreProjects.dll" -Destination "publish\plugins"