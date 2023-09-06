using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Abstracts;
using Core.GUI;



/*
 * Globally available type aliases
 * 
 */
namespace Core
{
    namespace Utilities
    {
        /// <summary>
        /// Content callbacks used in WindowLeaf for interaction between context menu and visualization content
        /// </summary>
        public class ContentCallbacks_Type : Tuple<AbstractWindow.AvailableContents_Delegate, AbstractWindow.CreateContent_Delegate, AbstractWindow.DeleteContent_Delegate>
        {
            public ContentCallbacks_Type(AbstractWindow.AvailableContents_Delegate a1, AbstractWindow.CreateContent_Delegate a2, AbstractWindow.DeleteContent_Delegate a3) : base(a1, a2, a3) { }
        }

        /// <summary>
        /// Meta data of content required by WindowLeaf.
        /// </summary>
        public class ReadContentMetaData_Type : Tuple<string, bool, bool, string>
        {
            // Arguments: <content name, is content available, multiple instances allowed, content type>
            public ReadContentMetaData_Type(string a1, bool a2, bool a3, string a4) : base(a1, a2, a3, a4) { }
        }
        /// <summary>
        /// List of meta data of available contents.
        /// </summary>
        public class AvailableContentsList_Type : List<ReadContentMetaData_Type> { }


        /// <summary>
        /// Meta data required by WindowLeaf to attach content.
        /// </summary>
        public class AttachContentMetaData_Type : Tuple<string, System.Windows.Controls.Panel>
        {
            // Arguments: <content name, is content available, multiple instances allowed, content type>
            public AttachContentMetaData_Type(string a1, System.Windows.Controls.Panel a2) : base(a1, a2) { }
        }
    }
}
