using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Frontend.ChildWindows;



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
            protected AbstractContent.AvailableContentCall _available_content = null;
            protected AbstractContent.RequestContentCall _request_content = null;
        }
    }
}
