﻿using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using Core.Data;
using Core.Utilities;
using Core.Abstracts;



/*
 *  
 *  
 */
namespace Core
{
    namespace Filter
    {
        public class TransposeFilter : AbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public TransposeFilter()
            {
                _Name = "Transpose";
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            protected override UIElement create_update_ui(in GenericDataStructure in_data)
            {
                var ui = new TextBlock();
                ui.Text = "Transpose tabular 2D data.";
                return ui;
            }

            protected override void apply_filter(GenericDataStructure out_data)
            {
                out_data.Transpose();
            }

            #endregion
        }
    }
}
