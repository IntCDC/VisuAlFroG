using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.GUI;




/*
 * Abstract Child Window
 * 
 */

using ContentDataListType = System.Collections.Generic.List<System.Tuple<string, string, Core.Abstracts.AbstractContent.DetachContentCallback>>;

namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractChild
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            // Provide 'id', 'header' and 'availability function' of content element
            public delegate ContentDataListType AvailableContentCallback();

            public delegate bool RequestContentCallback(string content_name, Grid content_element);


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
            protected AvailableContentCallback _available_content = null;
            protected RequestContentCallback _request_content = null;
        }
    }
}
