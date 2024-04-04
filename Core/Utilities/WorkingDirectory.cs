using System;
using System.Reflection;
using System.Windows;



/*
 * Path to application artifacts and file generation
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
                Resources,
                LogoIcons,
                MenuIcons,
                Themes,
                Visualizations,
            }


            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            ///  [STATIC] Returns the location from which the application is executed            
            ///  E.g. location of the GHA plug-in: "C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisuAlFroG"
            /// </summary>
            /// <returns>The absolute path as string.</returns>
            public static string GetWorkingDirectory()
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
            public static string CreateFileName(string body, string extension)
            {
                // Append unified prefix
                const string prefix = "visualfrog_";
                return prefix + body + "." + extension; ;
            }

            /// <summary>
            /// [STATIC] Create application specific file path.
            /// </summary>
            /// <param name="body">The body of the file name.</param>
            /// <param name="extension">The file extension.</param>
            /// <returns>The absolute file path.</returns>
            public static string CreateFilePath(string body, string extension)
            {
                return System.IO.Path.Combine(Utilities.WorkingDirectory.GetWorkingDirectory(), Utilities.WorkingDirectory.CreateFileName(body, extension));
            }

            /// <summary>
            /// [STATIC] The application specific path to the file resource location.
            /// </summary>
            /// <param name="resource">Specify the required resource location.</param>
            /// <param name="filename">The file name of the resource.</param>
            /// <returns>The absolute resource file path.</returns>
            public static Uri GetResourcePath(Locations resource, string filename)
            {
                string resource_path = "resources"; // = Resources
                string assembly_name = "Core";      // Assembly.GetCallingAssembly().GetName().Name;
                switch (resource)
                {
                    case (Locations.LogoIcons):
                        resource_path = System.IO.Path.Combine(resource_path, "logo-icons");
                        break;
                    case (Locations.MenuIcons):
                        resource_path = System.IO.Path.Combine(resource_path, "menu-icons");
                        break;
                    case (Locations.Themes):
                        resource_path = System.IO.Path.Combine(resource_path, "color-themes");
                        break;
                    case (Locations.Visualizations):
                        assembly_name = "Visualizations";
                        resource_path = System.IO.Path.Combine(resource_path, "templates");
                        break;
                }
                string file_path = System.IO.Path.Combine("pack://application:,,,/" + assembly_name + ";component", resource_path, filename);
                return new Uri(file_path, UriKind.RelativeOrAbsolute);
            }

        }
    }
}
