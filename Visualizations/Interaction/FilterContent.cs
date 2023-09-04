using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using Core.Abstracts;
using Core.Utilities;



/*
 *  Content for Data Filtering
 * 
 */
namespace Visualizations
{
    namespace Interaction
    {
        public class FilterContent : AbstractContent
        {
            /* ------------------------------------------------------------------*/
            // properties

            public override string Name { get { return "Data Filtering"; } }
            public override bool MultipleInstances { get { return true; } }
            public override List<Type> DependingServices { get { return new List<Type>() { }; } }


            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }
                _timer.Start();



                _timer.Stop();
                _initialized = true;
                if (_initialized)
                {
                    Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                }
                return _initialized;
            }

            public override bool Create()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                if (_created)
                {
                    Log.Default.Msg(Log.Level.Warn, "Content already created, skipping...");
                    return false;
                }
                _timer.Start();




                _timer.Stop();

                _created = true;
                return _created;
            }

            public override Control Attach()
            {
                if (!_initialized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return null;
                }
                if (!_created)
                {
                    Log.Default.Msg(Log.Level.Error, "Creation of content required prior to execution");
                    return null;
                }


                _content.Background = ColorTheme.BackgroundBlack;

                _attached = true;
                return _content;
            }

            public override bool Terminate()
            {
                if (_initialized)
                {
                    _content = null;


                    _initialized = false;
                }
                return true;
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ScrollViewer _content = null;

            /// TODO Connected visualizations...
        }
    }
}
