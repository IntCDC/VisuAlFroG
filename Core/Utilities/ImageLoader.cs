using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using Core.Utilities;
using System.Drawing;
using System.Reflection;



/*
 * Helpers for loading images from file
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class ImageLoader
        {
            /* ------------------------------------------------------------------*/
            #region static functions

            /// <summary>
            /// [STATIC] Create WPF __ImageSource__ for given resource location and file name.
            /// </summary>
            /// <param name="resource_location">One of the predefined resource locations.</param>
            /// <param name="filename">The file name of the image.</param>
            /// <returns>The image source.</returns>
            public static ImageSource ImageSourceFromFile(ResourcePaths.Locations resource_location, string filename)
            {
                var file_uri = ResourcePaths.GetResourcePath(resource_location, filename);
                try
                {
                    return new BitmapImage(file_uri);
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
            public static System.Windows.Controls.Image ImageFromFile(ResourcePaths.Locations resource_location, string filename)
            {
                var image = new System.Windows.Controls.Image();
                image.Source = ImageSourceFromFile(resource_location, filename);
                if (image.Source != null)
                {
                    return image;
                }
                return null;
            }

            /// <summary>
            /// [STATIC] Create WPF __Image__ for given resource location and file name.
            /// </summary>
            /// <param name="resource_location">One of the predefined resource locations.</param>
            /// <param name="filename">The file name of the image.</param>
            /// <returns>The image.</returns>
            public static Icon IconFromFile(ResourcePaths.Locations resource_location, string filename)
            {
                var file_uri = ResourcePaths.GetResourcePath(resource_location, filename);
                try
                {
                    return new Icon(Assembly.GetCallingAssembly().GetType(), file_uri.AbsolutePath);
                }
                catch (Exception exc)
                {
                    Log.Default.Msg(Log.Level.Error, exc.Message);
                }
                return null;
            }


            #endregion
        }
    }
}
