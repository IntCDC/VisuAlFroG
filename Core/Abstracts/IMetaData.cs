using System.ComponentModel;



/*
 * Interface for meta data
 * 
 */
namespace Core
{
    namespace Data
    {

        public interface IMetaData
        {

            /* ------------------------------------------------------------------*/
            // public events

            /// <summary>
            /// Event to indicated changed properties.
            /// </summary>
            event PropertyChangedEventHandler PropertyChanged;


            /* ------------------------------------------------------------------*/
            // public properties

            /// <summary>
            /// Index of the data point.
            /// </summary>
            int Index { get; set; }

            bool IsSelected { get; set; }

        }
    }
}
