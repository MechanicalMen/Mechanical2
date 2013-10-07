using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.FileFormats;

namespace Mechanical.DataStores.File
{
    //// NOTE: We only allow overwriting with the same token!
    ////       The reason for this is, that otherwise we would have
    ////       to handle the case, where an object would be replaced
    ////       by a value, which would force us to remove the subtree
    ////       which would force us to implement a Delete method
    ////       in file data stores, as well as rebuild the entry index dictionary...

    //// NOTE: Therefore, each operation should either expand the list, or keep it as it is.

    /// <summary>
    /// Handles a collection of file data store entries.
    /// </summary>
    public class FileDataStoreEntryList : ReadOnlyList.Wrapper<FileDataStoreEntry>
    {
        #region Private Fields

        private const string TokenHeader = "type";
        private const string DataStorePathHeader = "data store path";
        private const string FileNameHeader = "file name";
        private const string BinaryValueToken = "b";
        private const string TextValueToken = "t";
        private const string ObjectStartToken = "o";

        private readonly Dictionary<string, int> entriesByDataStorePath;

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStoreEntryList"/> class.
        /// </summary>
        public FileDataStoreEntryList()
            : base()
        {
            this.entriesByDataStorePath = new Dictionary<string, int>(DataStore.Comparer);
        }

        #endregion

        #region Private Methods

