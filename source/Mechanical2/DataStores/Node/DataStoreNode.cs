using System;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.DataStores.Node
{
    /// <summary>
    /// A base class implementing <see cref="IDataStoreNode"/>.
    /// </summary>
    public abstract class DataStoreNode : IDataStoreNode
    {
        #region Private Fields

        private readonly string name;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreNode"/> class.
        /// </summary>
        /// <param name="name">The name of the data store node.</param>
        protected DataStoreNode( string name )
        {
            Ensure.That(DataStore.IsValidName(name)).IsTrue(() => new ArgumentException("Invalid data store name!").Store("Name", name));

            this.name = name;
        }

        #endregion

        #region IDataStoreNode

        /// <summary>
        /// Gets the name of the data store node.
        /// </summary>
        /// <value>The name of the data store node.</value>
        public string Name
        {
            get { return this.name; }
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public /*virtual*/ bool Equals( IDataStoreNode other )
        {
            /*
            // in case we're overriding
            if( !base.Equals( other ) )
                return false;
            */

            // might not need this, if the base has checked it (or if 'other' is a value type)
            if( other.NullReference() )
                return false;


            if( !DataStore.Comparer.Equals(this.Name, other.Name) )
                return false;

            var value = this as IDataStoreValue;
            var otherValue = other as IDataStoreValue;
            if( value.NullReference()
             && otherValue.NullReference() )
            {
                // both must be objects
                var obj = (IDataStoreObject)this;
                var otherObj = (IDataStoreObject)other;

                if( obj.Nodes.Count != otherObj.Nodes.Count )
                    return false;

                for( int i = 0; i < obj.Nodes.Count; ++i )
                {
                    if( !obj.Nodes[i].Equals(otherObj.Nodes[i]) )
                        return false;
                }

                return true;
            }
            else
            {
                // both are values
                var binary = value as IDataStoreBinaryValue;
                var otherBinary = otherValue as IDataStoreBinaryValue;
                if( binary.NotNullReference()
                 && otherBinary.NotNullReference() )
                {
                    // both are binary values
                    if( binary.Content.Count != otherBinary.Content.Count )
                        return false;

                    for( int i = 0; i < binary.Content.Count; ++i )
                    {
                        if( binary.Content.Array[binary.Content.Offset + i] != otherBinary.Content.Array[otherBinary.Content.Offset + i] )
                            return false;
                    }

                    return true;
                }
                else
                {
                    var text = value as IDataStoreTextValue;
                    var otherText = otherValue as IDataStoreTextValue;
                    if( text.NotNullReference()
                     && otherText.NotNullReference() )
                    {
                        // both are text values
                        return text.Content.Equals(otherText.Content, System.Globalization.CompareOptions.Ordinal, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid type!").Store("type", this.GetType()).Store("otherType", other.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( DataStoreNode left, IDataStoreNode right )
        {
            if( object.ReferenceEquals(left, right) )
                return true;

            if( left.NullReference() )
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( DataStoreNode left, IDataStoreNode right )
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( IDataStoreNode left, DataStoreNode right )
        {
            return right == left;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( IDataStoreNode left, DataStoreNode right )
        {
            return !(right == left);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public override bool Equals( object other )
        {
            // for reference types
            var asNode = other as IDataStoreNode;

            if( asNode.NullReference() )
                return false;
            else
                return this.Equals(asNode);

            // for value types
            /*if( other is ValueType )
                return this.Equals((ValueType)other);
            else
                return false;*/
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 0;
            var value = this as IDataStoreValue;
            if( value.NullReference() )
            {
                var obj = (IDataStoreObject)this;
                hash = obj.Nodes.Count;

                for( int i = 0; i < obj.Nodes.Count; ++i )
                    hash ^= obj.Nodes[i].GetHashCode();
            }
            else
            {
                var binary = value as IDataStoreBinaryValue;
                if( binary.NotNullReference() )
                {
                    hash = binary.Content.Count;

                    for( int i = 0; i < binary.Content.Count; ++i )
                        hash ^= binary.Content.Array[binary.Content.Offset + i].GetHashCode();
                }
                else
                {
                    var text = value as IDataStoreTextValue;
                    if( text.NotNullReference() )
                        hash = text.ToString().GetHashCode();
                    else
                        throw new ArgumentException("Invalid type!").Store("type", this.GetType());
                }
            }

            return this.Name.GetHashCode() ^ hash;
        }

        #endregion


        #region MagicBag mappings

        internal static Mapping[] GetMappings()
        {
            var valueSerializer = new DataStoreValueSerializer(maxLength: -1);
            var objectSerializer = new DataStoreObjectSerializer(valueSerializer);

            return new Mapping[]
            {
                Map<IDataStoreValueSerializer<IDataStoreValue>>.To(() => valueSerializer).AsTransient(),
                Map<IDataStoreValueDeserializer<IDataStoreValue>>.To(() => valueSerializer).AsTransient(),
                Map<IDataStoreObjectSerializer<IDataStoreObject>>.To(() => objectSerializer).AsTransient(),
                Map<IDataStoreObjectDeserializer<IDataStoreObject>>.To(() => objectSerializer).AsTransient(),
            };
        }

        #endregion
    }

    //// TODO: better GetHashCode, through better Substring.GetHashCode (through IEquatable support for substring?)
}
