using System;
using System.Reflection;
using System.Windows;



/*
 * Get paths to application resources or generate file paths for output files
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class ResourcePaths
        {
            /* ------------------------------------------------------------------*/
            #region public types

            public enum Locations
            {
                LogoIcons,
                MenuIcons,
                Themes,
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region static functions

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
                return System.IO.Path.Combine(Utilities.ResourcePaths.GetWorkingDirectory(), Utilities.ResourcePaths.CreateFileName(body, extension));
            }

            /// <summary>
            /// [STATIC] The application specific path to the file resource location.
            /// </summary>
            /// <param name="resource">Specify the required resource location.</param>
            /// <param name="filename">The file name of the resource.</param>
            /// <returns>The absolute resource file path.</returns>
            public static Uri GetResourcePath(Locations resource, string filename)
            {
                // name of top level resource directory
                string resource_path = "resources";
                string assembly_name = Assembly.GetCallingAssembly().GetName().Name; // "Core"

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
                    default:
                        Log.Default.Msg(Log.Level.Error, "Unknown resource location.");
                        return new Uri("");
                }

                string file_path = System.IO.Path.Combine("pack://application:,,,/" + assembly_name + ";component", resource_path, filename);
                return new Uri(file_path, UriKind.RelativeOrAbsolute);
            }

            #endregion
        }
    }
}
