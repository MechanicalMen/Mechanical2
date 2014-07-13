using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <summary>
    /// An immutable string pattern. Basically a very inflexible, but easy to learn and use <see cref="Regex"/> replacement.
    /// Special symbols are: * ? ! (these can also be escaped: \* \? \! \\).
    /// Matching ignores character casing.
    /// </summary>
    public static class StringPattern
    {
        #region Private Static Fields

        private static Dictionary<string, Regex> currentCulturePatterns = new Dictionary<string, Regex>(StringComparer.CurrentCultureIgnoreCase);
        private static Dictionary<string, Regex> invariantCulturePatterns = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Private Static Methods

        private static Regex CreateRegex( string pattern, bool invariantCulture, bool compile )
        {
            //// NOTE: to use a specific culture, we would need to set Thread.CurrentThread.CurrentCulture
            ////       (and maybe not just here but when doing the actual matching as well?)

            var options = RegexOptions.IgnoreCase;
            if( invariantCulture )
                options |= RegexOptions.CultureInvariant;
#if !SILVERLIGHT
            if( compile )
                options |= RegexOptions.Compiled;
#endif
            return new Regex(GenerateRegexPattern(pattern), options);
        }

        private static Regex AddOrGetRegex( string pattern, bool invariantCulture )
        {
            Regex regex;
            if( invariantCulture )
            {
                lock( invariantCulturePatterns ) // Regex is thread-safe, but Dictionary is not
                {
                    if( invariantCulturePatterns.TryGetValue(pattern, out regex) )
                    {
                        regex = CreateRegex(pattern, invariantCulture, compile: true);
                        invariantCulturePatterns.Add(pattern, regex);
                    }
                }
            }
            else
            {
                lock( currentCulturePatterns ) // Regex is thread-safe, but Dictionary is not
                {
                    if( currentCulturePatterns.TryGetValue(pattern, out regex) )
                    {
                        regex = CreateRegex(pattern, invariantCulture, compile: true);
                        currentCulturePatterns.Add(pattern, regex);
                    }
                }
            }

            return regex;
        }

        private static string GenerateRegexPattern( string pattern )
        {
            var sb = new StringBuilder();
            sb.Append('^');

            // first we make sure all characters special to regular expressions are escaped
            // unfortunately this escapes our special characters as well,
            // so @"*" becomes @"\*", and @"\?" becomes @"\\\?"
            // however, non-special escaped characters are not changed, e.g.: @"\z" remains @"\z"
            var escapedPattern = Regex.Escape(pattern);

            char ch;
            for( int i = 0; i < escapedPattern.Length; ++i )
            {
                ch = escapedPattern[i];
                switch( ch )
                {
                case '\\':
                    // lone backslash characters are always escaped,
                    // and valid escape sequences already contain more than one character,
                    // so we know this is not the end of the string
                    {
                        ++i;
                        ch = escapedPattern[i];
                        switch( ch )
                        {
                        case '*':
                            // '*' - any character, zero or more times
                            sb.Append(".*"); // @"*" (pattern) --> @"\*" (escaped) --> @".*" (regex)
                            break;

                        case '?':
                            // '?' - any single character, or nothing
                            sb.Append(".?"); // @"?" (pattern) --> @"\?" (escaped) --> @".?" (regex)
                            break;

                        case '\\':
                            // we have two backslash characters
                            if( i < escapedPattern.Length - 1 )
                            {
                                ++i;
                                ch = escapedPattern[i];
                                switch( ch )
                                {
                                case '\\':
                                    // we have three backslash characters
                                    // we know this is not the end of the string,
                                    // since the third backslash was generated to escape something
                                    ++i;
                                    ch = escapedPattern[i];
                                    switch( ch )
                                    {
                                    case '*':
                                        sb.Append(@"\*"); // @"\*" (pattern) --> @"\\\*" (escaped) --> @"\*" (regex)
                                        break;

                                    case '?':
                                        sb.Append(@"\?"); // @"\?" (pattern) --> @"\\\?" (escaped) --> @"\?" (regex)
                                        break;

                                    case '\\':
                                        sb.Append(@"\\"); // @"\\" (pattern) --> @"\\\\" (escaped) --> @"\\" (regex)
                                        break;

                                    default:
                                        // something escaped by Regex.Escape: leave it
                                        sb.Append(@"\\\");
                                        sb.Append(ch); // @"\[" (pattern) --> @"\\\[" (escaped) --> @"\\\[" (regex)
                                        break;
                                    }
                                    break;

                                case '!':
                                    sb.Append('!'); // @"\!" (pattern) --> @"\\!" (escaped) --> @"!" (regex)
                                    break;

                                default:
                                    // @"\z" (pattern) --> @"\\z" (escaped)
                                    throw new FormatException("Invalid string pattern escape sequence!").Store("pattern", pattern).Store("escapedPattern", escapedPattern).Store("escapedIndex", i);
                                }
                            }
                            else
                            {
                                // there are no more characters after this
                                // @"\" (pattern) --> @"\\" (escaped)
                                throw new FormatException("Invalid string pattern escape sequence!").Store("pattern", pattern).Store("escapedPattern", escapedPattern).Store("escapedIndex", i);
                            }
                            break;

                        default:
                            // something escaped by Regex.Escape: leave it
                            sb.Append('\\');
                            sb.Append(ch); // @"[" (pattern) --> @"\[" (escaped) --> @"\[" (regex)
                            break;
                        }
                    }
                    break;

                case '!':
                    // '!' - any single character, exactly once
                    sb.Append('.'); // @"!" (pattern) --> @"!" (escaped) --> @"." (regex)
                    break;

                default:
                    sb.Append(ch);
                    break;
                }
            }

            sb.Append('$');
            return sb.ToString();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Indicates whether the specified string pattern finds a match in the specified input string.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="pattern">The string pattern to match.</param>
        /// <param name="invariantCulture"><c>true</c> to ignore cultural differences in language; otherwise, <c>false</c>.</param>
        /// <param name="cache"><c>true</c> to compile and cache the pattern, so that it will be faster next time; otherwise, <c>false</c>.</param>
        /// <returns><c>true</c> if the string pattern finds a match; otherwise, <c>false</c>.</returns>
        public static bool IsMatch( string input, string pattern, bool invariantCulture = false, bool cache = true )
        {
            if( input.NullReference()
             || pattern.NullReference() )
                throw new ArgumentNullException().Store("input", input).Store("pattern", pattern);

            Regex regex;
            if( cache )
                regex = AddOrGetRegex(pattern, invariantCulture);
            else
                regex = CreateRegex(pattern, invariantCulture, compile: false);

            return regex.IsMatch(input); // Regex is thread-safe
        }

        #endregion
    }
}
