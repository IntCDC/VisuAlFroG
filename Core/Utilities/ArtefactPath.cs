﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



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

            static public string Path()
            {
                // Returns the path of the VisFroG plugin, e.g. C:\Users\...\AppData\Roaming\Grasshopper\Libraries\VisFrog
                return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                /// Alternative: System.AppContext.CoreDirectory;
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
