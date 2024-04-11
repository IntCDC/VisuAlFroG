using System;



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

            /// <summary>
            /// Initialize the service.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Initialize();

            /// <summary>
            /// Terminate the service. Should implement counterpart to Initialize().
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            bool Terminate();
        }
    }
}
