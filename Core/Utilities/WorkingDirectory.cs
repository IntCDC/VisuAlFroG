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
            ///  [STATIC] Returns the location from which the application is executed            
            ///  E.g. location of the GHA plug-in: "C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFrog"
            /// </summary>
            /// <returns>The absolute path as string.</returns>
            public static string Path()
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // Alternative: System.AppContext.CoreDirectory;
            }

            /// <summary>
            /// [STATIC] Create application specific file name.
            /// </summary>
            /// <param name="body">The file name body.</param>
            /// <param name="extension">The file extension.</param>
            /// <returns>The file name.</returns>
            public static string FileName(string body, string extension)
            {
                // Append unified prefix
                const string prefix = "visfrog_";
                return prefix + body + "." + extension; ;
            }

            /// <summary>
            /// [STATIC] Create application specific file path.
            /// </summary>
            /// <param name="body">The body of the file name.</param>
            /// <param name="extension">The file extension.</param>
            /// <returns>The absolute file path.</returns>
            public static string FilePath(string body, string extension)
            {
                return System.IO.Path.Combine(Utilities.WorkingDirectory.Path(), Utilities.WorkingDirectory.FileName(body, extension));
            }

            /// <summary>
            /// [STATIC] The application specific path to the file resource location.
            /// </summary>
            /// <param name="resource">Specify the required resource location.</param>
            /// <param name="filename">The file name of the resource.</param>
            /// <returns>The absolute resource file path.</returns>
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
