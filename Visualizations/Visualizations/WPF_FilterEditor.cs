using System;
using System.Windows;
using System.Windows.Controls;
using Core.Utilities;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections.Generic;
using Core.Data;
using Core.Abstracts;
using Visualizations.WPFInterface;
using Core.GUI;



/*
 * Filter Editor Window Content
 * 
 */
namespace Visualizations
{
    public class WPF_FilterEditor : AbstractWPFVisualization<ScrollViewer>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _TypeName { get { return "Filter Editor (WPF)"; } }
        public override bool _MultipleInstances { get { return false; } }

        // Indicates to not create an unused copy of the data
        public override Type _RequiredDataType { get; } = null;

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public WPF_FilterEditor(int uid) : base(uid) { }

        public override bool Create()
        {
            if (!_initialized)
            {
                Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                return false;
            }
            if (_created)
            {
                // Log Console does not depend on data
                Log.Default.Msg(Log.Level.Debug, "Content already created. Skipping re-creating content.");
                return false;
            }
            _timer.Start();





            _timer.Stop();
            _created = true;
            return _created;
        }

        public override void Update(bool new_data)
        {
            if (!_created)
            {
                Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                return;
            }





        }

        public override bool Terminate()
        {
            return base.Terminate();
        }

        public override void AttachMenu(MenubarWindow menubar)
        {
            base.AttachMenu(menubar);

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
