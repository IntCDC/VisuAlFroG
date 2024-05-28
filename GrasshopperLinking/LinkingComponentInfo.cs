using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;


/*
 * Grasshopper component linking component info
 * 
 */
namespace GrasshopperLinking
{
    public class LinkingComponentInfo : GH_AssemblyInfo
    {
        public override string Name => "LinkingComponent";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("GrasshopperLinking.resources.logo.logo32.png");
                return new System.Drawing.Bitmap(stream);
            }
        }

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "[TODO]";

        public override Guid Id => new Guid("f38e8789-ee0d-474c-a7c5-e85794af4ec5");

        // Return a string identifying you or your company.
        public override string AuthorName => "Matthias Braun";

        // Return a string representing your preferred contact details.
        public override string AuthorContact => "matthias.braun@intcdc.uni-stuttgart.de";
    }
}
