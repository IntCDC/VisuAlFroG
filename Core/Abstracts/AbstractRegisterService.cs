using System;
using System.Collections.Generic;

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
        public abstract class AbstractRegisterService<ContentBaseType> : AbstractService
        {

            /* ------------------------------------------------------------------*/
            #region public  functions

            public override bool Initialize()
            {
                if (_initialized)
                {
                    Terminate();
                }

                _contents = new Dictionary<Type, Dictionary<string, ContentBaseType>>();

                return true;
            }

            public override bool Terminate()
            {
                clear_contents();

                return true;
            }

            #endregion


            /* ------------------------------------------------------------------*/
            #region abstract functions

            protected abstract bool reset_content(ContentBaseType content_value);

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected functions

            /// <summary>
            /// Register new content type.
            /// </summary>
            /// <param name="content_type">The content type.</param>
            protected void register_content(Type content_type)
            {
                // Check for required base type
                Type type = content_type;
                bool valid_type = false;
                while (!recursive_basetype(type, typeof(object)))
                {
                    if (recursive_basetype(type, typeof(ContentBaseType)))
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
                    if (_contents.ContainsKey(content_type))
                    {
                        Log.Default.Msg(Log.Level.Warn, "Content type already added: " + content_type.FullName);
                    }
                    else
                    {
                        _contents.Add(content_type, new Dictionary<string, ContentBaseType>());
                        Log.Default.Msg(Log.Level.Info, "Registered content type: " + content_type.FullName);
                    }
                }
                else
                {
                    Log.Default.Msg(Log.Level.Error, "Incompatible content type: " + content_type.FullName);
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
            /// Delete all contents.
            /// </summary>
            /// <returns>True on success, false otherwise.</returns>
            protected bool clear_contents()
            {
                bool terminated = true;
                foreach (var content_types in _contents)
                {
                    foreach (var content_data in content_types.Value)
                    {
                        terminated &= reset_content(content_data.Value);
                    }
                    content_types.Value.Clear();
                }
                return terminated;
            }

            #endregion

            /* ------------------------------------------------------------------*/
            #region protected variables

            protected Dictionary<Type, Dictionary<string, ContentBaseType>> _contents = null;

            #endregion
        }
    }
}
