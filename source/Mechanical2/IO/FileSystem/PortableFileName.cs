using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.DataStores;

namespace Mechanical.IO.FileSystem
{
    /// <summary>
    /// Validates non data store compatible, but portable file names.
    /// </summary>
    public static class PortableFileName
    {
        #region DOS device names

        private static readonly string[] DosDeviceNames = new string[]
        {
            "CON",
            "PRN",
            "AUX",
            "NUL"

            // "CLOCK$", // we don't allow that character anyways. NTFS reserved names are allowed for the same reason
        };

        static PortableFileName()
        {
            var strings = new List<string>();
            strings.AddRange(DosDeviceNames);
            for( int i = 1; i <= 9; ++i )
            {
                strings.Add("COM" + i.ToString(CultureInfo.InvariantCulture));
                strings.Add("LPT" + i.ToString(CultureInfo.InvariantCulture));
            }
            DosDeviceNames = strings.ToArray();
        }

        #endregion

        #region Character Tests

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidPosixPortableFileNameCharacter( char ch )
        {
            return ('A' <= ch && ch <= 'Z')
                || ('a' <= ch && ch <= 'z')
                || ('0' <= ch && ch <= '9')
                || ch == '.'
                || ch == '_'
                || ch == '-';
        }

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool InvalidWindowsCharacter( char ch )
        {
            return (0 <= ch && ch <= 31) // control characters
                || ch == 127 // delete control character
                || ch == '\\'
                || ch == '/'
                || ch == ':'
                || ch == '|'
                || ch == '?'
                || ch == '*'
                || ch == '"'
                || ch == '<'
                || ch == '>';
        }

        #endregion

        #region IsValid*

#if !MECHANICAL_NET4
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool IsValidWindowsNameCore( Substring fileName )
        {
            Substring trimmedFileName = fileName.TrimEnd();

            // may not end in period on windows
            // (even if period is followed by whitespaces)
            if( trimmedFileName[fileName.Length - 1] == '.' )
                return false;

            // current and parent directory names
            // the step above accounts for this
            /*if( trimmedFileName.Equals(".", CompareOptions.Ordinal)
             || trimmedFileName.Equals("..", CompareOptions.Ordinal) )
                return false;*/

            // DOS device names, in any character case
            foreach( var dosDevName in DosDeviceNames )
            {
                if( fileName.Equals(dosDevName, CompareOptions.OrdinalIgnoreCase) )
                    return false;
            }

            // DOS device names plus any extension
            int extensionAt = fileName.LastIndexOf('.'); // this would not be OK, if we were checking file paths, instead of file names!
            if( extensionAt != -1 )
            {
                fileName = fileName.Substr(startIndex: 0, length: extensionAt);
                foreach( var dosDevName in DosDeviceNames )
                {
                    if( fileName.Equals(dosDevName, CompareOptions.OrdinalIgnoreCase) )
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is a valid windows file name.
        /// </summary>
        /// <param name="fileName">The file name to check.</param>
        /// <returns><c>true</c> if the parameter is a valid windows file name; otherwise, <c>false</c>.</returns>
        public static bool IsWindowsName( Substring fileName )
        {
            // null or empty
            if( fileName.NullOrEmpty )
                return false;

            // valid characters only
            for( int i = 0; i < fileName.Length; ++i )
            {
                if( InvalidWindowsCharacter(fileName[i]) )
                    return false;
            }

            return IsValidWindowsNameCore(fileName);
        }

        /// <summary>
        /// Determines whether the specified file name is portable.
        /// </summary>
        /// <param name="fileName">The file name to check.</param>
        /// <returns><c>true</c> if the file name is portable; otherwise, <c>false</c>.</returns>
        public static bool IsValidName( Substring fileName )
        {
            // null or empty
            if( fileName.NullOrEmpty )
                return false;

            // valid characters only
            for( int i = 0; i < fileName.Length; ++i )
            {
                if( !IsValidPosixPortableFileNameCharacter(fileName[i]) )
                    return false;
            }

            return IsValidWindowsNameCore(fileName);
        }

        /// <summary>
        /// Determines whether the specified file path is portable.
        /// The directory separator is <see cref="DataStore.PathSeparator"/>.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns><c>true</c> if the file path is portable; otherwise, <c>false</c>.</returns>
        public static bool IsValidPath( string filePath )
        {
            if( filePath.NullOrEmpty() )
                return false;

            Substring remainingPath = filePath;
            Substring name;
            do
            {
                name = Substring.SplitFirst(ref remainingPath, DataStore.PathSeparatorArray, StringSplitOptions.None);
                if( !IsValidName(name) )
                    return false;
            }
            while( !remainingPath.NullOrEmpty );

            return true;
        }

        #endregion

        #region ToValidDataStorePath

        /// <summary>
        /// Converts the relative file path to a valid data store path.
        /// The directory separator is <see cref="DataStore.PathSeparator"/>.
        /// </summary>
        /// <param name="filePath">The relative file path to convert to a valid data store path; where the directory separator is <see cref="DataStore.PathSeparator"/>.</param>
        /// <returns>The data store path created.</returns>
        public static string ToDataStorePath( string filePath )
        {
            if( !IsValidPath(filePath) )
                throw new ArgumentException("The specified file path is not portable!").Store("filePath", filePath);

            var dataStorePath = DataStore.EscapePath(filePath);
            if( !DataStore.IsValidPath(dataStorePath) )
                throw new ArgumentException("The specified file path is too long!").Store("filePath", filePath);

            return dataStorePath;
        }

        #endregion

        /// <summary>
        /// The comparer to use for portable file name equality.
        /// Result is undetermined for invalid names.
        /// </summary>
        public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
    }
}
