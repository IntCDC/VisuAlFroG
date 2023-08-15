using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;



/*
 * Abstract Child Content
 * 
 */
namespace Frontend
{
    namespace ChildWindows
    {
        public abstract class AbstractContent
        {

            /* ------------------------------------------------------------------*/
            // public types

            public delegate void SetContentAvailableCall(bool available);


            /* ------------------------------------------------------------------*/
            // public functions

            public AbstractContent(string header)
            {
                _header = header;
                _name = header.Replace(" ", "_") + AbstractChild.GenerateID(); ;
            }


            public string Name()
            {
                return _name;
            }

            public string Header()
            {
                return _header;
            }

            public abstract void ProvideContent(Grid content_grid);


            public bool IsAvailable()
            {
                return _available;
            }

            public void SetAvailable(bool a)
            {
                _available = a;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private readonly string _header;
            private readonly string _name;
            private bool _available = true;
        }
    }
}
