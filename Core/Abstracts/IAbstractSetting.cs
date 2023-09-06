using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.GUI;



/*
 * Abstract Configuration Interfaces for data and classes providing some configuration
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public interface IAbstractConfigurationData
        {
            /// Empty so far 
        }


        public interface IAbstractConfiguration
        {
            /// <summary>
            /// Called for collecting all configurations.
            /// </summary>
            /// <returns>The serialized configurations as JSON string.</returns>
            string CollectConfigurations();

            /// <summary>
            /// Called for applying all configurations.
            /// </summary>
            /// <param name="configurations">The configurations as JSON string.</param>
            /// <returns>True on success, false otherwise.</returns>
            bool ApplyConfigurations(string configurations);
        }
    }
}
