using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.GUI;




/*
 * Abstract Child Window
 * 
 * 
 */

using ContentDataListType = System.Collections.Generic.List<System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCall>>;

namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractChild
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            // Provide 'id', 'header' and 'availability function' of content element
            public delegate ContentDataListType AvailableContentCall();

            public delegate bool RequestContentCall(string content_name, Grid content_grid);


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

            protected Grid _content = null;
            protected bool _parent_is_root = false;
            protected ChildBranch _parent_branch = null;
            protected AvailableContentCall _available_content = null;
            protected RequestContentCall _request_content = null;
        }
    }
}
