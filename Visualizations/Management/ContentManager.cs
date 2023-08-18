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
// Parameters: <name, available, type>
using AvailableContent_Type = System.Tuple<string, bool, System.Type>;
// Parameters: <name, available, type>
using AvailableContentList_Type = System.Collections.Generic.List<System.Tuple<string, bool, System.Type>>;


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


            /// <summary>
            ///  Provide necessary information of available window content
            /// >> Called by child leaf in _subwindows
            /// </summary>
            public AvailableContentList_Type GetContentsCallback()
            {
                var content_ids = new AvailableContentList_Type();
                foreach (Type content_type in _registerd_contents)
                {
                    string fieldname = "name";
                    FieldInfo property_header = content_type.GetField(fieldname);
                    if (property_header == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to find field: '" + fieldname + "'");
                        break;
                    }
                    fieldname = "multiple_instances";
                    FieldInfo property_instances = content_type.GetField(fieldname);
                    if (property_header == null)
                    {
                        Log.Default.Msg(Log.Level.Error, "Unable to find field: '" + fieldname + "'");
                        break;
                    }
                    
                    string header = (string)property_header.GetValue(null);
                    bool multi = (bool)property_instances.GetValue(null);
                    bool available = true;
                    foreach (var c in _contents)
                    {
                        if ((c.Value.GetType() == content_type) && !multi)
                        {
                            available = false;
                        }
                    }
                    content_ids.Add(new AvailableContent_Type(header, available, content_type));
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
