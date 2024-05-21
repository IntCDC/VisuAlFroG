using Core.Utilities;

using Grasshopper;
using Grasshopper.Kernel;

using System;
using System.Drawing;
using System.Reflection;



/*
 * VisuAlFroG Grasshopper Component Info
 * 
 */
namespace GrasshopperInterface
{
    public class VisuAlFroGInfo : GH_AssemblyInfo
    {

        /* ------------------------------------------------------------------*/
        #region public functions

        public override string Name => "VisuAlFroG";

        // Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("GrasshopperInterface.resources.logo.logo32.png");
                return new System.Drawing.Bitmap(stream);
            }
        }

        // Return a short string describing the purpose of this GHA library.
        public override string Description => "Visual analytics framework providing the concept of visual analytics pipeline within grasshopper.";

        public override Guid Id => new Guid("1baa1150-3229-4744-888d-eb44f39968b4");

        // Return a string identifying you or your company.
        public override string AuthorName => "Matthias Braun";

        // Return a string representing your preferred contact details.
        public override string AuthorContact => "matthias.braun@intcdc.uni-stuttgart.de";

        #endregion
    }
}
