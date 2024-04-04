using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;



/*
 * VisuAlFroG Grasshopper Component Info
 * 
 */
namespace Interface
{
    namespace GrasshopperComponent
    {
        public class VisuAlFroGInfo : GH_AssemblyInfo
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public override string Name => "VisuAlFroG";

            // Return a 24x24 pixel bitmap to represent this GHA library.
            public override Bitmap Icon => new Bitmap("resources/logo32.png");
            // NOTE: Logo icons are copied to output directory via post build event.

            // Return a short string describing the purpose of this GHA library.
            public override string Description => "Visual analytics framework providing the concept of visual analytics pipeline within grasshopper.";

            public override Guid Id => new Guid("1baa1150-3229-4744-888d-eb44f39968b4");

            // Return a string identifying you or your company.
            public override string AuthorName => "Matthias Braun";

            // Return a string representing your preferred contact details.
            public override string AuthorContact => "matthias.braun@intcdc.uni-stuttgart.de";
        }
    }
}
