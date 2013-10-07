using System;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.DataStores.File
{
    //// NOTE: the path used by the file data store implementation (filesystem, zip, ... etc.)
    ////       mirrors the data store path (though they may differ in individual identifiers
    ////       and the directory separator, they will have the same parent-child relationships).
    ////
    ////       We do this to make sure, that if a parent object was added,
    ////       the necessary resources (directory, zip entry, ...)
    ////       are available for it's children.

    /// <summary>
    /// A file data store entry.
    /// </summary>
    public struct FileDataStoreEntry : IEquatable<FileDataStoreEntry>
    {
        /// <summary>
        /// The type of data store node, this entry represents.
        /// </summary>
        public readonly DataStoreToken Token;

        /// <summary>
        /// The data store compatible string representing the file or directory.
        /// </summary>
        public readonly string DataStorePath;

        /// <summary>
        /// The alternative name of the file (or directory), or <c>null</c> if it is the same as the data store name.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreEntry"/> struct.
        /// </summary>
        /// <param name="token">The type of data store node, this entry represents.</param>
        /// <param name="dataStorePath">The data store compatible string representing the file or directory.</param>
        /// <param name="fileName">The alternative name of the file (or directory), or <c>null</c> if it is the same as the data store name.</param>
        public FileDataStoreEntry( DataStoreToken token, string dataStorePath, string fileName = null )
        {
            if( token == DataStoreToken.BinaryValue
             || token == DataStoreToken.TextValue
             || token == DataStoreToken.ObjectStart )
                this.Token = token;
            else
                throw new ArgumentException("Invalid token!").Store("token", token).Store("dataStorePath", dataStorePath).Store("fileName", fileName);

            if( dataStorePath.NullOrLengthy() )
                throw new ArgumentException().Store("token", token).Store("dataStorePath", dataStorePath).Store("fileName", fileName);
            else
                this.DataStorePath = dataStorePath;

            if( fileName.NullOrEmpty()
             || DataStore.Comparer.Equals(fileName, DataStore.GetNodeName(dataStorePath)) )
                this.FileName = null;
            else
                this.FileName = fileName;
        }

        /// <summary>
        /// Returns the string representation of this entry.
        /// </summary>
        /// <returns>The string representation of this entry.</returns>
        public override string ToString()
        {
            return SafeString.DebugFormat("{0}; {1}; {2}", this.Token, this.DataStorePath, this.FileName);
        }

        #region IEquatable

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public /*virtual*/ bool Equals( FileDataStoreEntry other )
        {
            // in case we're overriding
            /*if( !base.Equals( other ) )
                return false;*/

            // might not need this, if the base has checked it (or if 'other' is a value type)
            /*if( other.NullReference() )
                return false;*/

            // NOTE: since DataStore.Comparer is currently case-sensitive, this is not strictly correct for Windows.
            //       However: since portability is a requirement, this might indicate to the user, that the file
            //       names they use are not portable!
            return this.Token == other.Token
                && DataStore.Comparer.Equals(this.DataStorePath, other.DataStorePath)
                && DataStore.Comparer.Equals(this.FileName, other.FileName);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( FileDataStoreEntry left, FileDataStoreEntry right )
        {
            /*if( object.ReferenceEquals(left, right) )
                return true;

            if( left.NullReference() )
                return false;*/

            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( FileDataStoreEntry left, FileDataStoreEntry right )
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public override bool Equals( object other )
        {
            // for reference types
            /*var asNode = other as IDataStoreNode;

            if( asNode.NullReference() )
                return false;
            else
                return this.Equals(asNode);*/

            // for value types
            if( other is FileDataStoreEntry )
                return this.Equals((FileDataStoreEntry)other);
            else
                return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // NOTE: the data store path may be null, if we were instantiated through default()
            //       and the file name may be null anyways.
            return this.Token.GetHashCode()
                 ^ (this.DataStorePath.NullReference() ? 0 : this.DataStorePath.GetHashCode())
                 ^ (this.FileName.NullReference() ? 0 : this.FileName.GetHashCode());
        }

        #endregion
    }
}
