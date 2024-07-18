using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Core.Abstracts;
using Core.Filter;
using Core.GUI;
using Core.Utilities;
using Visualizations.WPFInterface;

using static Core.Filter.FilterManager;



/*
 * Filter Editor Window Content
 * 
 */
namespace Visualizations
{
    public class WPF_FilterEditor : AbstractWPFVisualization<Grid>
    {
        /* ------------------------------------------------------------------*/
        #region public properties

        public override string _Name { get { return "Filter Editor"; } }
        public override bool _MultipleInstances { get { return false; } }

        // Indicates to not create an unused copy of the data
        public override Type _RequiredDataType { get; } = null;

        #endregion

        /* ------------------------------------------------------------------*/
        #region public functions

        public WPF_FilterEditor(int uid) : base(uid) { }

        public override bool CreateUI()
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


            if (_get_ui_callback != null)
            {
                _Content.Children.Add(_get_ui_callback());
            }
            else
            {
                Log.Default.Msg(Log.Level.Error, "Missing callback to get UI. Set before calling this function.");
            }

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

            // Unused
        }

        public override bool Terminate()
        {
            if (_initialized)
            {
                _get_ui_callback = null;
                _initialized = false;
            }
            return base.Terminate();
        }

        public override void AttachMenu(MenubarWindow menubar)
        {
            base.AttachMenu(menubar);
        }


        public void RequestUICallback(GetUICallback_Delegate get_ui_callback)
        {
            _get_ui_callback = get_ui_callback;
        }

        #endregion

        /* ------------------------------------------------------------------*/
        #region private variables

        private GetUICallback_Delegate _get_ui_callback = null;

        #endregion
    }
}
