using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.FileFormats;

namespace Mechanical.DataStores.File
{
    /// <summary>
    /// Data store related information about a file path.
    /// </summary>
    public struct FileDataStoreEntry
    {
        /// <summary>
        /// The (probably not data store compatible) string representing the file or directory.
        /// </summary>
        public readonly string RawPath;

        /// <summary>
        /// The data store compatible string representing the file or directory.
        /// </summary>
        public readonly string DataStorePath;

        /// <summary>
        /// <c>true</c> for files; <c>false</c> for directories.
        /// </summary>
        public readonly bool IsFile;

        /// <summary>
        /// <c>true</c> if the file stores binary data; otherwise, <c>false</c>. Ignored for directories.
        /// </summary>
        public readonly bool IsBinary;

        private FileDataStoreEntry( string rawPath, string dataStorePath, bool isFile, bool isBinary )
        {
            Ensure.Debug(rawPath, r => r.NotNullOrEmpty());
            Ensure.Debug(dataStorePath, a => a.NotNullOrEmpty());

            this.RawPath = rawPath;
            this.DataStorePath = dataStorePath;
            this.IsFile = isFile;
            this.IsBinary = isBinary;
        }

        /// <summary>
        /// Returns a new <see cref="FileDataStoreEntry"/> instance.
        /// </summary>
        /// <param name="rawPath">The (probably not data store compatible) string representing the file or directory.</param>
        /// <param name="dataStorePath">The data store compatible string representing the file or directory.</param>
        /// <param name="isBinary"><c>true</c> if the file stores binary data; otherwise, <c>false</c>.</param>
        /// <returns>A new <see cref="FileDataStoreEntry"/> instance.</returns>
        public static FileDataStoreEntry FromFile( string rawPath, string dataStorePath, bool isBinary )
        {
            return new FileDataStoreEntry(rawPath, dataStorePath, isFile: true, isBinary: isBinary);
        }

        /// <summary>
        /// Returns a new <see cref="FileDataStoreEntry"/> instance.
        /// </summary>
        /// <param name="rawPath">The (probably not data store compatible) string representing the file or directory.</param>
        /// <param name="dataStorePath">The data store compatible string representing the file or directory.</param>
        /// <returns>A new <see cref="FileDataStoreEntry"/> instance.</returns>
        public static FileDataStoreEntry FromDirectory( string rawPath, string dataStorePath )
        {
            return new FileDataStoreEntry(rawPath, dataStorePath, isFile: false, isBinary: default(bool));
        }

        #region CSV

        private const string RawPathHeader = "raw path";
        private const string DataStorePathHeader = "data store path";
        private const string IsFileHeader = "is file";
        private const string IsBinaryHeader = "is binary";
        private const string True = "t";
        private const string False = "f";

