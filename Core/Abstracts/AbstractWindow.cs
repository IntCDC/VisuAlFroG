using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.GUI;



/*
 * Abstract Child Window
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;

using AttachContentMetaData_Type = System.Tuple<string, System.Windows.Controls.Panel>;


namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractWindow
        {
            /* ------------------------------------------------------------------*/
            // public delegates

            public delegate List<ReadContentMetaData_Type> AvailableContents_Delegate();

            public delegate AttachContentMetaData_Type CreateContent_Delegate(string content_id, string content_type);

            public delegate bool DeleteContent_Delegate(string content_id);


            /* ------------------------------------------------------------------*/
            // public properties

            public Grid _Content { get; protected set; } = null;


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
            // protected functions

            protected void Reset()
            {
                _Content = null;
                _parent_is_root = false;
                _parent_branch = null;
                _content_callbacks = null;
            }


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _parent_is_root = false;
            protected WindowBranch _parent_branch = null;
            protected Tuple<AvailableContents_Delegate, CreateContent_Delegate, DeleteContent_Delegate> _content_callbacks = null;
        }
    }
}
