using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Core.Utilities;



/*
 * Abstract Service
 * 
 */
namespace Core
{
    namespace Abstracts
    {

        // INTERFACE
        public interface IAbstractService
        {
            /* ------------------------------------------------------------------*/
            // interface functions

            bool Initialize();

            bool Execute();

            bool Terminate();
        }



        // ABSTRACT CLASS
        public abstract class AbstractService : IAbstractService 
        {

            /* ------------------------------------------------------------------*/
            // abstract functions

            public abstract bool Initialize();


            public abstract bool Execute();


            public abstract bool Terminate();


            /* ------------------------------------------------------------------*/
            // protected variables

            protected bool _initilized = false;

            /// DEBUG
            protected TimeBenchmark _timer = new TimeBenchmark();
        }
    }
}
