using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.GUI;



/*
 * Abstract Settings Interfaces for data and classes providing settings
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public interface IAbstractSettingData
        {
            /// Empty so far 
        }


        public interface IAbstractSettings
        {
            /// <summary>
            /// Called for collecting all settings.
            /// </summary>
            /// <returns>The serialized settings as JSON string.</returns>
            string CollectSettings();

            /// <summary>
            /// Called for applying all settings.
            /// </summary>
            /// <param name="settings">The settings as JSON string.</param>
            /// <returns>True on success, false otherwise.</returns>
            bool ApplySettings(string settings);
        }
    }
}
