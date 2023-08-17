using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;



/*
 * VisFrog Grasshopper Component Info
 * 
 */
namespace Interface
{
    namespace GrasshopperComponent
    {
        public class VisFroGInfo : GH_AssemblyInfo
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public override string Name => "VisFroG";

            // Return a 24x24 pixel bitmap to represent this GHA library.
            public override Bitmap Icon => new Bitmap("resources/logo32.png");

            // Return a short string describing the purpose of this GHA library.
            public override string Description => "Visualization framework providing the concept of visual analytics pipeline within grasshopper.";

            public override Guid Id => new Guid("1baa1150-3229-4744-888d-eb44f39968b4");

            // Return a string identifying you or your company.
            public override string AuthorName => "Matthias Braun";

            // Return a string representing your preferred contact details.
            public override string AuthorContact => "matthias.braun@intcdc.uni-stuttgart.de";
        }
    }
}
