using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data;

using SciChart.Charting.Model.Filters;


/*
 *  SciChart data filter for series data type
 *  
 */
namespace SciChartInterface
{
    namespace Data
    {
        public class ColumnSelectionFilter : AbstractFilter
        {
            /* ------------------------------------------------------------------*/
            #region public functions

            public ColumnSelectionFilter(int uid) : base(uid) { }

            public override bool CreateUI()
            {
                bool created = base.CreateUI();



                return created;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions


            #endregion
        }
    }
}
