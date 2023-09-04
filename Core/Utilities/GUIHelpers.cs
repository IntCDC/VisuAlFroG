using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;



/*
 * Global GUI Color Theme
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class ImageHelper
        {
            /* ------------------------------------------------------------------*/
            // static functions

            /// <summary>
            /// [STATIC] Create WPF __ImageSource__ for given resource location and file name.
            /// </summary>
            /// <param name="resource_location">One of the predefined resource locations.</param>
            /// <param name="filename">The file name of the image.</param>
            /// <returns>The image source.</returns>
            public static ImageSource ImageSourceFromFile(WorkingDirectory.Locations resource_location, string filename)
            {
                var file_path = WorkingDirectory.ResourcePath(resource_location, filename);
                if (!File.Exists(file_path))
                {
                    Log.Default.Msg(Log.Level.Error, "File not found: " + file_path);
                }
                try
                {
                    return new BitmapImage(new Uri(file_path, UriKind.RelativeOrAbsolute));
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return null;
            }

            /// <summary>
            /// [STATIC] Create WPF __Image__ for given resource location and file name.
            /// </summary>
            /// <param name="resource_location">One of the predefined resource locations.</param>
            /// <param name="filename">The file name of the image.</param>
            /// <returns>The image.</returns>
            public static Image ImageFromFile(WorkingDirectory.Locations resource_location, string filename)
            {
                var image = new Image();
                image.Source = ImageSourceFromFile(resource_location, filename);
                if (image.Source != null)
                {
                    return image;
                }
                return null;
            }
        }
    }
}
