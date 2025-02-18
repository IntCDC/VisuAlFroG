using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Core.Utilities;
using Core.Data;
using Core.Abstracts;
using System.Windows;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChartInterface.Data;



/*
 * Abstract Visualization for SciChart based visualizations relying on the SciChartSurface.
 * 
 */
namespace Visualizations
{
    namespace WPFInterface
    {
        public abstract class AbstractWPFVisualization<ContentType> : AbstractVisualization
            where ContentType : UIElement, new()
        {
            /* ------------------------------------------------------------------*/
            #region public properties

            public sealed override List<Type> _DependingServices { get { return new List<Type>() { }; } }
            public override Type _RequiredDataType { get; } = typeof(DataTypeGeneric);

            protected ContentType _Content { get; } = new ContentType();

            #endregion

            /* ------------------------------------------------------------------*/
            #region public functions

            public AbstractWPFVisualization(int uid) : base(uid) { }

            public override bool Initialize(DataManager.GetSpecificDataCallback_Delegate request_data_callback, DataManager.GetDataMenuCallback_Delegate request_menu_callback,
                UpdateSeriesSelection_Delegate update_selection_series)
            {
                _timer.Start();

                if (base.Initialize(request_data_callback, request_menu_callback, update_selection_series))
                {
                    attach_child_content(_Content);
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }

                _timer.Stop();
                return _initialized;
            }

            /* TEMPLATE
            public override bool CreateUI()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                _timer.Start();

                /// PLACE YOUR STUFF HERE ...

                _timer.Stop();
                _created = true;
                return _created;
            }
            */

            /* TEMPLATE
            public override void Update(bool new_data)
            {
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation required prior to execution");
                    return;
                }

                if (new_data)
                {
                    // Re-creation of content is required for new data
                    Create();
                }
                else
                {
                    /// PLACE YOUR STUFF HERE ...
                }
            }
            */

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _initialized = false;
                }
                return base.Terminate();
            }

            public override void UpdateEntrySelection(IMetaData updated_meta_data)
            {
                if (get_data(out GenericDataStructure data))
                {
                    var entry = data.GetEntryAtIndex(updated_meta_data._Index);
                    if (entry != null)
                    {
                        entry._Metadata._Selected = updated_meta_data._Selected;
                    }
                    else
                    {
                        Log.Default.Msg(Log.Level.Debug, "Can not find data entry at index: " + updated_meta_data._Index.ToString());
                    }
                }
            }

            public override void UpdateSeriesSelection(List<int> updated_series_indexes)
            {
                if (get_data(out GenericDataStructure data))
                {
                    for (int i = 0; i < data._Branches.Count; i++)
                    {
                        bool selected = updated_series_indexes.Contains(i);

                        foreach (var entry in data._Branches[i]._Entries)
                        {
                            entry._Metadata._Selected = selected;
                        }
                    }
                }
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            ///  UNUSED
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            protected override void event_series_selection_changed(object sender, EventArgs e)
            {
                // TODO Series selection in WPF content is currently not used
                /// Add branch meta data storing information about series selection = all entires of a branch are selected
                var series_indexes = new List<int>();
                _UpdateSeriesSelection(_UID, series_indexes);
            }

            #endregion

        }
    }
}
