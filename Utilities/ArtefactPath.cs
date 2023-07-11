using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Utilities
{
    public class Artefacts
    {

        static public string Path()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // System.AppContext.BaseDirectory;
        }

        static public string FileName(string body, string extension)
        {
            const string prefix = "visfrog_";
            return prefix + body + "." + extension; ;
        }

        static public string FilePath(string body, string extension) 
        {
            return System.IO.Path.Combine(Utilities.Artefacts.Path(), Utilities.Artefacts.FileName(body, extension));
        }
    }
}
