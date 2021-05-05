## NuGet Enhanced for Chocolatey

This repository contains a forked version of NuGet, which has been enhanced to work with Chocolatey, the Package Manager for Windows.

This includes, for example, modifications to the nuspec file to include additional properties that are specific to how Chocolatey works.

### Building

To build the project, run:

```
build.cmd
```

This will generate release binaries for NuGet.Core in the following folder:

```
src\Core\bin\Release
```

These will include:

* Microsoft.Web.XmlTransform.dll
* NuGet.Core.dll
* NuGet.Core.pdb

### Packaging

To create a NuGet package which can then be consumed within the chocolatey/choco project, follow these steps

* Build the solution
* Copy all files from `src\Core\bin\Release` into the `nuget\Chocolatey-NuGet.Core\lib\net4` folder.  **NOTE:** If this folder doesn't exist, create it.
* Navigate to the `nuget` folder in a PowerShell session
* Run the following command `.\strongname.cmd`
* This will ensure that the NuGet.Core.dll is strong name signed, which is a requirement for all assemblies referenced by the Chocolatey project.
* Copy the files from the `output` folder into the `nuget\Chocolatey-NuGet.Core\lib\net4` folder
* Delete the `Microsoft.Web.XmlTransform.dll` file from the `nuget\Chocolatey-NuGet.Core\lib\net4` folder
* Run the following command to create a NuGet package `nuget pack .\Chocolatey-NuGet.Core.nuspec -Version 2.11.0.<insert_date_here>` where insert_date_here is todays date, in the format yyyymmdd, i.e. 20210105 which would be 5th January 2021
* Push this package to internal NuGet repository
* Run the following command in chocolatey/choco repository to bring this package into the lib folder there `nuget install Chocolatey-NuGet.Core -Source <URL_to_repository>`, and then manually update the csproj files to target new location in lib folder.
* Delete previous Chocolatey-NuGet.Core folder from lib folder
* Build chocolatey/choco and ensure everything is working as expected