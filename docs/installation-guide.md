
# Installation Guide

[Back to README](../README.md)

<!-- TOC -->

## Contents

- [Building from Source](#building-from-source)
    - [Prerequisites](#prerequisites)
    - [Build Instructions](#build-instructions)
- [Installation of Pre-Compiled Binaries](#installation-of-pre-compiled-binaries)
  - [Test Example](#test-example)

<!-- /TOC -->
-----


<!-- ###################################################################### -->
## Building from Source

<!-- ###################################################################### -->
### Prerequisites

The following software is required to be installed beforehand:
- [Microsoft *Visual Studio* 2022](https://visualstudio.microsoft.com/vs/)  |  VisualStudio.17.Release/17.9.0+34607.119 Enterprise, Microsoft .NET Framework Version 4.8.09037)
- [Rhinoceros 3D](https://www.rhino3d.com/)(includes [Grasshopper](https://www.grasshopper3d.com/))  |  Version 8 SR5, 8.5.24072.13001, 2024-03-12, Educational Lab License)
- [SciChart](https://www.scichart.com/)  |  Version 8.3.0.28019
- [Python](https://www.python.org/downloads/)  |  Version 3.11.5 (3.9 and above)
- [Bokeh](https://bokeh.org/)  |  Version 3.4.0, see [First Steps](https://docs.bokeh.org/en/latest/docs/first_steps.html#first-steps))

(The appended version numbers show the last tested versions)

> **Note**
> - Upon account creation, the [full version of Rhinoceros 3D](https://www.rhino3d.com/download/rhino-for-windows/evaluation) can be used for free for 90 days.
> - Upon account creation, the free [trial version of SciChart](https://www.scichart.com/getting-started/scichart-wpf/) can be used for 30 days

> **Warn**
> - The .NET SDK version of Rhino and Grasshopper should not be larger than the version of the desktop application.


<!-- ###################################################################### -->
### Build Instructions

1. Open the `VisuAlFroG.sln` file with *Visual Studio* 2022.
2. Open the file  `SciChartInterface\SciChartRuntimeLicenseKey.cs` and replace *INSERT YOUR SCICHART LICENCE KEY HERE* with your SciChart runtime license key.
   *Optional:* Make *git* skip your local changes in `SciChartInterface\SciChartRuntimeLicenseKey.cs`, e.g. via *git Bash*:
```console
      $ git update-index --skip-worktree SciChartInterface/SciChartRuntimeLicenseKey.cs
```
3. From the menu `Build` choose `Build Solution`. **Note**: Make sure that `Interface\GrasshopperComponent` is the *Startup Project*.
   All *external dependencies* are automatically downloaded via NuGet when the *Visual Studio* solution is built.

*Developers* should continue with **9.**

4. Copy all files from the build output folder `\bin`.
5. Change to the directory `C:\Users\<username>\AppData\Roaming\Grasshopper\Libraries\` (replace <username> with the username of your local computer account).
6. Create a new directory called `VisuAlFroG`, for example.
7. Paste the previously copied folder.
8. Continue with step **5.** of [Installation of Pre-Compiled Binaries](#installation-of-pre-compiled-binaries).
  
--

9. Set `GrasshopperComponent` as StartUp project and select the Configuration `Debug` `x64` before starting the debugging.
10. After `Rhino` opened automaticall, start `Grasshopper`.
11. Change to the build output folder `\bin` and drag and drop the `VisuAlFroG.gha` component from the File Explorer onto the grasshopper canvas.
12. There will be a new `Visual Analysis` tab providing the `VisuAlFroG` component. 


-----
<!-- ###################################################################### -->
### Installation of Pre-Compiled Binaries

1. Donwload and extract latest binary release from the repository page: *<todo>*
2. Copy the extracted folder `VisuAlFroG`.
3. Change to the directory `C:\Users\<username>\AppData\Roaming\Grasshopper\Libraries\` (replace <username> with the username of your local computer account).
4. Paste the previously copied folder.
5. Open `Rhino` and start `Grasshopper`.
6. There will be a new `Visual Analysis` tab providing the `VisuAlFroG` component. 


<!-- ###################################################################### -->
#### Test Example

The successful installation can be tested by opening the provided Grasshopper example: `..\example\VisualizeDataExample.gh`.


<!-- ###################################################################### -->
### Known Issues

- Rhino ** Version 8** uses .NET Core by (default)[https://www.rhino3d.com/en/docs/guides/netcore/]. 
**VisuAlFroG** requires the legacy .NET Framework, instead.
Therefore, Rhino need to be changed to always use .NET Framework.
In the command line type `SetDotNetRuntime` and then enter `Runtime=NETFramework` and confirm.
Restart Rhino to take changes effect.

Following errors indicate the problem mentioned above:
- When debugging VisuAlFroG in Visual Studio the following error occurs: *A fatal error has occurred and debugging needs to be terminated. The debugger was configured to use the Desktop CLR (.NETFramework) Managed debugger, but the target process loaded the CoreCLR (.Net Core) runtime. To debug this project, configure it to use the 'Managed (CoreCLR)' debugger.*
- When VisuAlFroG is placed in Grasshopper library folder and then Grasshopper is started the following errors occur: 
  - Object: VisuAlFroG (level 1) Exception has been thrown by the target of an invocation. TargetInvocationException
  - Object: VisuAlFroG (level 2) Could not load file or assembly 'VisuAlFroG_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. FileNotFoundException


<!-- ###################################################################### -->


