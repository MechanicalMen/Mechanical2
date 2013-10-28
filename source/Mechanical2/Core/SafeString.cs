using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    //// NOTE: SafeString.Format is like a "safer" version of String.Format.
    ////       SafeString.Print is like a "safer" version of value.ToString(format, formatProvider)

    /// <summary>
    /// Common string operations which don't throw exceptions.
    /// </summary>
    public static class SafeString
    {
        #region FormatProvider

        //// NOTE: the Format method of an ICustomFormatter instance may be called in two ways:
        ////   - explicitly by the user, in which case the formatProvider passed to ICustomFormatter.Format may be anything.
        ////   - implicitly through an IFormatProvider passed to [Safe]String.Format, in which case the formatProvider parameter of ICustomFormatter.Format is the same object (that was passed to [Safe]String.Format; and the same object that ICustomFormatter.Format is being called on, although the interface is different)

        /// <summary>
        /// Format providers designed to be used implicitly through [Safe]String.Format (and others like them).
        /// They ignore their formatProvider parameters.
        /// </summary>
        public static class FormatProvider
        {
            #region FallbackBase

            /// <summary>
            /// A format provider that falls back to the specified format provider, unless otherwise instructed.
            /// Inheritors can format the types of their choosing, and let the fallback take care of the rest.
            /// </summary>
            public abstract class FallbackBase : IFormatProvider, ICustomFormatter
            {
                #region Private Fields

                private readonly IFormatProvider fallbackProvider;
                private readonly ICustomFormatter fallbackFormatter;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="FallbackBase"/> class.
                /// </summary>
                /// <param name="fallbackProvider">The <see cref="IFormatProvider"/> to fall back on; or <c>null</c>.</param>
                /// <param name="fallbackFormatter">The <see cref="ICustomFormatter"/> to fall back on; or <c>null</c>.</param>
                protected FallbackBase( IFormatProvider fallbackProvider = null, ICustomFormatter fallbackFormatter = null )
                {
                    this.fallbackProvider = fallbackProvider;
                    this.fallbackFormatter = fallbackFormatter;
                }

                #endregion

                #region Protected Methods

                /// <summary>
                /// Generates the string representation using only the fallback objects (and none of their overrides).
                /// </summary>
                /// <param name="format">A format string containing formatting specifications.</param>
                /// <param name="arg">An object to format.</param>
                /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/>.</returns>
                protected string FallbackFormat( string format, object arg )
                {
                    if( this.fallbackFormatter.NotNullReference() )
                        return this.fallbackFormatter.Format(format, arg, this.fallbackProvider);
                    else
                        return SafeString.Print(arg, format, this.fallbackProvider);
                }

                /// <summary>
                /// Overrides the <see cref="IFormatProvider"/> fallback.
                /// Gets an object that defines how to format the specified type.
                /// </summary>
                /// <param name="formatType">The type for which to get a formatting object.</param>
                /// <returns>A formatting object for the specified type; or <c>null</c>.</returns>
                protected virtual object ProvideFormat( Type formatType )
                {
                    if( formatType == typeof(ICustomFormatter) )
                        return this;
                    else
                        return null;
                }

                /// <summary>
                /// Overrides the <see cref="ICustomFormatter"/> fallback.
                /// Converts the value of a specified object to an equivalent string representation using the specified format.
                /// </summary>
                /// <param name="format">A format string containing formatting specifications.</param>
                /// <param name="arg">An object to format.</param>
                /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/>.</returns>
                protected virtual string ApplyFormat( string format, object arg )
                {
                    return null;
                }

                #endregion

                #region IFormatProvider

                /// <summary>
                /// Returns an object that provides formatting services for the specified type.
                /// </summary>
                /// <param name="formatType">An object that specifies the type of format object to return.</param>
                /// <returns>An instance of the object specified by <paramref name="formatType"/>, if the <see cref="IFormatProvider"/> implementation can supply that type of object; otherwise, <c>null</c>.</returns>
                object IFormatProvider.GetFormat( Type formatType )
                {
                    var formattingObject = this.ProvideFormat(formatType);
                    if( formattingObject.NotNullReference() )
                        return formattingObject;
                    else if( this.fallbackProvider.NotNullReference() )
                        return this.fallbackProvider.GetFormat(formatType);
                    else
                        return null;
                }

                #endregion

                #region ICustomFormatter

                /// <summary>
                /// Converts the value of a specified object to an equivalent string representation using the specified format and culture-specific formatting information.
                /// </summary>
                /// <param name="format">A format string containing formatting specifications.</param>
                /// <param name="arg">An object to format.</param>
                /// <param name="formatProvider">An object that supplies format information about the current instance.</param>
                /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.</returns>
                string ICustomFormatter.Format( string format, object arg, IFormatProvider formatProvider )
                {
                    var str = this.ApplyFormat(format, arg);
                    if( str.NotNullReference() )
                        return str;
                    else
                        return this.FallbackFormat(format, arg);
                }

                #endregion
            }

            #endregion

            #region Enumerable

            /// <summary>
            /// Supports <see cref="IEnumerable"/> formatting. Exposes the format of a culture. FormatProvider parameter is ignored.
            /// </summary>
            public class Enumerable : FallbackBase
            {
                #region Private Fields

                private const string OpeningBracket = "{";
                private const string ClosingBracket = "}";
                private const string Separator = ", ";
                private const string Ellipsis = "...";

                private const char DefaultEnumerableFormat = 'G';
                private const int DefaultMaxLength = 256;
                private const int InfiniteLength = 0;
                private const string DefaultElementFormat = null;

                #endregion

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Enumerable"/> class.
                /// </summary>
                /// <param name="fallbackProvider">The <see cref="IFormatProvider"/> to fall back on; or <c>null</c>.</param>
                /// <param name="fallbackFormatter">The <see cref="ICustomFormatter"/> to fall back on; or <c>null</c>.</param>
                protected Enumerable( IFormatProvider fallbackProvider = null, ICustomFormatter fallbackFormatter = null )
                    : base(fallbackProvider, fallbackFormatter)
                {
                }

                #endregion

                #region Protected Methods

                /// <summary>
                /// Overrides the <see cref="ICustomFormatter"/> fallback.
                /// Converts the value of a specified object to an equivalent string representation using the specified format.
                /// </summary>
                /// <param name="format">A format string containing formatting specifications.</param>
                /// <param name="arg">An object to format.</param>
                /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/>.</returns>
                protected override string ApplyFormat( string format, object arg )
                {
                    if( this.IsEnumerable(arg) )
                        return this.Format((IEnumerable)arg, format);
                    else
                        return base.ApplyFormat(format, arg);
                }

                #endregion

                #region Private Methods

                private bool IsEnumerable( object obj )
                {
                    return (obj is IEnumerable) && !(obj is string); // we don't want to print strings as a collection of characters!
                }

                private int GetDefaultMaxLength( char enumerableFormat )
                {
                    if( enumerableFormat == 'G' )
                        return DefaultMaxLength;
                    else
                        return InfiniteLength; // 'R'
                }

                private void ParseFormat( string format, out char enumerableFormat, out int maxLength, out string elementFormat )
                {
                    if( format.NullOrEmpty() )
                        goto returnDefaultFormat;

                    format = format.Trim();
                    switch( format[0] )
                    {
                    case 'g':
                    case 'G':
                        enumerableFormat = 'G';
                        break;

                    case 'r':
                    case 'R':
                        enumerableFormat = 'R';
                        break;

                    default:
                        // unknown format specifier
                        goto returnDefaultFormat;
                    }

                    if( format.Length != 1 )
                    {
                        int semicolonAt = format.IndexOf(';');
                        string maxLengthString;
                        if( semicolonAt == -1 )
                        {
                            // no semicolon
                            maxLengthString = format.Substring(1);
                            elementFormat = null;
                        }
                        else
                        {
                            // semicolon found
                            maxLengthString = format.Substring(1, semicolonAt - 1);
                            elementFormat = format.Substring(semicolonAt + 1).TrimStart();
                        }

                        if( maxLengthString.NullOrWhiteSpace() )
                        {
                            // maxLength not specified
                            maxLength = this.GetDefaultMaxLength(enumerableFormat);
                        }
                        else
                        {
                            // maxLength specified
                            if( !SafeString.TryParse(maxLengthString, out maxLength)
                             || maxLength < 0
                             || enumerableFormat == 'R' )
                            {
                                // invalid format
                                goto returnDefaultFormat;
                            }
                        }

                        return;
                    }
                    else
                    {
                        // at we least got the 'enumerableFormat'
                        maxLength = this.GetDefaultMaxLength(enumerableFormat);
                        elementFormat = DefaultElementFormat;
                        return;
                    }

                returnDefaultFormat:
                    enumerableFormat = DefaultEnumerableFormat;
                    maxLength = DefaultMaxLength;
                    elementFormat = DefaultElementFormat;
                }

                private string Format( IEnumerable enumerable, string format )
                {
                    char enumerableFormat;
                    int maxLength;
                    string elementFormat;
                    this.ParseFormat(format, out enumerableFormat, out maxLength, out elementFormat);

                    //// NOTE: enumerable may be: {};  {0};  {0, null, {}, ...}
                    //// NOTE: enumerableFormat may be: 'G', 'R'
                    //// NOTE: maxLength may be: 0, 1, 2, ...
                    //// NOTE: elementFormat may be: null, "", " ", other invalid or valid formats

                    bool isFirstItem = true;
                    string currentString;
                    int finalLength;

                    var sb = new StringBuilder();
                    sb.Append(OpeningBracket);

                    var enumerator = enumerable.GetEnumerator();
                    while( enumerator.MoveNext() )
                    {
                        if( isFirstItem )
                            isFirstItem = false;
                        else
                            sb.Append(Separator);

                        if( this.IsEnumerable(enumerator.Current) )
                            currentString = SafeString.Print(enumerator.Current, format, this);
                        else
                            currentString = SafeString.Print(enumerator.Current, elementFormat, this);

                        finalLength = sb.Length + currentString.Length + Separator.Length + Ellipsis.Length + ClosingBracket.Length;

                        if( enumerableFormat == 'R'
                         || finalLength <= maxLength )
                        {
                            sb.Append(currentString);
                        }
                        else
                        {
                            sb.Append(Ellipsis);
                            break;
                        }
                    }

                    sb.Append(ClosingBracket);

                    // perform one last check
                    if( enumerableFormat == 'R'
                     || sb.Length <= maxLength )
                        return sb.ToString();
                    else
                        //// e.g. "{...}" is too long but "{a}" is not
                        return string.Empty;
                }

                #endregion

                /// <summary>
                /// The default instance of the class.
                /// </summary>
                public static readonly Enumerable Default = new Enumerable();
            }

            #endregion

            #region Debug

            //// NOTE: we are not escaping non-ASCII or non-printable characters
            ////       that is left for the serializing code, or the displaying program to handle
            ////       (e.g. replacing CRLF with \r\n is the job of the interpreting program, not us)
            ////       (we do enclose strings in quotation marks, but that is mainly for making leading and trailing whitespaces "visible")

            /// <summary>
            /// Culture independent formatter for logging debugging data.
            /// </summary>
            public class Debug : Enumerable
            {
                #region Private Fields

                private const string Null = "null";
                private static readonly Dictionary<Type, string> BuiltInTypes;

                #endregion

                #region Constructors

                static Debug()
                {
                    BuiltInTypes = new Dictionary<Type, string>();
                    BuiltInTypes.Add(typeof(byte), "byte");
                    BuiltInTypes.Add(typeof(sbyte), "sbyte");
                    BuiltInTypes.Add(typeof(short), "short");
                    BuiltInTypes.Add(typeof(ushort), "ushort");
                    BuiltInTypes.Add(typeof(int), "int");
                    BuiltInTypes.Add(typeof(uint), "uint");
                    BuiltInTypes.Add(typeof(long), "long");
                    BuiltInTypes.Add(typeof(ulong), "ulong");
                    BuiltInTypes.Add(typeof(float), "float");
                    BuiltInTypes.Add(typeof(double), "double");
                    BuiltInTypes.Add(typeof(decimal), "decimal");
                    BuiltInTypes.Add(typeof(char), "char");
                    BuiltInTypes.Add(typeof(string), "string");
                    BuiltInTypes.Add(typeof(bool), "bool");
                    BuiltInTypes.Add(typeof(object), "object");
                    BuiltInTypes.Add(typeof(void), "void");
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Debug"/> class.
                /// </summary>
                protected Debug()
                    : base(fallbackProvider: CultureInfo.InvariantCulture)
                {
                }

                #endregion

                #region Private Methods

                #region Type

                private void AppendArrayBase( StringBuilder sb, string format, Type type )
                {
                    while( type.IsArray )
                        type = type.GetElementType();

                    this.AppendType(sb, format, type);
                }

                private void AppendArrayBrackets( StringBuilder sb, string format, Type type )
                {
                    if( type.IsArray )
                    {
                        sb.Append('[');
                        sb.Append(',', type.GetArrayRank() - 1);
                        sb.Append(']');

                        this.AppendArrayBrackets(sb, format, type.GetElementType());
                    }
                }

                private void AppendType( StringBuilder sb, string format, Type type )
                {
                    if( type.IsGenericParameter )
                    {
                        sb.Append(type.Name);
                    }
                    else if( type.IsArray )
                    {
                        this.AppendArrayBase(sb, format, type);
                        this.AppendArrayBrackets(sb, format, type);
                    }
                    else
                    {
                        string name;
                        if( BuiltInTypes.TryGetValue(type, out name) )
                        {
                            sb.Append(name);
                        }
                        else
                        {
#if !MECHANICAL_NET4CP && !SILVERLIGHT
                            var typeInfo = type.GetTypeInfo();
#else
                            var typeInfo = type;
#endif

                            // print namespace and possibly declaring type
                            if( type.IsNested )
                            {
                                this.AppendType(sb, format, type.DeclaringType);
                                sb.Append('.');
                            }
                            else
                            {
                                // only print namespace if not from 'mscorlib'
#if !SILVERLIGHT
                                if( !string.Equals(typeInfo.Assembly.GetName().Name, "mscorlib", StringComparison.Ordinal) )
#else
                                if( !typeInfo.Assembly.FullName.StartsWith("mscorlib", StringComparison.Ordinal) )
#endif
                                {
                                    // the namespace of nested types is the same as that of their declaring types
                                    sb.Append(type.Namespace);
                                    sb.Append('.');
                                }
                            }

                            // print name
                            name = type.Name;
                            int index = name.LastIndexOf('`');
                            if( index != -1 )
                                sb.Append(name, 0, index);
                            else
                                sb.Append(name);

                            // print generic arguments
                            // NOTE: same code as with MethodBase
                            if( typeInfo.IsGenericType )
                            {
                                Type[] types;
                                if( typeInfo.IsGenericTypeDefinition )
#if !MECHANICAL_NET4CP && !SILVERLIGHT
                                    types = typeInfo.GenericTypeParameters;
                                else
                                    types = type.GenericTypeArguments;
#else
                                    types = type.GetGenericArguments();
                                else
                                    types = Type.EmptyTypes;
#endif

                                sb.Append('<');
                                this.AppendType(sb, format, types[0]);
                                for( int i = 1; i < types.Length; ++i )
                                {
                                    sb.Append(", ");
                                    this.AppendType(sb, format, types[i]);
                                }
                                sb.Append('>');
                            }
                        }
                    }
                }

                #endregion

                #region ParameterInfo

                private void AppendParameter( StringBuilder sb, string format, ParameterInfo paramInfo )
                {
                    var paramType = paramInfo.ParameterType;
                    if( paramType.IsByRef )
                    {
                        // the parameter is passed by reference
                        // is it 'out' or 'ref'?
                        if( paramInfo.IsOut )
                            sb.Append("out ");
                        else
                            sb.Append("ref ");

                        // float instead of Single&
                        paramType = paramType.GetElementType();
                    }

                    if( Attribute.IsDefined(paramInfo, typeof(ParamArrayAttribute)) )
                    {
                        // 'params' parameter
                        sb.Append("params ");
                    }

                    // print parameter type
                    this.AppendType(sb, format, paramType);
                    sb.Append(' ');

                    // print parameter name
                    sb.Append(paramInfo.Name);
                }

                #endregion

                #region MethodBase

                private void AppendMethod( StringBuilder sb, string format, MethodBase methodBase )
                {
                    // print the return value, if this is not a constructor
                    var methodInfo = methodBase as MethodInfo;
                    if( methodInfo.NotNullReference() )
                    {
                        this.AppendType(sb, format, methodInfo.ReturnType);
                        sb.Append(' ');
                    }

                    // print the declaring type (if constructor)
                    this.AppendType(sb, format, methodBase.DeclaringType);
                    if( methodInfo.NotNullReference() )
                        sb.Append('.'); // constructors already have the '.' in their name

                    // print the name
                    sb.Append(methodBase.Name);

                    // print generic parameters
                    // NOTE: same code as with Type
                    if( methodBase.IsGenericMethod )
                    {
                        var types = methodBase.GetGenericArguments();

                        sb.Append('<');
                        this.AppendType(sb, format, types[0]);
                        for( int i = 1; i < types.Length; ++i )
                        {
                            sb.Append(", ");
                            this.AppendType(sb, format, types[i]);
                        }
                        sb.Append('>');
                    }

                    // print the parameter list
                    var parameters = methodBase.GetParameters();
                    sb.Append('(');

                    if( parameters.Length != 0 )
                    {
                        this.AppendParameter(sb, format, parameters[0]);
                        for( int i = 1; i < parameters.Length; ++i )
                        {
                            sb.Append(", ");
                            this.AppendParameter(sb, format, parameters[i]);
                        }
                    }

                    sb.Append(')');
                }

                #endregion

                #endregion

                #region Protected Methods

                /// <summary>
                /// Overrides the <see cref="ICustomFormatter"/> fallback.
                /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
                /// </summary>
                /// <param name="format">A format string containing formatting specifications.</param>
                /// <param name="arg">An object to format.</param>
                /// <returns>The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/>.</returns>
                protected override string ApplyFormat( string format, object arg )
                {
                    if( arg.NullReference() )
                    {
                        return Null;
                    }
                    else if( arg is sbyte )
                    {
                        return this.FallbackFormat(format, arg) + "y";
                    }
                    else if( arg is byte )
                    {
                        return this.FallbackFormat(format, arg) + "uy";
                    }
                    else if( arg is short )
                    {
                        return this.FallbackFormat(format, arg) + "s";
                    }
                    else if( arg is ushort )
                    {
                        return this.FallbackFormat(format, arg) + "us";
                    }
                    else if( arg is uint )
                    {
                        return this.FallbackFormat(format, arg) + "u";
                    }
                    else if( arg is long )
                    {
                        return this.FallbackFormat(format, arg) + "L";
                    }
                    else if( arg is ulong )
                    {
                        return this.FallbackFormat(format, arg) + "UL";
                    }
                    else if( arg is float )
                    {
                        return this.FallbackFormat(string.IsNullOrEmpty(format) ? "R" : format, arg) + "f";
                    }
                    else if( arg is double )
                    {
                        return this.FallbackFormat(string.IsNullOrEmpty(format) ? "R" : format, arg) + "d";
                    }
                    else if( arg is decimal )
                    {
                        return this.FallbackFormat(format, arg) + "m";
                    }
                    else if( arg is bool )
                    {
                        return (bool)arg ? "true" : "false";
                    }
                    else if( arg is char )
                    {
                        return '\'' + ((char)arg).ToString() + '\'';
                    }
                    else if( arg is string )
                    {
                        return '"' + ((string)arg).Replace("\"", "\"\"") + '"';
                    }
                    else if( arg is Substring )
                    {
                        return SafeString.Print(((Substring)arg).ToString(), format, formatProvider: this);
                    }
                    else if( arg is byte[] )
                    {
                        return Convert.ToBase64String((byte[])arg);
                    }
                    else if( arg is Type )
                    {
                        var sb = new StringBuilder();
                        this.AppendType(sb, format, (Type)arg);
                        return sb.ToString();
                    }
#if !MECHANICAL_NET4CP && !SILVERLIGHT
                    else if( arg is TypeInfo )
                    {
                        var sb = new StringBuilder();
                        this.AppendType(sb, format, ((TypeInfo)arg).GetType());
                        return sb.ToString();
                    }
#endif
                    else if( arg is ParameterInfo )
                    {
                        var sb = new StringBuilder();
                        this.AppendParameter(sb, format, (ParameterInfo)arg);
                        return sb.ToString();
                    }
                    else if( arg is MethodBase )
                    {
                        var sb = new StringBuilder();
                        this.AppendMethod(sb, format, (MethodBase)arg);
                        return sb.ToString();
                    }
                    else if( arg is Exception )
                    {
                        return Mechanical.DataStores.ExceptionInfo.From((Exception)arg).ToString();
                    }
                    else if( arg is Mechanical.DataStores.ExceptionInfo )
                    {
                        return ((Mechanical.DataStores.ExceptionInfo)arg).ToString();
                    }
                    else if( arg is DateTime )
                    {
                        return ((DateTime)arg).ToString("o", CultureInfo.InvariantCulture);
                    }
                    else if( arg is DateTimeOffset )
                    {
                        return ((DateTimeOffset)arg).ToString("o", CultureInfo.InvariantCulture);
                    }
                    else if( arg is TimeSpan )
                    {
                        return ((TimeSpan)arg).ToString("c", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        var type = arg.GetType();
                        if( type.IsEnum )
                        {
                            // defer to the Enum<T> handler
                            var enumWrapper = typeof(Enum<>).MakeGenericType(type);
                            var wrapperInstance = Activator.CreateInstance(enumWrapper, arg);
                            return SafeString.Print(wrapperInstance, format, this);
                        }
                        else if( type.IsGenericType )
                        {
                            var typeDef = type.GetGenericTypeDefinition();
                            if( typeDef == typeof(Enum<>) )
                            {
                                // print the type, and then use the wrapper to print the value
                                var enumType = type.GetGenericArguments()[0];
                                return SafeString.Print(enumType, format: null, formatProvider: this) + '.' + arg.ToString();
                            }
                            else if( typeDef == typeof(Nullable<>) )
                            {
                                if( arg.NullReference() )
                                    return Null;

                                var hasValue = (bool)arg.GetType().GetProperty("HasValue", BindingFlags.Public | BindingFlags.Instance).GetValue(arg, index: null);
                                if( hasValue )
                                {
                                    object value = arg.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetValue(arg, index: null);
                                    return SafeString.DebugPrint(value);
                                }
                                else
                                    return Null;
                            }
                        }

                        return base.ApplyFormat(format, arg);
                    }
                }

                #endregion

                /// <summary>
                /// The default instance of the class.
                /// </summary>
                public static new readonly Debug Default = new Debug();
            }

            #endregion
        }

        #endregion

        #region Private Static Methods

        private static bool TryParse( string str, out int value )
        {
            return int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        #endregion

        #region TryPrint, Print

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="format">The format to use; or <c>null</c>.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <param name="result">The string representation of <paramref name="obj"/> in the specified format.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryPrint( object obj, string format, IFormatProvider formatProvider, out string result )
        {
            bool completeSuccess = true;

            // can we leave the job to a custom formatter?
            if( formatProvider.NotNullReference() )
            {
                // try to find one
                ICustomFormatter customFormatter;
                try
                {
                    customFormatter = formatProvider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
                }
                catch
                {
                    completeSuccess = false;
                    customFormatter = null;
                }

                // try to use one
                if( customFormatter.NotNullReference() )
                {
                    try
                    {
                        result = customFormatter.Format(format, obj, formatProvider);
                    }
                    catch
                    {
                        completeSuccess = false;
                        result = null;
                    }

                    // did it work?
                    if( result.NotNullReference() )
                        return completeSuccess;
                }
            }

            if( obj.NotNullReference() )
            {
                // is it perhaps formattable?
                var formattable = obj as IFormattable;
                if( formattable.NotNullReference() )
                {
                    try
                    {
                        result = formattable.ToString(format, formatProvider);
                    }
                    catch
                    {
                        completeSuccess = false;
                        result = null;
                    }

                    // did it work?
                    if( result.NotNullReference() )
                        return completeSuccess;
                }

                // nothing left but ToString
                try
                {
                    result = obj.ToString();
                }
                catch
                {
                    completeSuccess = false;
                    result = null;
                }

                // did it work?
                if( result.NotNullReference() )
                    return completeSuccess;
            }

            result = string.Empty;
            return completeSuccess; // NOT false!
        }

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="format">The format to use; or <c>null</c>.</param>
        /// <param name="result">The string representation of <paramref name="obj"/> in the specified format.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryPrint( object obj, string format, out string result )
        {
            return TryPrint(obj, format, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <param name="result">The string representation of <paramref name="obj"/> in the specified format.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryPrint( object obj, IFormatProvider formatProvider, out string result )
        {
            return TryPrint(obj, null, formatProvider, out result);
        }

        /// <summary>
        /// Gets the string representation of an object.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="result">The string representation of <paramref name="obj"/>.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryPrint( object obj, out string result )
        {
            return TryPrint(obj, null, CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="format">The format to use; or <c>null</c>.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <returns>The string representation of <paramref name="obj"/> in the specified format.</returns>
        public static string Print( object obj, string format, IFormatProvider formatProvider )
        {
            string result;
            TryPrint(obj, format, formatProvider, out result);
            return result;
        }

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="format">The format to use; or <c>null</c>.</param>
        /// <returns>The string representation of <paramref name="obj"/> in the specified format.</returns>
        public static string Print( object obj, string format )
        {
            string result;
            TryPrint(obj, format, CultureInfo.CurrentCulture, out result);
            return result;
        }

        /// <summary>
        /// Gets the string representation of an object in the specified format.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <returns>The string representation of <paramref name="obj"/> in the specified format.</returns>
        public static string Print( object obj, IFormatProvider formatProvider )
        {
            string result;
            TryPrint(obj, null, formatProvider, out result);
            return result;
        }

        /// <summary>
        /// Gets the string representation of an object.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <returns>The string representation of <paramref name="obj"/>.</returns>
        public static string Print( object obj )
        {
            string result;
            TryPrint(obj, null, CultureInfo.CurrentCulture, out result);
            return result;
        }

        #endregion

        #region TryFormat, Format

        private static readonly object[] EmptyObjectArray = new object[0];
        private static readonly char[] CurlyBrackets = new char[] { '{', '}' };

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. A specified parameter supplies culture-specific formatting information.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="result">The string representation of <paramref name="args"/> in the specified format.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryFormat( out string result, IFormatProvider provider, string format, params object[] args )
        {
            bool completeSuccess = true;

            if( format.NullReference() )
            {
                format = string.Empty;
                completeSuccess = false;
            }

            if( args.NullReference() )
            {
                args = EmptyObjectArray;
                completeSuccess = false;
            }

            var sb = new StringBuilder(format.Length + (args.Length * 8));
            int numWritten = 0;
            while( numWritten < format.Length )
            {
                int at = format.IndexOfAny(CurlyBrackets, numWritten);
                if( at == -1 )
                {
                    // no more special characters, write what's left and return
                    sb.Append(format, numWritten, format.Length - numWritten);
                    break;
                }
                else
                {
                    // special character found: write everything up to it
                    int length = at - numWritten;
                    sb.Append(format, numWritten, length);
                    numWritten += length;
                }


                if( format[at] == '}' )
                {
                    if( at + 1 < format.Length
                     && format[at + 1] == '}' )
                    {
                        // escape sequence
                        sb.Append('}');
                        numWritten += 2;
                        continue;
                    }
                    else
                    {
                        // syntax error
                        completeSuccess = false;
                        numWritten += 1; // skip the character
                        continue;
                    }
                }

                // at this point we know that format[at] == '{'
                if( at + 1 < format.Length
                 && format[at + 1] == '{' )
                {
                    // escape sequence
                    sb.Append('{');
                    numWritten += 2;
                    continue;
                }
                else
                {
                    // the start of a composite format string
                    int endAt = format.IndexOf('}', at + 1);
                    if( endAt == -1 )
                    {
                        // syntax error
                        completeSuccess = false;
                        numWritten += 1; // skip the character
                        continue;
                    }

                    // find out if the alignment, or format string components were defined
                    int alignmentCharAt = format.IndexOf(',', at + 1);
                    if( alignmentCharAt >= endAt )
                        alignmentCharAt = -1;
                    int formatStringCharAt = alignmentCharAt == -1 ? format.IndexOf(':', at + 1) : format.IndexOf(':', alignmentCharAt + 1);
                    if( formatStringCharAt >= endAt )
                        formatStringCharAt = -1;


                    // find the substring representing the index component
                    int nextCharAfterIndexAt = endAt;
                    if( alignmentCharAt != -1 )
                        nextCharAfterIndexAt = alignmentCharAt;
                    else if( formatStringCharAt != -1 )
                        nextCharAfterIndexAt = formatStringCharAt;

                    // try to parse the index component
                    int index;
                    if( !TryParse(format.Substring(at + 1, nextCharAfterIndexAt - (at + 1)), out index)
                     || index < 0
                     || index >= args.Length )
                    {
                        // bad format, or out of range
                        completeSuccess = false;
                        numWritten = endAt + 1;
                        continue;
                    }

                    int alignment = 0;
                    if( alignmentCharAt != -1 )
                    {
                        // find the substring representing the alignment component
                        int nextCharAfterAlignmentAt = endAt;
                        if( formatStringCharAt != -1 )
                            nextCharAfterAlignmentAt = formatStringCharAt;

                        // try to parse alignment component
                        if( !TryParse(format.Substring(alignmentCharAt + 1, nextCharAfterAlignmentAt - (alignmentCharAt + 1)), out alignment) )
                        {
                            // bad format
                            completeSuccess = false;
                            //// NOTE: we will try to continue without the alignment component
                        }
                    }


                    // extract format string
                    string currentFormatString = null;
                    if( formatStringCharAt != -1 )
                    {
                        int formatStringStartAt = Math.Max(alignmentCharAt + 1, formatStringCharAt + 1);
                        currentFormatString = format.Substring(formatStringStartAt, endAt - formatStringStartAt);
                    }


                    // produce the formatted string representation of the argument
                    var arg = args[index];
                    string text = null; // keeps compiler from nagging
                    completeSuccess = TryPrint(arg, currentFormatString, provider, out text) && completeSuccess;


                    // apply alignment component
                    if( alignment > 0 )
                        text = text.PadLeft(alignment);
                    else
                        text = text.PadRight(-alignment);


                    // write result and skip composite format string
                    sb.Append(text);
                    numWritten = endAt + 1;
                }
            }

            result = sb.ToString();
            return completeSuccess;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="result">A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns><c>false</c> if <see cref="M:String.Format"/> would have thrown an exception; otherwise <c>true</c>.</returns>
        public static bool TryFormat( out string result, string format, params object[] args )
        {
            return TryFormat(out result, null, format, args);
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. A specified parameter supplies culture-specific formatting information.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information; or <c>null</c>.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</returns>
        public static string Format( IFormatProvider provider, string format, params object[] args )
        {
            string result;
            TryFormat(out result, provider, format, args);
            return result;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</returns>
        public static string Format( string format, params object[] args )
        {
            string result;
            TryFormat(out result, null, format, args);
            return result;
        }

        #endregion

        #region DebugPrint, DebugFormat

        /// <summary>
        /// Gets the string representation of an object using the Debug formatter.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="format">The format to use; or <c>null</c>.</param>
        /// <returns>The string representation of <paramref name="obj"/>.</returns>
        public static string DebugPrint( object obj, string format = null )
        {
            string result;
            TryPrint(obj, format, FormatProvider.Debug.Default, out result);
            return result;
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array.
        /// Uses the Debug formatter.
        /// Always returns a non-null string.
        /// Throws no exceptions.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of <paramref name="format"/> in which the format items have been replaced by the string representation of the corresponding objects in <paramref name="args"/>.</returns>
        public static string DebugFormat( string format, params object[] args )
        {
            string result;
            TryFormat(out result, FormatProvider.Debug.Default, format, args);
            return result;
        }

        #endregion
    }
}
