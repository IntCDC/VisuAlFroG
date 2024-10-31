using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Data;
using Core.GUI;
using System.Windows.Controls;
using Core.Utilities;


/*
 *  Data variety interface
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public interface IDataType
        {
            /* ------------------------------------------------------------------*/
            #region interface properties

            int _UID { get; }
            object _Specific { get; }
            GenericDataStructure _Generic { get; }
            uint _Dimension { get; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region interface functions

            void UpdateData(GenericDataStructure data);

            List<Control> GetDataMenu();

            #endregion
        }
    }
}
