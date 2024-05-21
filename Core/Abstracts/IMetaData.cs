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
            #region interface events

            /// <summary>
            /// Event to indicated changed properties.
            /// </summary>
            event PropertyChangedEventHandler PropertyChanged;

            #endregion

            /* ------------------------------------------------------------------*/
            #region interface properties

            /// <summary>
            /// Index of the data point.
            /// </summary>
            uint _Index { get; set; }

            bool _Selected { get; set; }

            #endregion
        }
    }
}
