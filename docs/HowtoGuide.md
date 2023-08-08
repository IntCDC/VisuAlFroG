
# VisFroG How-to Guide

<!-- TOC -->

## Contents

- [Installation and Setup](#installation-and-setup)
    - [Prerequisites](#prerequisites)
    - [Building and Installation](#building-and-installation)
    - [Test Example](#test-example)

<!-- /TOC -->
-----


<!-- ###################################################################### -->
## Installation and Setup


<!-- ###################################################################### -->
### Prerequisites

Building and running `VisFroG` requires following software being installed beforehand:
- [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (VisualStudio.17.Release/17.4.4+33213.308 Enterprise, Microsoft .NET Framework Version 4.8.04084)
- [Rhinoceros 3D](https://www.rhino3d.com/) (Version SR28, 7.28.23058.3001, 2023-02-27, Educational Lab License)
- [Grasshopper](https://www.grasshopper3d.com/) (Version Wednesday, 01 February 2023 13:00, Build 1.0 007, part of Rhino3D)
- [Python](https://www.python.org/downloads/) (Version 3.8 and above )
- [Bokeh](https://bokeh.org/) (Version 3.1.1, see [First Steps](https://docs.bokeh.org/en/latest/docs/first_steps.html#first-steps))
- [SciChart](https://www.scichart.com/) (Version 7.0.2.27161)
(The appended versions show the last tested versions)

> **Note**
> - Upon account creation, the [full version of Rhinoceros 3D](https://www.rhino3d.com/download/rhino-for-windows/evaluation) can be used for free for 90 days.
> - Upon account creation, the free [trial version of SciChart](https://www.scichart.com/getting-started/scichart-wpf/) can be used for 30 days


<!-- ###################################################################### -->
### Building and Installation

- Open the `VisFroG.sln` file with Visual Studio 2022.
- Open the file  `...\SciChartInterface\SciChartRuntimeLicenseKey.cs` and replace the comment ***"INSERT YOUR SCICHART LICENCE KEY HERE"*** with your SciChart runtime license key.
- Make git skip your local changes in `/SciChartInterface/SciChartRuntimeLicenseKey.cs`:
```console
$ git update-index --skip-worktree SciChartInterface/SciChartRuntimeLicenseKey.cs
```
- From the menu `Build` choose `Build Solution`. **Note**: Mkae sure that `Interface/GrasshopperComponent` is the *Startup Project*.
  - All *external dependencies* are automatically downloaded via NuGet when Visual Studio solution is built.
- Move all files from the build output folder `\bin` into a new Grasshopper plugin folder, e.g.: `C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFroG`.
- Open `Rhino` and start `Grasshopper`.
- In the `Visual Analysis` tab drag and drop the `VisFroG` component onto the grasshopper canvas.


<!-- ###################################################################### -->
### Test Example

For testing the successful installation, open the provided example Grasshopper definition `..\example\VisualizeDataExample.gh`.


<!-- ###################################################################### -->