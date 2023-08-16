using System;



/*
 * Path to application artefacts and file generation
 */
namespace Core
{
    namespace Utilities
    {
        public class Artefacts
        {

            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            ///  Returns the path of the VisFroG plugin
            ///  e.g. C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFrog
            ///  </summary>
            static public string Path()
            {
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // Alternative: System.AppContext.CoreDirectory;
            }


            static public string FileName(string body, string extension)
            {
                // Append unified prefix
                const string prefix = "visfrog_";
                return prefix + body + "." + extension; ;
            }


            static public string FilePath(string body, string extension)
            {
                return System.IO.Path.Combine(Utilities.Artefacts.Path(), Utilities.Artefacts.FileName(body, extension));
            }
        }
    }
}
