using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Abstracts;
using Core.GUI;
using Core.Utilities;



// Parameters: <name, available, is-multi-instance, type>
using AvailableContentList_Type = System.Collections.Generic.List<System.Tuple<string, bool, bool, System.Type>>;

using ContentCallbacks = System.Tuple<Core.Abstracts.AbstractWindow.AvailableContents_Delegate, Core.Abstracts.AbstractWindow.RequestContent_Delegate, Core.Abstracts.AbstractWindow.DeleteContent_Delegate>;


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

            public delegate string RequestContent_Delegate(string content_id, Type content_type, Grid content_element);

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
            protected ContentCallbacks _content_callbacks = null;
        }
    }
}
