
# Installation Guide

[Back to README](#README.md)

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
- [Microsoft *Visual Studio* 2022](https://visualstudio.microsoft.com/vs/) (VisualStudio.17.Release/17.4.4+33213.308 Enterprise, Microsoft .NET Framework Version 4.8.04084)
- [Rhinoceros 3D](https://www.rhino3d.com/)(includes [Grasshopper](https://www.grasshopper3d.com/)) (Version 7 SR31, 7.31.23166.15001, 2023-06-15, Educational Lab License)
- [SciChart](https://www.scichart.com/) (Version 7.0.2.27161)
*Optional:*
- [Python](https://www.python.org/downloads/)                             (Version 3.8 and above)
- [Bokeh](https://bokeh.org/)                                             (Version 3.1.1, see [First Steps](https://docs.bokeh.org/en/latest/docs/first_steps.html#first-steps))

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


<!-- ###################################################################### -->
### Installation of Pre-Compiled Binaries

1. Donwload and extract latest binary release from the repository page: *<todo>*
2. Copy the extracted folder `VisuAlFroG`.
3. Change to the directory `C:\Users\<username>\AppData\Roaming\Grasshopper\Libraries\` (replace <username> with the username of your local computer account).
4. Paste the previously copied folder.
5. Open `Rhino` and start `Grasshopper`.
6. There will be a new `Visual Analysis` tab providing the `VisuAlFroG` component. 


<!-- ###################################################################### -->
### Test Example

The successful installation can be tested by opening the provided Grasshopper example: `..\example\VisualizeDataExample.gh`.


<!-- ###################################################################### -->