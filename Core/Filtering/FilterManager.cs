using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using Core.Abstracts;
using Core.GUI;
using Core.Utilities;

using SciChartInterface.Data;



/*
 * Data Manager
 * 
 */
namespace Core
{
    namespace Data
    {
        public class FilterManager : AbstractRegisterService<AbstractFilter>
        {
            /* ------------------------------------------------------------------*/
            #region public delegates


            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public override bool Initialize()
            {
                if (!base.Initialize())
                {
                    return false;
                }
                _timer.Start();


                register_content(typeof(ColumnSelectionFilter));
                register_content(typeof(ValueSelectionFilter));


                _timer.Stop();
                _initialized = true;
                return _initialized;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {


                    _initialized = false;
                }
                return true;
            }


            public override void AttachMenu(MenubarMain menu_bar)
            {


            }

            public void AddFilterCallback()
            {


            }

            public void RemoveFilterCallback()
            {


            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            /// 
            /// </summary>
            /// <param name="content_data"></param>
            /// <returns></returns>
            protected override bool reset_content(AbstractFilter content_value)
            {

                return true; // content_value.Terminate();
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region private functions


            #endregion

            /* ------------------------------------------------------------------*/
            #region private variables



            #endregion
        }
    }
}
