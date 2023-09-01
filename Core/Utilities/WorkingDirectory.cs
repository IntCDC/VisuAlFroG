using System;
using System.Windows;



/*
 * Path to application artefacts and file generation
 */
namespace Core
{
    namespace Utilities
    {
        public class WorkingDirectory
        {
            /* ------------------------------------------------------------------*/
            // public types

            public enum Locations
            {
                WorkingDirectory,
                Resource,
                MenuIcons,
            }


            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            ///  Returns the path of the VisFroG plugin
            ///  e.g. C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFrog
            ///  </summary>
            public static string Path()
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // Alternative: System.AppContext.CoreDirectory;
            }


            public static string FileName(string body, string extension)
            {
                // Append unified prefix
                const string prefix = "visfrog_";
                return prefix + body + "." + extension; ;
            }


            public static string FilePath(string body, string extension)
            {
                return System.IO.Path.Combine(Utilities.WorkingDirectory.Path(), Utilities.WorkingDirectory.FileName(body, extension));
            }


            public static string ResourcePath(Locations resource, string filename)
            {
                // resource == WorkingDirectory:
                var resource_path = "";
                switch (resource)
                {
                    case (Locations.Resource):
                        resource_path = "resources";
                        break;
                    case (Locations.MenuIcons):
                        resource_path = System.IO.Path.Combine("resources", "menu");
                        break;
                }
                return System.IO.Path.Combine(Utilities.WorkingDirectory.Path(), resource_path, filename);
            }

        }
    }
}
