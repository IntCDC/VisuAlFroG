# Developer Guide

[Back to README](../README.md)

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

- Open Visual Studio and copy the file `../Visualizations/WPFInterface/template/CustomWPFVisualization.cs.template` to `../Visualizations/Visualizations/<NAME OF YOUR VISUALIZATION>.cs`
- Rename the class `CustomWPFVisualization` to `<NAME OF YOUR VISUALIZATION>`
- Change the name property of your new visualization class:
```C#
    public override string Name { get { return "<NAME OF YOUR VISUALIZATION>"; } }
```
- Open the file `../Visualizations/ContentManager.cs` and add register your new visualization in the *Visualization.ContentManager.Initialize* method below the already registered contents via:
```C#
    register_content(typeof(<NAME OF YOUR VISUALIZATION>));
```
- The default content element can be accessed via the `Content`propoerty. The default content element `Canvas` is defined in the template of the inherited class `AbstractWPFVisualization` and can be changed to any `UIElement`.
        

<!--
### d3

<TODO>

### Bokeh (Python)

<TODO>
-->


<!-- ###################################################################### -->
## Additional Information

### Logging debug messages
In order to show messages in the `LogConsole` messages have to be provided via the `Log` class the following way:
```C#
Log.Default.Msg(Log.Level.Error, "YOUR DEBUG MESSAG");
```
Enable/disable *DEBUG* message by changing the following line in *Frontend.Application.MainWindow.Initilaize():138*:
```C#
    Log.Default.DisableDebug = true;
```

### Predefined Colors
GUI element colors are defined for each available color theme in `.../Core/resources/color-themes/<theme>.xaml`. 
Colors defined in the themes can be assigned to WPF elements in two ways:
```C#
    <Frameworkelement>.SetResourceReference(<Frameworkelement>.<PropertyName>, "Brush_<Name>");
    # OR
    <Setter>.Value = new DynamicResourceExtension("Brush_<Name>");
```

### Startup Configuration
The default window configuration on startup is defined in *Core.GUI.WindowManager.CreateDefault()* and called in *Frontend.Application.MainWindow.create():221*:
```C#
    _winmanager.CreateDefault();
```


-----
<!-- ###################################################################### -->
## Code Style

Reccomendations for code formatting are stored in `.editorconfig` and are automatically checked via [dotnet-format](https://github.com/dotnet/format) in *Visual Studio*. 
In *Visual Studio*, look at the info messages in the *Error List* after building *VisuAlFroG* and adjust code according to recommendations.

### Naming Conventions

#### Public Properties
- Start with `_` and then camel case | Example: `_PublicProperty`
#### Public Functions
- Start with upper case letter, then camel case | Example:`PublicFunction`

#### Protected Properties/Variables
- Start with lower case letter, then only lower case letters, separate words with `_` | Example:`protected_variable`
#### Protected Functions:
- Start with lower case letter, then only lower case letters, separate words with `_` | Example:`protected_function`

#### Private Variables
- Start with `_` and then only lower case letters, separate words with `_` | Example:`_private_function`
#### Private Functions
- Start with lower case letter, then only lower case letters, separate words with `_` | Example:`private_function`


-----
<!-- ###################################################################### -->
## Resource Files

Add new resource files to the existing `Core/resources` or `Visualizations/Libraries/.../template` folder and create new subfolder if desired. 
Add new resource files in *Visual Studio* via `Add` -> `Existing Items...`. 
Change the `Build Action` property of the newly added resource file to `Resource`.
Add new enum to `Location` in *Core.Utilities.ResourcePaths* and add option for subfolder of new location in *Core.Utilities.ResourcePaths.GetResourcePath()*.


<!-- ###################################################################### -->