        /// <summary>
        /// Writes the headers of a file entry CSV stream.
        /// </summary>
        /// <param name="writer">The <see cref="CsvWriter"/> to use.</param>
        public static void WriteColumnHeaders( CsvWriter writer )
        {
            if( writer.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            writer.Write(RawPathHeader);
            writer.Write(DataStorePathHeader);
            writer.Write(IsFileHeader);
            writer.Write(IsBinaryHeader);
            writer.WriteLine();
        }

        /// <summary>
        /// Parses a line from a file entry CSV stream.
        /// </summary>
        /// <param name="reader">The <see cref="CsvReader"/> to use.</param>
        /// <returns>A new <see cref="FileDataStoreEntry"/> instance.</returns>
        public static FileDataStoreEntry FromCsvLine( CsvReader reader )
        {
            if( reader.NullReference() )
                throw new ArgumentNullException().StoreFileLine();

            try
            {
                if( reader.Record.Count != 4 )
                    throw new FormatException("Invalid number of cells!").Store("cellCount", reader.Record.Count);

                return new FileDataStoreEntry(
                    rawPath: reader.Record[0],
                    dataStorePath: reader.Record[1],
                    isFile: string.Equals(reader.Record[2].Trim(), True, StringComparison.InvariantCultureIgnoreCase),
                    isBinary: reader.Record[3].NullOrWhiteSpace() ? default(bool) : string.Equals(reader.Record[3].Trim(), True, StringComparison.InvariantCultureIgnoreCase));
            }
            catch( Exception ex )
            {
                ex.Store("Record", reader.Record);
                throw;
            }
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static string GetDataStorePath( string rawPath, string parentDataStorePath )
        {
            var escapedFileName = Path.GetFileName(rawPath);
            escapedFileName = DataStore.Escape(escapedFileName);
            if( !DataStore.IsValidName(escapedFileName) )
            {
                // can not generate data store name: too long
                return null;
            }
            else
                return DataStore.Combine(parentDataStorePath, escapedFileName);
        }

#if !MECHANICAL_NET4CP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsDirectory( string rawPath )
        {
            return (System.IO.File.GetAttributes(rawPath) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        private static void WriteFile( int numCharactersToCut, string rawPath, string parentDataStorePath, CsvWriter writer )
        {
            var dataStorePath = GetDataStorePath(rawPath, parentDataStorePath);
            if( dataStorePath.NullReference() )
                return;

            var fileExtension = Path.GetExtension(rawPath);
            bool isBinary = true;
            if( string.Equals(fileExtension, ".txt", StringComparison.InvariantCultureIgnoreCase) )
                isBinary = false;

            writer.Write(rawPath.Substring(startIndex: numCharactersToCut));
            writer.Write(dataStorePath);
            writer.Write(True); // IsFile
            writer.Write(isBinary ? True : False);
            writer.WriteLine();
        }

        private static void WriteDirectory( int numCharactersToCut, string rawPath, string parentDataStorePath, CsvWriter writer )
        {
            var dataStorePath = GetDataStorePath(rawPath, parentDataStorePath);
            if( dataStorePath.NullReference() )
                return;

            writer.Write(rawPath.Substring(startIndex: numCharactersToCut));
            writer.Write(dataStorePath);
            writer.Write(False); // IsFile
            writer.Write(); // IsBinary
            writer.WriteLine();

            foreach( string rawChildPath in Directory.EnumerateFileSystemEntries(rawPath) )
            {
                if( IsDirectory(rawChildPath) )
                    WriteDirectory(numCharactersToCut, rawChildPath, dataStorePath, writer);
                else
                    WriteFile(numCharactersToCut, rawChildPath, dataStorePath, writer);
            }
        }

        /// <summary>
        /// Generates a data store file entry CSV file. Results are unordered. File names too long are skipped silently.
        /// </summary>
        /// <param name="csvFilePath">The path to the CSV file to generate.</param>
        /// <param name="relativeRootPath">The file or directory representing the root node of the data store, relative to the CSV file; or <c>null</c> for an empty data store.</param>
        public static void CreateCsvFile( string csvFilePath, string relativeRootPath )
        {
            try
            {
                if( Path.IsPathRooted(relativeRootPath) )
                    throw new ArgumentException("The path to the root file or directory needs to be relative to the CSV file!");

                var csvDir = Path.GetDirectoryName(csvFilePath);
                int numCharactersToCut = csvDir.Length;
                if( csvDir[csvDir.Length - 1] != Path.DirectorySeparatorChar
                 && csvDir[csvDir.Length - 1] != Path.AltDirectorySeparatorChar
                 && csvDir[csvDir.Length - 1] != Path.VolumeSeparatorChar )
                    ++numCharactersToCut;

                string rootPath = null;
                if( !relativeRootPath.NullOrEmpty() )
                {
                    rootPath = Path.Combine(csvDir, relativeRootPath);
                    if( !Directory.Exists(rootPath)
                     && !System.IO.File.Exists(rootPath) )
                        throw new FileNotFoundException("Could not find root file or directory!");

                    // directory contents will have proper character casing
                    // but we need to make sure the root name does as well
                    // (do note, that only the name of the root will be checked, not it's whole path!)
                    var rootParentDir = Path.GetDirectoryName(rootPath);
                    var correctName = Directory.GetFileSystemEntries(rootParentDir, searchPattern: Path.GetFileName(rootPath), searchOption: SearchOption.TopDirectoryOnly)[0];
                    rootPath = Path.Combine(rootParentDir, correctName);
                }

                using( var sw = new StreamWriter(csvFilePath, append: false, encoding: Encoding.UTF8) )
                using( var writer = new CsvWriter(sw, CsvFormat.International) )
                {
                    WriteColumnHeaders(writer);

                    if( !relativeRootPath.NullOrEmpty() )
                    {
                        if( IsDirectory(rootPath) )
                            WriteDirectory(numCharactersToCut, rootPath, string.Empty, writer);
                        else
                            WriteFile(numCharactersToCut, rootPath, string.Empty, writer);
                    }
                }
            }
            catch( Exception ex )
            {
                ex.StoreFileLine();
                ex.Store("csvFilePath", csvFilePath);
                ex.Store("relativeRootPath", relativeRootPath);
                throw;
            }
        }

        #endregion
    }
}
