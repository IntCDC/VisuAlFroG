using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Data;
using Core.GUI;
using System.Windows.Controls;


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

            object _Get { get; }
            uint _Dimension { get; }

            #endregion

            /* ------------------------------------------------------------------*/
            #region interface functions

            void UpdateData(GenericDataStructure data);

            void UpdateMetaDataEntry(IMetaData updated_meta_data);

            List<MenuItem> GetMenu();

            #endregion
        }
    }
}
