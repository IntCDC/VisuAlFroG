using System;
using System.Linq;
using System.Windows.Markup;



/*
 * Generate unique ID string
 * 
 */
namespace Core
{
    namespace Utilities
    {
        public class UniqueID
        {

            /* ------------------------------------------------------------------*/
            #region static functions

            /// <summary>
            /// [STATIC] Generate unique string id.
            /// </summary>
            /// <returns>The id as string.</returns>
            public static string GenerateString()
            {
                var random = new Random();
                string id = Guid.NewGuid().ToString("N");
                const string letters = "abcdefghijklmnopqrstuvwxyz";
                // Prefix letter to get valid WPF names
                id = letters[random.Next(letters.Length)] + id;
                return id;
            }

            public static int GenerateInt()
            {
                var uid = Guid.NewGuid();
                return uid.GetHashCode();
            }

            /// <summary>
            /// [STATIC] Variable representing an invalid id.
            /// </summary>
            public static string InvalidString { get { return "invalid"; } }

            /// <summary>
            /// [STATIC] Variable representing an invalid id.
            /// </summary>
            public static int InvalidInt { get { return int.MinValue; } }

            #endregion
        }
    }
}
