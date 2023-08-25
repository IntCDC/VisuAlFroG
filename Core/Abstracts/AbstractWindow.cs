using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Abstracts;
using Core.Utilities;



/*
 * Abstract Child Window
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractWindow
        {

            /* ------------------------------------------------------------------*/
            // public delegates

            public delegate AvailableContentList_Type AvailableContents_Delegate();

            public delegate Control RequestContent_Delegate(string content_id, Type content_type);

            public delegate void DeleteContent_Delegate(string content_id);


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
            protected WindowBranch _parent_branch = null;
            protected ContentCallbacks_Type _content_callbacks = null;
        }
    }
}
