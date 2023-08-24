using System;



/*
 * Generate unique ID string
 */
namespace Core
{
    namespace Utilities
    {
        public class UniqueID
        {

            /* ------------------------------------------------------------------*/
            // static functions

            public static string Generate()
            {
                return Guid.NewGuid().ToString("N");
            }


            public static readonly string Invalid = "invalid";
        }
    }
}
