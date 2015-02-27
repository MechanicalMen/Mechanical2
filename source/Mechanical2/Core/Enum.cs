using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Mechanical.Collections;
using Mechanical.Conditions;

namespace Mechanical.Core
{
    /// <summary>
    /// An immutable, strongly typed wrapper for enumerations. Do note, that in some edge cases it deviates from the results of <see cref="System.Enum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented", Justification = "The partial class is private.")]
    public struct Enum<TEnum> : IEquatable<TEnum>, IEquatable<Enum<TEnum>>
        where TEnum : struct, IFormattable, IConvertible, IComparable
    {
        #region Constructors

        static Enum()
        {
            var enumType = typeof(TEnum);
            Ensure.That(typeof(TEnum).IsEnum).IsTrue(() => new ArgumentException("The specified type is not an 'enum'!").Store("enumType", enumType));

            UnderlyingType = Enum.GetUnderlyingType(enumType);
            ZeroValue = Expression.Lambda<Func<TEnum>>(Expression.Convert(Expression.Default(UnderlyingType), enumType)).Compile()();
            HasFlagsAttribute = Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute), inherit: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enum{TEnum}"/> struct.
        /// </summary>
        /// <param name="value">The enumeration value to wrap.</param>
        public Enum( TEnum value )
        {
            this.value = value;
        }

        #endregion

        #region Instance Members

        private readonly TEnum value;

        #region ToString

        /// <summary>
        /// Converts the value to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of this value.</returns>
        public override string ToString()
        {
            return this.value.ToString();
        }

        #endregion

        #region IsDefined

        private static partial class CompiledOp
        {
            private static Func<TEnum, TEnum, bool> enumEquals;

            internal static Func<TEnum, TEnum, bool> EnumEquals
            {
                get
                {
                    if( enumEquals.NullReference() )
                    {
                        var leftParam = Expression.Parameter(typeof(TEnum));
                        var rightParam = Expression.Parameter(typeof(TEnum));
                        var left = Expression.Convert(leftParam, UnderlyingType);
                        var right = Expression.Convert(rightParam, UnderlyingType);
                        var body = Expression.Equal(left, right);
                        var op = Expression.Lambda<Func<TEnum, TEnum, bool>>(body, leftParam, rightParam).Compile();
                        Interlocked.CompareExchange(ref enumEquals, op, null);
                    }

                    return enumEquals;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a constant with this value exists in the enumeration.
        /// </summary>
        /// <value>Indicates whether a constant with this value exists in the enumeration.</value>
        public bool IsDefined
        {
            get
            {
                var values = Values;
                var equals = CompiledOp.EnumEquals;
                for( int i = 0; i < values.Count; ++i )
                {
                    if( equals(this, values[i]) )
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region HasDescription, Description

        /// <summary>
        /// Gets a value indicating whether a description is available for this value.
        /// </summary>
        /// <value>Indicates whether a description is available for this value.</value>
        public bool HasDescription
        {
            get { return Descriptions.ContainsKey(this); }
        }

        /// <summary>
        /// Gets the description of this value, or <see cref="String.Empty"/> if the attribute was not defined.
        /// </summary>
        /// <returns>The description of this value, or <see cref="String.Empty"/> if the attribute was not defined.</returns>
        public string Description
        {
            get
            {
                string descr;
                if( Descriptions.TryGetValue(this, out descr) )
                    return descr;
                else
                    return string.Empty;
            }
        }

        #endregion

        #region HasFlag

        private static partial class CompiledOp
        {
            private static Func<TEnum, TEnum, bool> hasFlag;

            internal static Func<TEnum, TEnum, bool> HasFlag
            {
                get
                {
                    if( hasFlag.NullReference() )
                    {
                        //// NOTE: this may be faster than the .NET implementation.
                        ////       we would have to compile anyways, it is an instance method (reflection!)
                        ////       see: http://msdn.microsoft.com/en-us/library/system.enum.hasflag.aspx

                        var valueParam = Expression.Parameter(typeof(TEnum));
                        var flagParam = Expression.Parameter(typeof(TEnum));
                        var v = Expression.Convert(valueParam, UnderlyingType);
                        var f = Expression.Convert(flagParam, UnderlyingType);
                        var and = Expression.And(v, f);
                        var body = Expression.Equal(and, f);
                        var op = Expression.Lambda<Func<TEnum, TEnum, bool>>(body, valueParam, flagParam).Compile();
                        Interlocked.CompareExchange(ref hasFlag, op, null);
                    }

                    return hasFlag;
                }
            }
        }

        /// <summary>
        /// Determines whether one or more bit fields are set in the current instance.
        /// </summary>
        /// <param name="flag">An enumeration value.</param>
        /// <returns><c>true</c> if the bit field or bit fields that are set in <paramref name="flag"/> are also set in the current instance; otherwise, <c>false</c>.</returns>
        public bool HasFlag( TEnum flag )
        {
            if( !DefinesFlags )
                throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

            return CompiledOp.HasFlag(this, flag);
        }

        #endregion

        #region SetFlag

        private static partial class CompiledOp
        {
            private static Func<TEnum, TEnum, TEnum> setFlag;

            internal static Func<TEnum, TEnum, TEnum> SetFlag
            {
                get
                {
                    if( setFlag.NullReference() )
                    {
                        var valueParam = Expression.Parameter(typeof(TEnum));
                        var flagParam = Expression.Parameter(typeof(TEnum));
                        var v = Expression.Convert(valueParam, UnderlyingType);
                        var f = Expression.Convert(flagParam, UnderlyingType);
                        var or = Expression.Or(v, f);
                        var body = Expression.Convert(or, typeof(TEnum));
                        var op = Expression.Lambda<Func<TEnum, TEnum, TEnum>>(body, valueParam, flagParam).Compile();
                        Interlocked.CompareExchange(ref setFlag, op, null);
                    }

                    return setFlag;
                }
            }
        }

        /// <summary>
        /// Sets one or more bit fields in a new instance.
        /// </summary>
        /// <param name="flag">An enumeration value.</param>
        /// <returns>A new enumeration value that has the specified flags set.</returns>
        public Enum<TEnum> SetFlag( TEnum flag )
        {
            if( !DefinesFlags )
                throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

            return new Enum<TEnum>(CompiledOp.SetFlag(this, flag));
        }

        #endregion

        #region RemoveFlag

        private static partial class CompiledOp
        {
            private static Func<TEnum, TEnum, TEnum> removeFlag;

            internal static Func<TEnum, TEnum, TEnum> RemoveFlag
            {
                get
                {
                    if( removeFlag.NullReference() )
                    {
                        var valueParam = Expression.Parameter(typeof(TEnum));
                        var flagParam = Expression.Parameter(typeof(TEnum));
                        var v = Expression.Convert(valueParam, UnderlyingType);
                        var nf = Expression.Not(Expression.Convert(flagParam, UnderlyingType));
                        var and = Expression.And(v, nf);
                        var body = Expression.Convert(and, typeof(TEnum));
                        var op = Expression.Lambda<Func<TEnum, TEnum, TEnum>>(body, valueParam, flagParam).Compile();
                        Interlocked.CompareExchange(ref removeFlag, op, null);
                    }

                    return removeFlag;
                }
            }
        }

        /// <summary>
        /// Unsets one or more bit fields in a new instance.
        /// </summary>
        /// <param name="flag">An enumeration value.</param>
        /// <returns>A new enumeration value that has the specified flags unset.</returns>
        public Enum<TEnum> RemoveFlag( TEnum flag )
        {
            if( !DefinesFlags )
                throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

            return new Enum<TEnum>(CompiledOp.RemoveFlag(this, flag));
        }

        #endregion

        #region IsValid

        /// <summary>
        /// Gets a value indicating whether the specified value can be constructed using the constants (flags) of the enumeration.
        /// </summary>
        /// <param name="value">An enumeration value.</param>
        /// <returns><c>true</c> if the specified value can be constructed using the constants of the enumeration; otherwise, <c>false</c>.</returns>
        public bool IsValid
        {
            get
            {
                if( !DefinesFlags )
                    throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

                if( this.IsDefined )
                    return true;

                if( this.IsZero )
                    return false; // an undefined zero can not be "reconstructed"

                var equal = CompiledOp.EnumEquals;
                var reconstructed = Zero;
                foreach( var f in GetNonZeroFlags(this) )
                    reconstructed = reconstructed.SetFlag(f); // (f != this) because 'this' is undefined

                return equal(reconstructed, this);
            }
        }

        #endregion

        #region IsZero

        /// <summary>
        /// Gets a value indicating whether the underlying value is zero. For flags, this means the no bits are set.
        /// </summary>
        /// <value><c>true</c> if the underlying value is zero; otherwise <c>false</c>.</value>
        public bool IsZero
        {
            get { return CompiledOp.EnumEquals(this, Zero); }
        }

        #endregion

        #region AsSimpleFlags, IsSimpleFlag

        /// <summary>
        /// Gets a value indicating whether the current value is a simple flag (i.e. a non-zero constant that has no (defined) flags, other than itself).
        /// </summary>
        /// <value><c>true</c> if the current value is a simple flag; otherwise <c>false</c>.</value>
        public bool IsSimpleFlag
        {
            get
            {
                if( !DefinesFlags )
                    throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

                var equals = CompiledOp.EnumEquals;
                foreach( var f in SimpleFlags )
                {
                    if( equals(this, f) )
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the simple flags of this value.
        /// </summary>
        /// <value>The single flags of this value.</value>
        public Enum<TEnum>[] AsSimpleFlags
        {
            get
            {
                if( !DefinesFlags )
                    throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

                var list = new List<Enum<TEnum>>();
                foreach( var potentialFlag in GetNonZeroFlags(this) )
                {
                    if( potentialFlag.IsSimpleFlag )
                        list.Add(potentialFlag);
                }
                return list.ToArray();
            }
        }

        #endregion

        #endregion

        #region Static Members

        private static readonly Type UnderlyingType;

        #region Names, Values, WrappedValues

        //// NOTE: we BREAK System.Enum here!

        //// NOTE: if there is an enum with constants A1 and A2=A1
        ////       then GetNames returns A1 and A2, but GetValues returns A1 and A1
        ////       (this is by design: both A1 and A2 are represented by the same value,
        ////       we do the same thing)

        //// NOTE: GetNames returns (arguably) unexpected results for negative constants
        ////       see: http://stackoverflow.com/questions/6819348/enum-getnames-results-in-unexpected-order-with-negative-enum-constants

        private static Tuple<ReadOnlyList.Base<string>, ReadOnlyList.Base<TEnum>, ReadOnlyList.Base<Enum<TEnum>>> namesValues = null;

        private class UnderlyingObjectComparer : IComparer<object>
        {
            public int Compare( object x, object y )
            {
                return ((IComparable)x).CompareTo(y);
            }
        }

        private static partial class CompiledOp
        {
            private static Func<object, TEnum> objToVal;

            internal static Func<object, TEnum> ObjectToValue
            {
                get
                {
                    if( objToVal.NullReference() )
                    {
                        var param = Expression.Parameter(typeof(object));
                        var conv = Expression.Convert(Expression.Convert(param, UnderlyingType), typeof(TEnum));
                        var op = Expression.Lambda<Func<object, TEnum>>(conv, param).Compile();
                        Interlocked.CompareExchange(ref objToVal, op, null);
                    }

                    return objToVal;
                }
            }
        }

        private static Tuple<ReadOnlyList.Base<string>, ReadOnlyList.Base<TEnum>, ReadOnlyList.Base<Enum<TEnum>>> GetNamesValues()
        {
            var list = new List<KeyValuePair<string, object>>();
            foreach( var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static) )
                list.Add(new KeyValuePair<string, object>(field.Name, field.GetValue(null)));

            // we do not use List<T>.Sort, because it uses an unstable sort (that is, if two elements are equal, their order might not be preserved)
            // OrderBy however performs a stable sort.
            list = list.OrderBy(pair => pair.Value, new UnderlyingObjectComparer()).ToList();

            var names = new string[list.Count];
            for( int i = 0; i < names.Length; ++i )
                names[i] = list[i].Key;

            var objToValue = CompiledOp.ObjectToValue;
            var values = new TEnum[list.Count];
            var wrappedValues = new Enum<TEnum>[list.Count];
            for( int i = 0; i < values.Length; ++i )
            {
                values[i] = objToValue(list[i].Value);
                wrappedValues[i] = values[i];
            }

            return new Tuple<ReadOnlyList.Base<string>, ReadOnlyList.Base<TEnum>, ReadOnlyList.Base<Enum<TEnum>>>(new ReadOnlyList.Wrapper<string>(names), new ReadOnlyList.Wrapper<TEnum>(values), new ReadOnlyList.Wrapper<Enum<TEnum>>(wrappedValues));
        }

        /// <summary>
        /// Gets an array of the names of the constants in the enumeration.
        /// </summary>
        /// <value>An array of the names of the constants in the enumeration.</value>
        public static ReadOnlyList.Base<string> Names
        {
            get
            {
                if( namesValues.NullReference() )
                {
                    var newNamesValues = GetNamesValues();
                    Interlocked.CompareExchange(ref namesValues, newNamesValues, null);
                }

                return namesValues.Item1;
            }
        }

        /// <summary>
        /// Gets an array of the values of the constants in the enumeration.
        /// </summary>
        /// <value>An array of the values of the constants in the enumeration.</value>
        public static ReadOnlyList.Base<TEnum> Values
        {
            get
            {
                if( namesValues.NullReference() )
                {
                    var newNamesValues = GetNamesValues();
                    Interlocked.CompareExchange(ref namesValues, newNamesValues, null);
                }

                return namesValues.Item2;
            }
        }

        /// <summary>
        /// Gets an array of the values of the constants in the enumeration.
        /// </summary>
        /// <value>An array of the values of the constants in the enumeration.</value>
        public static ReadOnlyList.Base<Enum<TEnum>> WrappedValues
        {
            get
            {
                if( namesValues.NullReference() )
                {
                    var newNamesValues = GetNamesValues();
                    Interlocked.CompareExchange(ref namesValues, newNamesValues, null);
                }

                return namesValues.Item3;
            }
        }

        #endregion

        #region Parse, TryParse

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> to ignore case; <c>false</c> to regard case.</param>
        /// <returns>An instance of <typeparamref name="TEnum"/> whose value is represented by <paramref name="value"/>.</returns>
        public static Enum<TEnum> Parse( string value, bool ignoreCase = false )
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="result">When this method returns, contains an object of type <typeparamref name="TEnum"/> whose value is represented by <paramref name="value"/>. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse( string value, out Enum<TEnum> result )
        {
            TEnum r;
            if( Enum.TryParse<TEnum>(value, out r) )
            {
                result = r;
                return true;
            }
            else
            {
                result = default(Enum<TEnum>);
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> to ignore case; <c>false</c> to consider case.</param>
        /// <param name="result">When this method returns, contains an object of type <typeparamref name="TEnum"/> whose value is represented by <paramref name="value"/>. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter was converted successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse( string value, bool ignoreCase, out Enum<TEnum> result )
        {
            TEnum r;
            if( Enum.TryParse<TEnum>(value, ignoreCase, out r) )
            {
                result = r;
                return true;
            }
            else
            {
                result = default(Enum<TEnum>);
                return false;
            }
        }

        #endregion

        #region Descriptions

        //// NOTE: if there is an enum with constants A1 and A2=A1
        ////       then Enum.Parse("A2") returns A1, and we would loose the description,
        ////       if it was applied to A2 (we avoid this by directly accessing the enum fields)

        private static IReadOnlyDictionary<TEnum, string> descriptions;

        /// <summary>
        /// Gets all available descriptions. The keys are the string representations of the constants.
        /// </summary>
        /// <value>All available descriptions.</value>
        public static IReadOnlyDictionary<TEnum, string> Descriptions
        {
            get
            {
                if( descriptions.NullReference() )
                {
                    var newDescriptions = new Dictionary<TEnum, string>();
                    var objToValue = CompiledOp.ObjectToValue;

                    foreach( var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static) )
                    {
                        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), inherit: false);
                        if( attribute.NotNullReference() )
                        {
                            var e = objToValue(field.GetValue(null));
                            if( newDescriptions.ContainsKey(e) )
                            {
                                // both the first value encountered and it's alias have a description
                                // choose the one that System.Enum would return
                                if( string.Equals(Enum.GetName(typeof(TEnum), e), field.Name, StringComparison.Ordinal) )
                                    newDescriptions[e] = attribute.Description;
                            }
                            else
                            {
                                newDescriptions.Add(e, attribute.Description);
                            }
                        }
                    }

                    Interlocked.CompareExchange(ref descriptions, new ReadOnlyDictionary.Wrapper<TEnum, string>(newDescriptions), null);
                }

                return descriptions;
            }
        }

        #endregion

        #region DefinesFlags

        private static readonly bool HasFlagsAttribute;

        /// <summary>
        /// Gets a value indicating whether the enumeration was declared with the Flags attribute.
        /// </summary>
        /// <value>Indicates whether the enumeration was declared with the Flags attribute.</value>
        public static bool DefinesFlags
        {
            get { return HasFlagsAttribute; }
        }

        #endregion

        #region Zero

        private static readonly Enum<TEnum> ZeroValue;
        private static int hasZero = Unknown;
        private const int Unknown = 0;
        private const int Yes = 1;
        private const int No = 2;

        /// <summary>
        /// Gets an enumeration value with an underlying value of zero. For flags, this means the no bits are set. May not be valid.
        /// </summary>
        /// <value>An enumeration value with an underlying value of zero.</value>
        public static Enum<TEnum> Zero
        {
            get { return ZeroValue; }
        }

        /// <summary>
        /// Gets a value indicating whether the enumeration defines a constant with an underlying value of zero.
        /// </summary>
        /// <value><c>true</c> if the enumeration defines a constant with an underlying value of zero; otherwise <c>false</c>.</value>
        public static bool HasZero
        {
            get
            {
                if( hasZero == Unknown )
                    Interlocked.CompareExchange(ref hasZero, Zero.IsDefined ? Yes : No, comparand: Unknown);

                return hasZero == Yes;
            }
        }

        #endregion

        #region SimpleFlags

        private static ReadOnlyList.Base<Enum<TEnum>> simpleFlags;

        private static List<Enum<TEnum>> GetNonZeroFlags( Enum<TEnum> value )
        {
            // NOTE: this will return the original value as well (and may return more flags than are necessary)
            var list = new List<Enum<TEnum>>();

            foreach( var potentialFlag in WrappedValues )
            {
                if( value.HasFlag(potentialFlag)
                 && !potentialFlag.IsZero ) // do not consider "zero" a flag
                    list.Add(potentialFlag);
            }

            return list;
        }

        private static List<Enum<TEnum>> GetSimpleFlags()
        {
            var simpleFlags = new List<Enum<TEnum>>();
            var values = WrappedValues;
            if( values.Count != 0 )
            {
                foreach( var v in values )
                {
                    if( v.IsZero )
                        continue;

                    var flags = GetNonZeroFlags(v);
                    if( flags.Count == 1 )
                    {
                        simpleFlags.Add(v);
                    }
                }
            }

            return simpleFlags;
        }

        /// <summary>
        /// Gets the simple flags of the enumeration (i.e. non-zero constants who have no (defined) flags, other than themselves).
        /// Usually they are the single-bit flags.
        /// </summary>
        /// <value>The simple flags of the enumeration.</value>
        public static ReadOnlyList.Base<Enum<TEnum>> SimpleFlags
        {
            get
            {
                if( !DefinesFlags )
                    throw new InvalidOperationException("The specified enumeration does not have a FlagsAttribute!").Store("enumType", typeof(TEnum));

                if( simpleFlags.NullReference() )
                    Interlocked.CompareExchange(ref simpleFlags, new ReadOnlyList.Wrapper<Enum<TEnum>>(GetSimpleFlags()), null);

                return simpleFlags;
            }
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Implicitly unwraps the specified object.
        /// </summary>
        /// <param name="wrapper">The object to unwrap.</param>
        /// <returns>The unwrapped value.</returns>
        public static implicit operator TEnum( Enum<TEnum> wrapper )
        {
            return wrapper.value;
        }

        /// <summary>
        /// Implicitly wraps the specified object.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>The wrapped value.</returns>
        public static implicit operator Enum<TEnum>( TEnum value )
        {
            //// NOTE: this should technically be explicit, but let's assume our generic conditions are "close enough"...
            return new Enum<TEnum>(value);
        }

        #endregion

        #endregion

        #region IEquatable

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public /*virtual*/ bool Equals( Enum<TEnum> other )
        {
            /*
            // in case we're overriding
            if( !base.Equals( other ) )
                return false;
            */

            // might not need this, if the base has checked it (or if 'other' is a value type)
            /*if( other.NullReference() )
                return false;*/

            return CompiledOp.EnumEquals(this, other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals( TEnum other )
        {
            return CompiledOp.EnumEquals(this, other);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( Enum<TEnum> left, Enum<TEnum> right )
        {
            /*if( Object.ReferenceEquals(left, right) )
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
        public static bool operator !=( Enum<TEnum> left, Enum<TEnum> right )
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( Enum<TEnum> left, TEnum right )
        {
            return left == new Enum<TEnum>(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left  side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==( TEnum left, Enum<TEnum> right )
        {
            return new Enum<TEnum>(left) == right;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( Enum<TEnum> left, TEnum right )
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=( TEnum left, Enum<TEnum> right )
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare this object to.</param>
        /// <returns><c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.</returns>
        public override bool Equals( object other )
        {
            // for reference types
            /*var asNode = other as Measurement;

            if( asNode.NullReference() )
                return false;
            else
                return this.Equals(asNode);*/

            // for value types
            if( other is Enum<TEnum> )
                return this.Equals((Enum<TEnum>)other);
            else if( other is TEnum )
                return this.Equals((TEnum)other);
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
            return this.value.GetHashCode();
        }

        #endregion
    }

    /// <content>
    /// Methods extending the <see cref="Enum{T}"/> type.
    /// </content>
    public static partial class CoreExtensions
    {
        /// <summary>
        /// Returns the wrapped enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="value">The enumeration value to wrap.</param>
        /// <returns>The wrapped enumeration value.</returns>
        public static Enum<TEnum> Wrap<TEnum>( this TEnum value )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return value;
        }

        /// <summary>
        /// Returns the unwrapped enumeration value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="wrapper">The wrapper to unwrap.</param>
        /// <returns>The unwrapped enumeration value.</returns>
        public static TEnum Unwrap<TEnum>( this Enum<TEnum> wrapper )
            where TEnum : struct, IFormattable, IConvertible, IComparable
        {
            return wrapper;
        }
    }
}
