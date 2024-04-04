# Developer Guide

[Back to README](README.md)

<!-- TOC -->

## Contents

- [Custom Visualization](#custom-visualization)
- [Code Style](#code-style)
- [Resource Files](#resource-files)

<!-- /TOC -->
-----


<!-- ###################################################################### -->
## Custom Visualization

### C#/WPF

*TODO*...



### d3

*WIP*...

### Bokeh (Python)

*WIP*...


<!-- ###################################################################### -->
## Code Style

Reccomendations for code formatting are stored in `.editorconfig` and are automatically checked via [dotnet-format](https://github.com/dotnet/format) in *Visual Studio*. 
In *Visual Studio*, look at the info messages in the *Error List* after building *VisuAlFroG* and adjust code according to recommendations.


<!-- ###################################################################### -->
## Resource Files

Add new resource files to the existing `Core\resources` or `Visualizations\resources` folder and create new subfolder if desired. 
Add new resource files in *Visual Studio* via `Add` `Existing Items...`. 
Change the `Build Action` property of the newly added resource file to `Resource`.
Add new enum to `Location` in `Core.Utilities.WorkingDirectory` and add option for subfolder of new location in `WorkingDirectory.ResourcePath()`.


<!-- ###################################################################### -->
