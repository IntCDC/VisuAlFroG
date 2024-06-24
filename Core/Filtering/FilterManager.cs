using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Core.Abstracts;
using Core.GUI;
using Core.Utilities;



/*
 * Data Manager
 * 
 */
namespace Core
{
    namespace Data
    {
        public class FilterManager : AbstractService
        {
            /* ------------------------------------------------------------------*/
            #region public delegates


            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();



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
