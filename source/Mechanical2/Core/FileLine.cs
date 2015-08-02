using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mechanical.Core
{
    //// NOTE: code here needs to depend only on the .NET framework!

    /// <summary>
    /// Represents a line in a source file.
    /// Only the file name is stored (instead of the full path).
    /// </summary>
    public struct FileLine
    {
        #region Private Fields

        private readonly string file;
        private readonly string member;
        private readonly int line;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLine"/> struct.
        /// </summary>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public FileLine(
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            if( file.NullOrLengthy()
             || member.NullOrLengthy()
             || line < 0 )
                throw new ArgumentException(
                    string.Format( // NOTE: intentionally not using validation or SafeString
                        CultureInfo.InvariantCulture,
                        "Invalid arguments! (file: {0}; member: {1}; line: {2}",
                        file.NullReference() ? "<null>" : '"' + file + '"',
                        member.NullReference() ? "<null>" : '"' + member + '"',
                        line));

            this.file = ToFileName(file);
            this.member = member;
            this.line = line;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the source file that contains the caller.
        /// </summary>
        /// <value>The source file that contains the caller.</value>
        public string File
        {
            get { return this.file; }
        }

        /// <summary>
        /// Gets the name of the method or property, that the source code line implements.
        /// </summary>
        /// <value>The name of the method or property, that the source code line implements.</value>
        public string Member
        {
            get { return this.member; }
        }

        /// <summary>
        /// Gets the line in the source file that this instance points to.
        /// </summary>
        /// <value>The line in the source file that this instance points to.</value>
        public int Line
        {
            get { return this.line; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Appends the string representation of this instance to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
        public void ToString( StringBuilder sb )
        {
            if( sb.NullReference() )
                throw new ArgumentNullException("sb");

            sb.Append("   at ");
            sb.Append(string.IsNullOrEmpty(this.member) ? "?" : this.member);
            sb.Append(" in ");
            sb.Append(string.IsNullOrEmpty(this.file) ? "?" : this.file);
            sb.Append(":line ");
            sb.Append(this.line.ToString("D", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            const int InitialCapacity = 32 + 64 + 64; // 64 characters for file and member names, 32 for everything else
            var sb = new StringBuilder(InitialCapacity);
            this.ToString(sb);
            return sb.ToString();
        }

        #endregion

        #region Static Members

        private static readonly char[] DirectorySeparatorChars = new char[] { '\\', '/' };

        /// <summary>
        /// Shortens the full source file path to only the file name.
        /// </summary>
        /// <param name="filePath">The full source file path to shorten.</param>
        /// <returns>The source file name.</returns>
        public static string ToFileName( [CallerFilePath] string filePath = "" )
        {
            // let's not expose the developer's directory structure!
            // (may contain sensitive information, like user names, ... etc.)
            if( filePath.NotNullReference() )
            {
                // System.IO.Path expects the directory separators 
                // of the platform this code is being run on. But code may
                // have been compiled on a different platform! (e.g. building an app on Windows, and running it on Android)
                int directorySeparatorAt = filePath.LastIndexOfAny(DirectorySeparatorChars);
                if( directorySeparatorAt != -1 )
                {
                    filePath = filePath.Substring(startIndex: directorySeparatorAt + 1);
                }
                else
                {
                    //// no directory separator?
                    //// only if this string was not generated by the compiler!
                    //// (either it was provided by the user, or it was already shortened)
                    //// Let the user deal with it.
                }
            }

            return filePath;
        }

        #endregion
    }
}
