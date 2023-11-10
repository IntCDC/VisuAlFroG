 ### <img src="https://github.com/IntCDC/VisuAlFroG/assets/30432154/e8baed3d-1409-4be4-ada0-9702a55a06d7" width="50">  VisuAlFroG
 # Visual Analytics Framework for Grasshopper

<!-- BADGES ------------------------------------------------------------------>

[![Build](https://github.com/IntCDC/VisuAlFroG/actions/workflows/build.yml/badge.svg)](https://github.com/IntCDC/VisuAlFroG/actions/workflows/build.yml)
[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)
[![GitHub License](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.tik.uni-stuttgart.de/IntCDC-RDM-RSE/VisuAlFroG/blob/main/LICENSE)
[![Windows](https://badgen.net/badge/icon/windows?icon=windows&label)](https://microsoft.com/windows/)
[![DaRUS DOI](https://img.shields.io/badge/DaRUS%20DOI-unpublished-orange?style=flat&logo=data:image/png;base64,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)](https://darus.uni-stuttgart.de/dataverse/exc-intcdc)
<!-- if published replace 'unpublished' with '10.18419/darus--XXXX' and adapt link at the end -->

<!---------------------------------------------------------------------------->

## Description

*VisuAlFroG* is a framework that provides various visualizations to support visual analytics for the algorithmic modelling software [Grasshopper](https://www.grasshopper3d.com/). 
Grasshopper for its part is a plug-in of the commercial 3D computer graphics and computer-aided design application software [Rhinoceros](https://www.rhino3d.com/).
The basic architecture of *VisuAlFroG* consists of a core framework providing general functionality and a module-based extension mechanism that easily allows to integrate new visualizations.

Currently, it is planned to offer the integration of visualizations that can be based on:
- [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/?view=netdesktop-7.0)
- [SciChart](https://www.scichart.com/)
- [d3.js](https://d3js.org/)
- [Bokeh](https://bokeh.org/)


## Documentation

- [How-to Guide](docs/HowtoGuide.md)
- [Developer Guide](docs/DeveloperGuide.md)


## License

See the [license](LICENSE) file.

For *external dependencies* different licenses apply:
- RhinoCommon (.NET plugin SDK for Rhino): [MIT](https://developer.rhino3d.com/)
- Grasshopper .NET plugin: [MIT](https://developer.rhino3d.com/)
- d3: [ISC License](https://github.com/d3/d3/blob/main/LICENSE) (functionally equivalent to MIT license)
- Bokeh: [BSD-3 Clause](https://github.com/bokeh/bokeh/blob/main/LICENSE.txt)
- SciChart: [EULA](https://www.scichart.com/scichart-eula/)
- pythonnet: [MIT](https://www.nuget.org/packages/pythonnet/3.0.1/license)
- OWIN: [Apache 2.0](https://github.com/owin-contrib/owin-hosting/blob/master/LICENSE.txt)
- OwinSelfHost: [Microsoft Software License Terms](https://www.nuget.org/packages/Microsoft.AspNet.WebApi.OwinSelfHost/5.2.9/license)
- EntityFramework: [Apache 2.0](https://licenses.nuget.org/Apache-2.0)
- other transitive dependencies are published under [MIT](https://licenses.nuget.org/MIT), [Apache 2.0](https://licenses.nuget.org/Apache-2.0), or the [Microsoft Software License Terms](https://dotnet.microsoft.com/en-us/dotnet_library_license.htm)


## Citation

Choose from GitHub citation prompt *Cite this repository* on the right.
The [citation](CITATION.cff) is based on the [Citation Cite Format](https://citation-file-format.github.io/) (CFF) and has been generated with [CFFINIT](https://citation-file-format.github.io/cff-initializer-javascript/#/).


## Acknowledgements

- This project is supported by the Deutsche Forschungsgemeinschaft (DFG, German Research Foundation) under Germany's Excellence Strategy – EXC 2120/1 – 390831618
- Special thanks to [SciChart](https://www.scichart.com/contact-us/) for permission to distribute licensed SciChart as part of the compiled form of this software.
- Thanks to the [Bokeh Development Team](https://github.com/bokeh/bokeh).

----

> **Note**
> Internal information and documentation can be found on the [Confluence project page](https://wisman.izus.uni-stuttgart.de/display/IntCDCMan/Visual+Analytics+Framework+for+Grasshopper).

