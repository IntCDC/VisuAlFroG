using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Abstracts;



/*
 * Type Aliases
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class ContentCallbacks_Type : Tuple<AbstractWindow.AvailableContents_Delegate, AbstractWindow.RequestContent_Delegate, AbstractWindow.DeleteContent_Delegate>
        {
            public ContentCallbacks_Type(AbstractWindow.AvailableContents_Delegate a1, AbstractWindow.RequestContent_Delegate a2, AbstractWindow.DeleteContent_Delegate a3) : base(a1, a2, a3) { }
        }


        public class AvailableContent_Type : Tuple<string, bool, bool, Type>
        {
            // Arguments: <content name, is content available, multiple instances allowed, content type>
            public AvailableContent_Type(string a1, bool a2, bool a3, Type a4) : base(a1, a2, a3, a4) { }
        }


        public class AvailableContentList_Type : List<AvailableContent_Type>
        {
        }


        public class AttachedContent_Type : Tuple<string, Type>
        {
            // Arguments: <content id, content type>
            public AttachedContent_Type(string a1, Type a2) : base(a1, a2) { }
        }


        public class AbstractData_Type : List<List<double>>
        {
        }
    }
}
