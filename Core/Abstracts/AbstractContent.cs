using System;
using System.Windows.Controls;



/*
 * Abstract Child Content
 * 
 */
namespace Core
{
    namespace Abstracts
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
