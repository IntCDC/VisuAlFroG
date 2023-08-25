using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;



/*
 * Abstract Content Interface
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public interface IAbstractContent
        {
            /* ------------------------------------------------------------------*/
            // interface properties

            string Name { get; }

            bool MultipleIntances { get; }

            List<Type> DependingServices { get; }

            bool IsAttached { get; }

            string ID { get; }


            /* ------------------------------------------------------------------*/
            // interface functions

            bool Create();

            Control Attach();

            bool Detach();
        }
    }
}
