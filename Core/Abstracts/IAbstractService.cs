using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Abstract Service Interface
 * 
 */
namespace Core
{
    namespace Abstracts
    {

        public interface IAbstractService
        {
            /* ------------------------------------------------------------------*/
            // interface functions

            bool Initialize();

            bool Terminate();
        }
    }
}
