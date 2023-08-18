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

                RegisterContent<LogContent>();
                RegisterContent<TestVisualization>();

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

                return executed;
            }


            public override bool Terminate()
            {
                bool terminated = true;
                if (_initilized)
                {
                    _registerd_contents.Clear();
                    _contents.Clear();
                    _initilized = false;
                }
                return terminated;
            }


            public void RegisterContent<T>() where T : AbstractContent
            {
                Type content_type = typeof(T);
                _registerd_contents.Add(content_type);
                Log.Default.Msg(Log.Level.Info, "Registered Content: '" + content_type.FullName + "'");
            }


            public List<Type> GetDependingServices()
            {
                var depending_services = new List<Type>();
                foreach (Type content_type in _registerd_contents)
                {
                    // Get static member variable of type (= fields and not properties)
                    string fieldname = "depending_services";
                    FieldInfo property_depservices = content_type.GetField(fieldname, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    if (property_depservices != null)
                    {
                        var service_types = (List<Type>)property_depservices.GetValue(null);
                        depending_services.AddRange(service_types);
                    } else 
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to find field: '" + fieldname + "' in type: '" + content_type.ToString() + "'");
                    }
                }
                // Removing duplicates
                depending_services = depending_services.Distinct().ToList();
                return depending_services;
            }


            /// <summary>
            ///  Provide necessary information of available window content
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public AvailableContentList_Type GetContentsCallback()
            {
                var content_ids = new AvailableContentList_Type();
                foreach (Type content_type in _registerd_contents)
                {
                    // Get static member variables of type (= fields and not properties)
                    string fieldname = "name";
                    FieldInfo property_header = content_type.GetField(fieldname, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    if (property_header == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to find field: '" + fieldname + "' in type: '" + content_type.ToString() + "'");
                        break;
                    }
                    string header = (string)property_header.GetValue(null);

                    fieldname = "multiple_instances";
                    FieldInfo property_instances = content_type.GetField(fieldname, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    if (property_header == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to find field: '" + fieldname + "' in type: '" + content_type.ToString() + "'");
                        break;
                    }
                    bool multi = (bool)property_instances.GetValue(null);

                    // Content is only available if multiple instance are allowed or has not been instanciated yet
                    bool available = true;
                    foreach (var content_dict in _contents)
                    {
                        if ((content_dict.Value.GetType() == content_type) && !multi)
                        {
                            available = false;
                        }
                    }
                    content_ids.Add(new AvailableContent_Type(header, available, multi, content_type));
                }
                return content_ids;
            }


            /// <summary>
            /// Attach requested content to provided parent content element.
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public string RequestContentCallback(string content_id, Type content_type, Grid content_element)
            {
                if (_contents.ContainsKey(content_id))
                {
                    if (_contents[content_id].AttachContent(content_element))
                    {
                        return content_id;
                    }
                }
                else
                {
                    // Create new instance from type
                    var new_content = (AbstractContent)Activator.CreateInstance(content_type);
                    string new_id = new_content.ID();
                    _contents.Add(new_id, new_content);
                    if (_contents[new_id].AttachContent(content_element))
                    {
                        return new_id;
                    }
                }
                return UniqueID.Invalid;
            }


            /// <summary>
            /// Delete content.
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public void DeleteContentCallback(string content_id)
            {
                if (_contents.ContainsKey(content_id))
                {
                    _contents[content_id].DetachContent();
                    _contents.Remove(content_id);
                }
            }


            /* ------------------------------------------------------------------*/
            // private variables

            private ContentRegister_Type _registerd_contents = new ContentRegister_Type();
            private Dictionary<string, AbstractContent> _contents = new Dictionary<string, AbstractContent>();
        }
    }

}
