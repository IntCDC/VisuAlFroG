using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    namespace Utilities
    {
        public class UniqueStringID
        {

            /* ------------------------------------------------------------------*/
            // static functions

            public static string Generate()
            {
                return Guid.NewGuid().ToString("N");
            }
        }
    }
}
