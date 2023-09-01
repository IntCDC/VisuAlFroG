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
 * Data Filtering Window Content
 * 
 */
namespace Visualizations
{

    namespace Interaction
    {
        namespace Filtering
        {
            internal class FilterContent : AbstractContent
            {
                /* ------------------------------------------------------------------*/
                // properties

                public override string Name { get { return "Data Filtering"; } }
                public override bool MultipleIntances { get { return true; } }
                public override List<Type> DependingServices { get { return new List<Type>() { }; } }


                /* ------------------------------------------------------------------*/
                // public functions

                public override bool Initialize()
                {
                    if (_initilized)
                    {
                        Terminate();
                    }
                    _timer.Start();



                    _timer.Stop();
                    _initilized = true;
                    if (_initilized)
                    {
                        Log.Default.Msg(Log.Level.Info, "Successfully initialized: " + this.GetType().FullName);
                    }
                    return _initilized;
                }


                public override bool Create()
                {
                    if (!_initilized)
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
                    if (!_initilized)
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
                    if (_initilized)
                    {
                        _content = null;


                        _initilized = false;
                    }
                    return true;
                }


                /* ------------------------------------------------------------------*/
                // private variables

                private ScrollViewer _content = null;

                // connected visualization ???
            }
        }
    }
}
