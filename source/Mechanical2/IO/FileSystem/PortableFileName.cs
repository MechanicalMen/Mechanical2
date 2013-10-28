using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mechanical.Core;

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

        #region POSIX portable file name character set

#if !MECHANICAL_NET4CP
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

        #endregion

        #region IsValid

        /// <summary>
        /// Determines whether the specified file name is portable.
        /// </summary>
        /// <param name="fileName">The file name to check.</param>
        /// <returns><c>true</c> if the file name is portable; otherwise, <c>false</c>.</returns>
        public static bool IsValid( string fileName )
        {
            // null or empty
            if( fileName.NullOrEmpty() )
                return false;

            // valid characters only
            foreach( char ch in fileName )
            {
                if( !IsValidPosixPortableFileNameCharacter(ch) )
                    return false;
            }

            // may not end in period on windows
            if( fileName[fileName.Length - 1] == '.' )
                return false;

            // current and parent directory names
            if( string.Equals(fileName, ".", StringComparison.Ordinal)
             || string.Equals(fileName, "..", StringComparison.Ordinal) )
                return false;

            // DOS device names, in any character case
            foreach( var dosDevName in DosDeviceNames )
            {
                if( string.Equals(fileName, dosDevName, StringComparison.OrdinalIgnoreCase) )
                    return false;
            }

            // DOS device names plus any extension
            int extensionAt = fileName.LastIndexOf('.'); // this would not be OK, if we were checking file paths, instead of file names!
            if( extensionAt != -1 )
            {
                fileName = fileName.Substring(startIndex: 0, length: extensionAt);
                foreach( var dosDevName in DosDeviceNames )
                {
                    if( string.Equals(fileName, dosDevName, StringComparison.OrdinalIgnoreCase) )
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
