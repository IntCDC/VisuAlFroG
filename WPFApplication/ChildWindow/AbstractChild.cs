using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Frontend.ChildWindows;

using static Frontend.ChildWindows.AbstractContent;



/*
 * Abstract Child Window
 * 
 * 
 */
namespace Frontend
{
    namespace ChildWindows
    {
        public abstract class AbstractChild
        {
            /* ------------------------------------------------------------------*/
            // static functions

            public static string GenerateID()
            {
                return Guid.NewGuid().ToString("N");
            }

            // Provide header and name of content element
            public delegate List<Tuple<string, string, SetContentAvailableCall>> AvailableContentCall();
            public delegate void RequestContentCall(string content_name, Grid content_grid);


            /* ------------------------------------------------------------------*/
            // public types

            public enum SplitOrientation
            {
                None,
                Horizontal,
                Vertical,
            }

            public enum ChildLocation
            {
                None,
                Top_Left,
                Bottom_Right,
            }

            /* ------------------------------------------------------------------*/
            // protected variables

            protected Grid _grid = null;
            protected bool _parent_is_root = false;
            protected ChildBranch _parent_branch = null;
            protected AvailableContentCall _available_content = null;
            protected RequestContentCall _request_content = null;
        }
    }
}
