# Why BuildOnce?

Continuous delivery norms and the Visual Studio toolset encourage developers to build our bits and configuration together.
This typically leads to re-executing the build, unit test and packages steps for each environment.
This ineficiency can have reprocusions to your build infrastructure and create a bottleneck in your delivery pipeline.  

BuildOnce allows you to transform all your configuration files into the required environment targets as part of your build!

This enables you to then combine the bits and config as needed for each environment without the need to rebuild and re-run unit tests and thus make you a CI rock :star:.

## Installation

The [library] is available on nuget 

`install-package BuildOnce`

## Usage

Once the package is installed in your project you can enable the DeployOnce config transforms by:

* Passing the `/p:DeployOnceEnabled=True` to msbuild on the command line
* Set the property in your project file `<DeployOnceEnabled>False</DeployOnceEnabled>`

> The property can even be set conditionally under the `Release` configuration which fits most use cases

### Custom Output Path

By default the transformed config files are placed under the `obj/config` folder with a subfolder for each transform. 
Relative paths are maintained to ensure you can simply copy and paste the config over your code/website.
You can override this behavior by:

* Passing the `/p:DeployOnceOutputPath=../mycustompath/..` to msbuild on the command line
* Set the property in your project file `<DeployOnceOutputPath>../mycustompath/..</DeployOnceOutputPath>`

### Disabling Cleanup

By default whenever you clean the solution BuildOnce will remove the folder identified in `DeployOnceOutputPath`.
You can override this behavior by:

* Passing the `/p:BuildOnceRemoveOutputOnClean=AnythingButTrue` to msbuild on the command line
* Set the property in your project file `<BuildOnceRemoveOutputOnClean>AnythingButTrue</BuildOnceRemoveOutputOnClean>`

### Non-standard nuget packages folders

Typically your packages folder is nested at the same level as your solution. 
If for some reason you have a different setup you can handle it by:

* Passing the `/p:NuGetPackageDirectory=../PathToPackages/Folder` to msbuild on the command line
* Set the property in your project file `<NuGetPackageDirectory>NuGetPackageDirectory</NuGetPackageDirectory>`

## Known Issues
1) Visual Studio tends to hold on to the BuildOnce.dll that serves the custom msbuild task. 
When you remove/upgrade the package you may experience a delay and a message that Nuget was unable to completely uninstall the package with a prompt to restart.
Make sure you save the changes to your project file! When VS restarts it will delete the finish deleting the unused package folder.

2) Due to msbuild discovery process DeployOnce cannot support multiple versions within a solution. 
If they are out of sync you will receive an error telling you to correct the issue.

## Version
* BuildOnce 0.1.0

## License
MIT

[library]:https://www.nuget.org/packages/BuildOnce/
