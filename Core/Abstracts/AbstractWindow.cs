using System.Windows.Controls;
using Core.GUI;



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

            public delegate AvailableContentsList_Type AvailableContents_Delegate();

            public delegate AttachContentMetaData_Type CreateContent_Delegate(string content_id, string content_type);

            public delegate bool DeleteContent_Delegate(string content_id);


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
                _content = null;
                _parent_is_root = false;
                _parent_branch = null;
                _content_callbacks = null;
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