        private void BuildFilePath( StringBuilder sb, string dataStorePath, char directorySeparator )
        {
            var entryIndex = this.entriesByDataStorePath[dataStorePath];
            var entry = this.Items[entryIndex];
            var parentPath = DataStore.GetParentPath(entry.DataStorePath);

            if( !parentPath.NullOrEmpty() )
            {
                this.BuildFilePath(sb, parentPath, directorySeparator);
                sb.Append(directorySeparator);
            }

            if( entry.FileName.NullReference() )
                sb.Append(DataStore.GetNodeName(entry.DataStorePath));
            else
                sb.Append(entry.FileName);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the entry with the specified data store path.
        /// </summary>
        /// <param name="dataStorePath">The data store path of the entry to return.</param>
        /// <returns>The file data store entry found.</returns>
        public FileDataStoreEntry this[string dataStorePath]
        {
            get
            {
                int entryIndex;
                if( !this.entriesByDataStorePath.TryGetValue(dataStorePath, out entryIndex) )
                    throw new KeyNotFoundException().Store("dataStorePath", dataStorePath);
                else
                    return this.Items[entryIndex];
            }
        }

        /// <summary>
        /// Adds the specified <see cref="FileDataStoreEntry"/> to the end of the list.
        /// Does not check whether it actually belongs there.
        /// </summary>
        /// <param name="entry">The <see cref="FileDataStoreEntry"/> to add.</param>
        public void AddToEnd( FileDataStoreEntry entry )
        {
            if( entry.DataStorePath.NullReference() ) // created through default()
                throw new ArgumentException("Invalid entry!").Store("entry", entry);

            if( this.entriesByDataStorePath.ContainsKey(entry.DataStorePath) )
                throw new ArgumentException("Data store path already in use!").Store("entry", entry);

            this.entriesByDataStorePath.Add(entry.DataStorePath, this.Items.Count);
            this.Items.Add(entry);
        }

        /// <summary>
        /// Adds the specified <see cref="FileDataStoreEntry"/> as a child of it's parent data store object.
        /// If the entry already exists, then it throws an exception, if it it not the same as the one being added.
        /// </summary>
        /// <param name="entry">The <see cref="FileDataStoreEntry"/> to add.</param>
        /// <returns><c>true</c> if a new entry was created; otherwise <c>false</c>.</returns>
        public bool AddToParent( FileDataStoreEntry entry )
        {
            try
            {
                if( entry.DataStorePath.NullReference() ) // created through default()
                    throw new ArgumentException("Invalid entry!").StoreFileLine();

                if( this.entriesByDataStorePath.ContainsKey(entry.DataStorePath) )
                    throw new ArgumentException("Data store path already in use!").StoreFileLine();

                int entryIndex;
                var parentPath = DataStore.GetParentPath(entry.DataStorePath);
                if( parentPath.NullOrEmpty() )
                {
                    //// we're adding a root node
                    entryIndex = 0;
                }
                else
                {
                    //// this is not a root node

                    int parentEntryIndex;
                    if( !this.entriesByDataStorePath.TryGetValue(parentPath, out parentEntryIndex) )
                        throw new KeyNotFoundException("Parent data store object not found! Please add it first.").StoreFileLine();

                    entryIndex = parentEntryIndex + this.GetNumDescendants(parentEntryIndex) + 1;
                }

                if( entryIndex < this.Items.Count )
                {
                    // entry already present: make sure it's the same one
                    if( !entry.Equals(this.Items[entryIndex]) )
                        throw new ArgumentException("Entry already present, but it is not the same, as the one being added!").Store("entryAlreadyPresent", this.Items[entryIndex]);

                    return false;
                }
                else
                {
                    // this is a new child
                    this.Items.Insert(entryIndex, entry);
                    this.entriesByDataStorePath.Add(entry.DataStorePath, entryIndex);

                    return true;
                }
            }
            catch( Exception ex )
            {
                ex.Store("entry", entry);
                throw;
            }
        }

        /// <summary>
        /// Gets the number of direct children, the specified entry has.
        /// </summary>
        /// <param name="entryIndex">The index of the entry, to count the children of.</param>
        /// <returns>The number of direct children found.</returns>
        public int GetNumDirectChildren( int entryIndex )
        {
            var entry = this.Items[entryIndex];
            if( entry.Token != DataStoreToken.ObjectStart )
                return 0;

            int num = 0;
            for( int i = entryIndex + 1; i < this.Items.Count; ++i )
            {
                if( DataStore.Comparer.Equals(entry.DataStorePath, DataStore.GetParentPath(this.Items[i].DataStorePath)) )
                    ++num;

                //// TODO: write DataStore.Comparer.StartsWith, and use it to optimize this
            }
            return num;
        }

        /// <summary>
        /// Gets the number of descendants, the specified entry has.
        /// </summary>
        /// <param name="entryIndex">The index of the entry, to count the descendants of.</param>
        /// <returns>The number of descendants found.</returns>
        public int GetNumDescendants( int entryIndex )
        {
            var entry = this.Items[entryIndex];
            if( entry.Token != DataStoreToken.ObjectStart )
                return 0;

            int num = 0;
            for( int i = entryIndex + 1; i < this.Items.Count; ++i )
            {
                if( this.Items[i].DataStorePath.StartsWith(entry.DataStorePath + DataStore.PathSeparator, StringComparison.Ordinal) ) // TODO: replace this with DataStore.Comparer.StartsWith
                    ++num;

                //// TODO: write DataStore.Comparer.StartsWith, and use it to optimize this
            }
            return num;
        }

        /// <summary>
        /// Returns an altered data store path: data store names are replaced with file names
        /// (when available) and the specified separator is used.
        /// </summary>
        /// <param name="entry">The entry, to base the path on.</param>
        /// <param name="directorySeparator">The directory separator to use.</param>
        /// <returns>The path string created.</returns>
        public string BuildFilePath( FileDataStoreEntry entry, char directorySeparator )
        {
            int entryIndex;
            if( !this.entriesByDataStorePath.TryGetValue(entry.DataStorePath, out entryIndex) )
                throw new KeyNotFoundException("Entry not registered!").Store("entryDataStorePath", entry.DataStorePath);

            return this.BuildFilePath(entryIndex, directorySeparator);
        }

        /// <summary>
        /// Returns an altered data store path: data store names are replaced with file names
        /// (when available) and the specified separator is used.
        /// </summary>
        /// <param name="entryIndex">The index of the entry, to base the path on.</param>
        /// <param name="directorySeparator">The directory separator to use.</param>
        /// <returns>The path string created.</returns>
        public string BuildFilePath( int entryIndex, char directorySeparator )
        {
            var entry = this.Items[entryIndex];
            var sb = new StringBuilder();
            this.BuildFilePath(sb, entry.DataStorePath, directorySeparator);
            return sb.ToString();
        }

        #endregion

        #region CSV

        /// <summary>
        /// Saves all entries of this collection to the specified <see cref="CsvWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="CsvWriter"/> to use.</param>
        public void SaveTo( CsvWriter writer )
        {
            Ensure.That(writer).NotNull();

            writer.Write(TokenHeader);
            writer.Write(DataStorePathHeader);
            writer.Write(FileNameHeader);
            writer.WriteLine();

            FileDataStoreEntry entry;
            for( int i = 0; i < this.Items.Count; ++i )
            {
                entry = this.Items[i];

                if( entry.Token == DataStoreToken.BinaryValue )
                    writer.Write(BinaryValueToken);
                else if( entry.Token == DataStoreToken.TextValue )
                    writer.Write(TextValueToken);
                else
                    writer.Write(ObjectStartToken);

                writer.Write(entry.DataStorePath);
                writer.Write(entry.FileName);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Loads all entries of this collection from the specified <see cref="CsvReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="CsvReader"/> to use.</param>
        /// <returns>A new <see cref="FileDataStoreEntryList"/> instance.</returns>
        public static FileDataStoreEntryList LoadFrom( CsvReader reader )
        {
            Ensure.That(reader).NotNull();

            // skip column headers
            reader.Read();

            var list = new FileDataStoreEntryList();
            string token;
            while( reader.Read() )
            {
                if( reader.Record.Count == 0 )
                    continue;
                else if( reader.Record.Count != 3 )
                    throw new FormatException("Invalid number of cells!").Store("cellCount", reader.Record.Count);

                token = reader.Record[0].Trim();
                if( string.Equals(token, BinaryValueToken, StringComparison.InvariantCultureIgnoreCase) )
                    list.AddToEnd(new FileDataStoreEntry(DataStoreToken.BinaryValue, reader.Record[1], reader.Record[2]));
                else if( string.Equals(token, TextValueToken, StringComparison.InvariantCultureIgnoreCase) )
                    list.AddToEnd(new FileDataStoreEntry(DataStoreToken.TextValue, reader.Record[1], reader.Record[2]));
                else if( string.Equals(token, ObjectStartToken, StringComparison.InvariantCultureIgnoreCase) )
                    list.AddToEnd(new FileDataStoreEntry(DataStoreToken.ObjectStart, reader.Record[1], reader.Record[2]));
                else
                    throw new FormatException(SafeString.DebugFormat("Unrecognized value in column: {0}!", TokenHeader)).Store("Record", reader.Record);
            }

            return list;
        }

        #endregion

        #region GenerateFromDirectory

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static string GetDataStorePath( string fileOrDirectoryPath, string parentDataStorePath )
        {
            var fileName = Path.GetFileName(fileOrDirectoryPath);
            var dataStoreName = DataStore.GenerateName(fileName);
            return DataStore.Combine(parentDataStorePath, dataStoreName);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsDirectory( string rawPath )
        {
            return (System.IO.File.GetAttributes(rawPath) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private static void AddFile( FileDataStoreEntryList list, string filePath, string parentDataStorePath )
        {
            var dataStorePath = GetDataStorePath(filePath, parentDataStorePath);
            var fileExtension = Path.GetExtension(filePath);
            bool isBinary = true;
            if( string.Equals(fileExtension, ".txt", StringComparison.InvariantCultureIgnoreCase) )
                isBinary = false;

            list.AddToEnd(new FileDataStoreEntry(isBinary ? DataStoreToken.BinaryValue : DataStoreToken.TextValue, dataStorePath, Path.GetFileName(filePath)));
        }

        private static void AddDirectory( FileDataStoreEntryList list, string directoryPath, string parentDataStorePath )
        {
            var dataStorePath = GetDataStorePath(directoryPath, parentDataStorePath);
            list.AddToEnd(new FileDataStoreEntry(DataStoreToken.ObjectStart, dataStorePath, Path.GetFileName(directoryPath)));

            foreach( string rawChildPath in Directory.EnumerateFileSystemEntries(directoryPath) )
            {
                if( IsDirectory(rawChildPath) )
                    AddDirectory(list, rawChildPath, dataStorePath);
                else
                    AddFile(list, rawChildPath, dataStorePath);
            }
        }

        /// <summary>
        /// Generates a data store file entry list, by enumerating the contents of the specified directory.
        /// </summary>
        /// <param name="fileOrDirectoryPath">The path to the file or directory, that represents the root data store node of the file data store.</param>
        /// <returns>A new <see cref="FileDataStoreEntryList"/> instance.</returns>
        public static FileDataStoreEntryList Generate( string fileOrDirectoryPath )
        {
            try
            {
                if( !Directory.Exists(fileOrDirectoryPath)
                 && !System.IO.File.Exists(fileOrDirectoryPath) )
                    throw new FileNotFoundException("Could not find root file or directory!").StoreFileLine();

                // enumerated directory contents will have proper character casing
                // but we need to make sure the root name does as well.
                // (do note, that only the name of the root will be checked, not it's whole path!)
                var rootParentDir = Path.GetDirectoryName(fileOrDirectoryPath);
                var correctRootName = Directory.GetFileSystemEntries(rootParentDir, searchPattern: Path.GetFileName(fileOrDirectoryPath), searchOption: SearchOption.TopDirectoryOnly)[0];
                fileOrDirectoryPath = Path.Combine(rootParentDir, correctRootName);

                var list = new FileDataStoreEntryList();
                if( IsDirectory(fileOrDirectoryPath) )
                    AddDirectory(list, fileOrDirectoryPath, string.Empty);
                else
                    AddFile(list, fileOrDirectoryPath, string.Empty);
                return list;
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("fileOrDirectoryPath", fileOrDirectoryPath);
                throw;
            }
        }

        #endregion

        //// TODO: validate CSV file: raw path character casing
        //// TODO: validate CSV file: entry order (no child precedes it's parent, or succeeds it's parent's next sibling)
    }
}
