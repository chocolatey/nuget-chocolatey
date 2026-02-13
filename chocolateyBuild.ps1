Push-Location $PSScriptRoot
.\build.cmd
mkdir nuget/Chocolatey-NuGet.Core/lib/net4
Copy-Item .\src\Core\bin\Release\* .\nuget\Chocolatey-NuGet.Core\lib\net4\
Push-Location nuget/Chocolatey-NuGet.Core
.\strongname.cmd
Pop-Location
Pop-Location