using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.Nodes
{
    /// <summary>
    /// A text based data store value.
    /// </summary>
    public class DataStoreTextValue : DataStoreNode, IDataStoreTextValue
    {
        #region Private Fields

        private Substring content;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreTextValue"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        /// <param name="content">The content of the data store value.</param>
        public DataStoreTextValue( string name, Substring content )
            : base(name)
        {
            this.Content = content;
        }

        #endregion

        #region IDataStoreTextValue

        /// <summary>
        /// Gets or sets the content of the data store value.
        /// </summary>
        /// <value>The content of the data store value.</value>
        public Substring Content
        {
            get
            {
                return this.content;
            }
            set
            {
                if( value.Origin.NullReference() )
                    throw new ArgumentNullException().StoreFileLine();

                this.content = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the content of the data store value.
        /// </summary>
        /// <returns>The content of the data store value.</returns>
        public override string ToString()
        {
            return this.content.ToString();
        }

        #endregion
    }
}
