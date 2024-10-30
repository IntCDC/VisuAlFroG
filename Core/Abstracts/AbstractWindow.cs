using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Core.Windows;



/*
 * Abstract Child Window
 * 
 */

// Arguments: <content name, flag: is content available, flag: are multiple instances allowed, content type>
using ReadContentMetaData_Type = System.Tuple<string, bool, bool, string>;

using AttachContentMetaData_Type = System.Tuple<int, string, System.Windows.UIElement, Core.Abstracts.AbstractVisualization.AttachWindowMenu_Delegate>;
using System.Windows.Data;


namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractWindow
        {
            /* ------------------------------------------------------------------*/
            #region public delegates

            public delegate List<ReadContentMetaData_Type> AvailableContents_Delegate();

            public delegate AttachContentMetaData_Type CreateContent_Delegate(int content_uid, string content_type, Binding name_binding);

            public delegate bool DeleteContent_Delegate(int content_uid);

            #endregion

            /* ------------------------------------------------------------------*/
            #region public properties

            public Grid _Content { get; protected set; } = null;

            #endregion

            /* ------------------------------------------------------------------*/
            #region public types

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

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected void reset()
            {
                _Content = null;
                _parent_is_root = false;
                _parent_branch = null;
                _content_callbacks = null;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected bool _parent_is_root = false;
            protected WindowBranch _parent_branch = null;
            protected Tuple<AvailableContents_Delegate, CreateContent_Delegate, DeleteContent_Delegate> _content_callbacks = null;

            #endregion
        }
    }
}
