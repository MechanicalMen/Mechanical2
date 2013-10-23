using System;
using System.Text;

namespace Mechanical.Core
{
    /// <content>
    /// Methods extending the <see cref="String"/> type.
    /// </content>
    public static partial class CoreExtensions
    {
        #region NullOrEmpty

        /// <summary>
        /// Determines whether the string is <c>null</c> or empty.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns><c>true</c> if the specified string is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
        public static bool NullOrEmpty( this string str )
        {
            return string.IsNullOrEmpty(str);
        }

        #endregion

        #region NullOrLengthy

        /// <summary>
        /// Determines whether the string is <c>null</c>, empty, or if it has leading or trailing white-space characters.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns><c>true</c> if the specified string is <c>null</c>, empty or lengthy; otherwise, <c>false</c>.</returns>
        public static bool NullOrLengthy( this string str )
        {
            return string.IsNullOrEmpty(str)
                || char.IsWhiteSpace(str, 0)
                || char.IsWhiteSpace(str, str.Length - 1);
        }

        #endregion

        #region NullOrWhiteSpace

        /// <summary>
        /// Determines whether the string is <c>null</c>, empty, or if it consists only of white-space characters.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns><c>true</c> if the specified string is <c>null</c>, empty, or whitespace; otherwise, <c>false</c>.</returns>
        public static bool NullOrWhiteSpace( this string str )
        {
            return string.IsNullOrWhiteSpace(str);
        }

        #endregion

        #region Append

        /// <summary>
        /// Appends the specified <see cref="Substring"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="substr">The <see cref="Substring"/> to append.</param>
        /// <returns>A reference to <paramref name="sb"/> after the append operation has completed.</returns>
        public static StringBuilder Append( StringBuilder sb, Substring substr )
        {
            if( substr.NullOrEmpty )
                return sb;
            else
                return sb.Append(substr.Origin, substr.StartIndex, substr.Length);
        }

        #endregion
    }
}
