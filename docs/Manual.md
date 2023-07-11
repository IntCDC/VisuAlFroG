
# VisFroG Manual

<!-- TOC -->

## Contents

- [Installation and Setup](#installation-and-setup)
    - [Prerequisites](#prerequisites)
    - [Build](#build)

<!-- /TOC -->

<!-- ###################################################################### -->
-----
## Installation and Setup

### Prerequisites

Building and running `VisFroG` requires following software:
- [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (VisualStudio.17.Release/17.4.4+33213.308 Enterprise, Microsoft .NET Framework Version 4.8.04084)
- [Rhinoceros 3D](https://www.rhino3d.com/) (Version SR28, 7.28.23058.3001, 2023-02-27, Educational Lab License)
    - You can try the full version for 90 days for free.
- [Grasshopper](https://www.grasshopper3d.com/) (Version Wednesday, 01 February 2023 13:00, Build 1.0 007, part of Rhino3D)
- [Python](https://www.python.org/downloads/) (Version 3.8 and above )
- [Bokeh](https://bokeh.org/) (Version 3.1.1, see [First Steps](https://docs.bokeh.org/en/latest/docs/first_steps.html#first-steps))
- [SciChart](https://www.scichart.com/) (Version 7.0.2.27161)
    - Currently a 30 day trail license key is provided (valid until August 4, 2023 )
    - *TODO: Included permanent educational license?*
(The appended versions show the tested configurations)

### Building and Installation

- Open the `VisFroG.sln` file with Visual Studio 2022 and from the menu `Build`choose `Build Solution`. The *Startup Project* should be `Interface/GrasshopperComponent`.
- Move all files from the build output folder `\bin` into a new Grasshopper plugin folder: `C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFroG`
- Open `Rhino` and start `Grasshopper`
- In the `Visual Analysis` tab drag and drop the `VisFroG` component onto the grasshopper canvas.

### Inital Example

For testing the successful installation, you ...



<!-- ###################################################################### -->