using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Core.Utilities;
using Core.Abstracts;
using Core.GUI;
using Visualizations.Types;
using System.Reflection;
using System.Windows;



using ContentRegister_Type = System.Collections.Generic.List<System.Type>;
// Parameters: <name, available, is-multi-instance, type>
using AvailableContent_Type = System.Tuple<string, bool, bool, System.Type>;
// Parameters: <name, available, is-multi-instance, type>
using AvailableContentList_Type = System.Collections.Generic.List<System.Tuple<string, bool, bool, System.Type>>;
using SciChart.Core.Extensions;
using SciChart.Charting.Visuals;


/*
 * Content Manager
 * 
 */
namespace Visualizations
{
    namespace Management
    {
        public class ContentManager : AbstractService
        {

            /* ------------------------------------------------------------------*/
            // public functions

            public override bool Initialize()
            {
                if (_initilized)
                {
                    Terminate();
                }

                RegisterContent(typeof(LogContent));
                RegisterContent(typeof(TestVisualization));

                _initilized = true;
                return _initilized;
            }


            public override bool Execute()
            {
                if (!_initilized)
                {
                    Log.Default.Msg(Log.Level.Error, "Initialization required prior to execution");
                    return false;
                }
                bool executed = true;
                // Contents are loaded via callbacks
                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    foreach (var c_data in _contents)
                    {
                        foreach (var c in c_data.Value)
                        {
                            c.Value.Detach();
                        }
                        c_data.Value.Clear();
                    }
                    _contents.Clear();
                    _initilized = false;
                }
                return terminated;
            }


            public void RegisterContent(Type content_type)
            {
                // Check for required base type
                Type base_name = content_type.BaseType;
                string required_basetype = typeof(AbstractContent).Name;
                bool valid_type = false;
                while (!base_name.Name.StartsWith(typeof(object).Name))
                {
                    if (base_name.Name.StartsWith(required_basetype))
                    {
                        valid_type = true;
                        break;
                    }
                    base_name = base_name.BaseType;
                    if (base_name == null)
                    {
                        break;
                    }
                }
                if (valid_type)
                {
                    _contents.Add(content_type, new Dictionary<string, AbstractContent>());
                    Log.Default.Msg(Log.Level.Info, "Registered Content: " + content_type.Name);
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Wrong input type for content: " + base_name);
                }
            }


            public List<Type> DependingServices()
            {
                var depending_services = new List<Type>();

                // Loop over registered types
                foreach (var c_data in _contents)
                {
                    Type c_type = c_data.Key;

                    // Create temporary instance of content
                    var tmp_content = (AbstractContent)Activator.CreateInstance(c_type);
                    depending_services.AddRange(tmp_content.DependingServices);
                }
                // Removing duplicates
                depending_services = depending_services.Distinct().ToList();

                return depending_services;
            }


            /// <summary>
            ///  Provide necessary information of available window content
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public AvailableContentList_Type ContentsCallback()
            {
                var content_ids = new AvailableContentList_Type();

                // Loop over registered types
                foreach (var c_data in _contents)
                {
                    Type c_type = c_data.Key;

                    // Create temporary instance of content
                    var tmp_content = (AbstractContent)Activator.CreateInstance(c_type);
                    string header = tmp_content.Name;
                    bool multiple_instances = tmp_content.MultipleIntances;

                    // Content is only available if multiple instance are allowed or has not been instanciated yet
                    bool available = (multiple_instances || (c_data.Value.IsEmpty() && !multiple_instances));

                    content_ids.Add(new AvailableContent_Type(header, available, multiple_instances, c_type));
                }

                return content_ids;
            }


            /// <summary>
            /// Attach requested content to provided parent content element.
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public string AttachContentCallback(string content_id, Type content_type, Grid content_element)
            {
                // Loop over registered types
                if (_contents.ContainsKey(content_type))
                {
                    string id = content_id;
                    if (!_contents[content_type].ContainsKey(id))
                    {
                        // Create new instance from type
                        var new_content = (AbstractContent)Activator.CreateInstance(content_type);
                        new_content.Create();

                        id = new_content.ID;
                        _contents[content_type].Add(id, new_content);
                    }

                    if (_contents[content_type][id].Attach(content_element))
                    {
                        return id;
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Unregistered content type: " + content_type.ToString());
                }
                return UniqueID.Invalid;
            }


            /// <summary>
            /// Delete content.
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public void DetachContentCallback(string content_id)
            {
                // Loop over registered types
                foreach (var c_data in _contents)
                {
                    Type c_type = c_data.Key;

                    if (c_data.Value.ContainsKey(content_id))
                    {
                        c_data.Value[content_id].Detach();
                        c_data.Value.Remove(content_id);
                    }
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            // separate dict for each content type
            private Dictionary<Type, Dictionary<string, AbstractContent>> _contents = new Dictionary<Type, Dictionary<string, AbstractContent>>();

        }
    }

}
