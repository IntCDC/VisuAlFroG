using System;
using System.Collections.Generic;
using System.Reflection;

using Core.GUI;
using Core.Utilities;



/*
 * Abstract Library
 * 
 */
namespace Core
{
    namespace Abstracts
    {
        public abstract class AbstractRegisterService<entryBaseType> : AbstractService
        {

            /* ------------------------------------------------------------------*/
            #region public  functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }

                _entries = new Dictionary<Type, Dictionary<int, entryBaseType>>();

                return true;
            }

            public override bool Terminate()
            {
                reset_entry();
                return true;
            }

            #endregion


            /* ------------------------------------------------------------------*/
            #region abstract functions

            protected abstract bool reset_entry(entryBaseType entry_value);

            /// <summary>
            /// Convert string to type.
            /// </summary>
            /// <param name="type_string">The type as string.</param>
            /// <returns>The requested type, default(?) otherwise.</returns>
            protected Type get_type(string type_string)
            {
                Type type = default(Type);
                try
                {
                    // Try to load type from "Core" (=current) assembly (suppress errors -> return null on error)
                    type = Type.GetType(type_string, false);
                    if (type == null)
                    {
                        // Try to load type from "Visualizations" assembly - trow error if this is also not possible
                        type = Assembly.Load("Visualizations").GetType(type_string, true);
                    }
                }
                catch (TypeLoadException e)
                {
                    Log.Default.Msg(Log.Level.Error, e.Message);
                }
                return type;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            /// Register new entry type.
            /// </summary>
            /// <param name="entry_type">The entry type.</param>
            protected void register_entry(Type entry_type)
            {
                // Check for required base type
                Type type = entry_type;
                bool valid_type = false;
                while (!recursive_basetype(type, typeof(object)))
                {
                    if (recursive_basetype(type, typeof(entryBaseType)))
                    {
                        valid_type = true;
                        break;
                    }
                    type = type.BaseType;
                    if (type == null)
                    {
                        break;
                    }
                }
                if (valid_type)
                {
                    if (_entries.ContainsKey(entry_type))
                    {
                        Log.Default.Msg(Log.Level.Warn, "entry type already added: " + entry_type.FullName);
                    }
                    else
                    {
                        _entries.Add(entry_type, new Dictionary<int, entryBaseType>());
                        Log.Default.Msg(Log.Level.Info, "Registered entry type: " + entry_type.FullName);
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible entry type: " + entry_type.FullName);
                }
            }

            /// <summary>
            /// Check recursively for base type.
            /// </summary>
            /// <param name="check_type">The type to look into.</param>
            /// <param name="reference_base_type">The base type to check for.</param>
            /// <returns>True on success, false otherwise.</returns>
            protected bool recursive_basetype(Type check_type, Type reference_base_type)
            {
                Type base_type = check_type;
                bool valid_base_type = false;
                while (base_type != typeof(object))
                {
                    if (base_type == reference_base_type)
                    {
                        valid_base_type = true;
                        break;
                    }
                    base_type = base_type.BaseType;
                    if (base_type == null)
                    {
                        break;
                    }
                }
                return valid_base_type;
            }

            /// <summary>
            /// Delete all entrys.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            protected bool reset_entry()
            {
                bool terminated = true;
                foreach (var entry_types in _entries)
                {
                    foreach (var entry_data in entry_types.Value)
                    {
                        terminated &= reset_entry(entry_data.Value);
                    }
                    entry_types.Value.Clear();
                }
                return terminated;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected Dictionary<Type, Dictionary<int, entryBaseType>> _entries = null;

            #endregion
        }
    }
}
