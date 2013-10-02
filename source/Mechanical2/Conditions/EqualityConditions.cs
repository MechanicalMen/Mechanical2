using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mechanical.Conditions
{
    /// <content>
    /// Equality conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        private static Exception CreateMaxErrorException()
        {
            return new ArgumentOutOfRangeException("The non-integer error specified is either NaN, +/- Infinity, or negative.");
        }

        #region Equal

        private static Exception CreateEqualException()
        {
            return new ArgumentException("The objects are not equal!");
        }

        #region Equal (sbyte)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Equal( this IConditionContext<sbyte> context, sbyte right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Equal( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (byte)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Equal( this IConditionContext<byte> context, byte right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Equal( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (short)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Equal( this IConditionContext<short> context, short right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Equal( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (ushort)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Equal( this IConditionContext<ushort> context, ushort right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Equal( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (int)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Equal( this IConditionContext<int> context, int right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Equal( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (uint)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Equal( this IConditionContext<uint> context, uint right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Equal( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (long)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Equal( this IConditionContext<long> context, long right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Equal( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (ulong)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Equal( this IConditionContext<ulong> context, ulong right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Equal( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (float)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Equal( this IConditionContext<float> context, float right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Equal( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (float, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Equal( this IConditionContext<float> context, float right, float maxError )
        {
            return Equal(context, right, maxError, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Equal( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (double)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Equal( this IConditionContext<double> context, double right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Equal( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (double, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Equal( this IConditionContext<double> context, double right, double maxError )
        {
            return Equal(context, right, maxError, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Equal( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (decimal)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Equal( this IConditionContext<decimal> context, decimal right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Equal( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object != right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Equal( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return Equal(context, right, maxError, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Equal( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (string)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> Equal( this IConditionContext<string> context, string right )
        {
            return Equal(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> Equal( this IConditionContext<string> context, string right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !string.Equals(context.Object, right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (string, StringComparison)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> Equal( this IConditionContext<string> context, string right, StringComparison comparisonType )
        {
            return Equal(context, right, comparisonType, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> Equal( this IConditionContext<string> context, string right, StringComparison comparisonType, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparisonType, c => c.IsDefined());

            if( !string.Equals(context.Object, right, comparisonType) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (IEquatable)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Equal<T>( this IConditionContext<T> context, T right )
            where T : IEquatable<T>
        {
            return Equal<T>(context, right, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Equal<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IEquatable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( !context.Object.Equals(right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region Equal (IEqualityComparer)

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Equal<T>( this IConditionContext<T> context, T right, IEqualityComparer<T> comparer )
        {
            return Equal<T>(context, right, comparer, CreateEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are not equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Equal<T>( this IConditionContext<T> context, T right, IEqualityComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( !comparer.Equals(context.Object, right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #endregion

        #region NotEqual

        private static Exception CreateNotEqualException()
        {
            return new ArgumentException("The objects are equal!");
        }

        #region NotEqual (sbyte)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NotEqual( this IConditionContext<sbyte> context, sbyte right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NotEqual( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (byte)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> NotEqual( this IConditionContext<byte> context, byte right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> NotEqual( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (short)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NotEqual( this IConditionContext<short> context, short right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NotEqual( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (ushort)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> NotEqual( this IConditionContext<ushort> context, ushort right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> NotEqual( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (int)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NotEqual( this IConditionContext<int> context, int right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NotEqual( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (uint)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> NotEqual( this IConditionContext<uint> context, uint right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> NotEqual( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (long)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NotEqual( this IConditionContext<long> context, long right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NotEqual( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (ulong)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> NotEqual( this IConditionContext<ulong> context, ulong right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> NotEqual( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (float)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotEqual( this IConditionContext<float> context, float right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotEqual( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (float, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotEqual( this IConditionContext<float> context, float right, float maxError )
        {
            return NotEqual(context, right, maxError, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotEqual( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (double)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotEqual( this IConditionContext<double> context, double right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotEqual( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (double, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotEqual( this IConditionContext<double> context, double right, double maxError )
        {
            return NotEqual(context, right, maxError, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotEqual( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (decimal)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NotEqual( this IConditionContext<decimal> context, decimal right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NotEqual( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object == right )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NotEqual( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return NotEqual(context, right, maxError, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NotEqual( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (string)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotEqual( this IConditionContext<string> context, string right )
        {
            return NotEqual(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotEqual( this IConditionContext<string> context, string right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( string.Equals(context.Object, right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (string, StringComparison)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotEqual( this IConditionContext<string> context, string right, StringComparison comparisonType )
        {
            return NotEqual(context, right, comparisonType, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the comparison.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<string> NotEqual( this IConditionContext<string> context, string right, StringComparison comparisonType, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparisonType, c => c.IsDefined());

            if( string.Equals(context.Object, right, comparisonType) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (IEquatable)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotEqual<T>( this IConditionContext<T> context, T right )
            where T : IEquatable<T>
        {
            return NotEqual<T>(context, right, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotEqual<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IEquatable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.Equals(right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #region NotEqual (IEqualityComparer)

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotEqual<T>( this IConditionContext<T> context, T right, IEqualityComparer<T> comparer )
        {
            return NotEqual<T>(context, right, comparer, CreateNotEqualException);
        }

        /// <summary>
        /// Throws an exception, if the arguments are equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to check equality with.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> NotEqual<T>( this IConditionContext<T> context, T right, IEqualityComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( comparer.Equals(context.Object, right) )
                throw createException().Store(context).Store("comparand", right);
            else
                return context;
        }

        #endregion

        #endregion


        #region NonZero

        #region NonZero (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NonZero( this IConditionContext<sbyte> context )
        {
            return NotEqual(context, (sbyte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NonZero( this IConditionContext<sbyte> context, Func<Exception> createException )
        {
            return NotEqual(context, (sbyte)0, createException);
        }

        #endregion

        #region NonZero (byte)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> NonZero( this IConditionContext<byte> context )
        {
            return NotEqual(context, (byte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> NonZero( this IConditionContext<byte> context, Func<Exception> createException )
        {
            return NotEqual(context, (byte)0, createException);
        }

        #endregion

        #region NonZero (short)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NonZero( this IConditionContext<short> context )
        {
            return NotEqual(context, (short)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NonZero( this IConditionContext<short> context, Func<Exception> createException )
        {
            return NotEqual(context, (short)0, createException);
        }

        #endregion

        #region NonZero (ushort)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> NonZero( this IConditionContext<ushort> context )
        {
            return NotEqual(context, (ushort)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> NonZero( this IConditionContext<ushort> context, Func<Exception> createException )
        {
            return NotEqual(context, (ushort)0, createException);
        }

        #endregion

        #region NonZero (int)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NonZero( this IConditionContext<int> context )
        {
            return NotEqual(context, (int)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NonZero( this IConditionContext<int> context, Func<Exception> createException )
        {
            return NotEqual(context, (int)0, createException);
        }

        #endregion

        #region NonZero (uint)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> NonZero( this IConditionContext<uint> context )
        {
            return NotEqual(context, 0u);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> NonZero( this IConditionContext<uint> context, Func<Exception> createException )
        {
            return NotEqual(context, 0u, createException);
        }

        #endregion

        #region NonZero (long)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NonZero( this IConditionContext<long> context )
        {
            return NotEqual(context, 0L);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NonZero( this IConditionContext<long> context, Func<Exception> createException )
        {
            return NotEqual(context, 0L, createException);
        }

        #endregion

        #region NonZero (ulong)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> NonZero( this IConditionContext<ulong> context )
        {
            return NotEqual(context, 0UL);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> NonZero( this IConditionContext<ulong> context, Func<Exception> createException )
        {
            return NotEqual(context, 0UL, createException);
        }

        #endregion

        #region NonZero (float)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonZero( this IConditionContext<float> context )
        {
            return NotEqual(context, 0f);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonZero( this IConditionContext<float> context, Func<Exception> createException )
        {
            return NotEqual(context, 0f, createException);
        }

        #endregion

        #region NonZero (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonZero( this IConditionContext<float> context, float maxError )
        {
            return NotEqual(context, 0f, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonZero( this IConditionContext<float> context, float maxError, Func<Exception> createException )
        {
            return NotEqual(context, 0f, maxError, createException);
        }

        #endregion

        #region NonZero (double)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonZero( this IConditionContext<double> context )
        {
            return NotEqual(context, 0d);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonZero( this IConditionContext<double> context, Func<Exception> createException )
        {
            return NotEqual(context, 0d, createException);
        }

        #endregion

        #region NonZero (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonZero( this IConditionContext<double> context, double maxError )
        {
            return NotEqual(context, 0d, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonZero( this IConditionContext<double> context, double maxError, Func<Exception> createException )
        {
            return NotEqual(context, 0d, maxError, createException);
        }

        #endregion

        #region NonZero (decimal)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonZero( this IConditionContext<decimal> context )
        {
            return NotEqual(context, 0m);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonZero( this IConditionContext<decimal> context, Func<Exception> createException )
        {
            return NotEqual(context, 0m, createException);
        }

        #endregion

        #region NonZero (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonZero( this IConditionContext<decimal> context, decimal maxError )
        {
            return NotEqual(context, 0m, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is zero.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonZero( this IConditionContext<decimal> context, decimal maxError, Func<Exception> createException )
        {
            return NotEqual(context, 0m, maxError, createException);
        }

        #endregion

        #endregion

        #region NotNaN

        //// NOTE: (From MSDN:) It is not possible to determine whether a value is not a number by comparing it to another value equal to NaN.

        #region NotNaN (float)

        /// <summary>
        /// Throws an exception, if the argument is NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotNaN( this IConditionContext<float> context )
        {
            return NotNaN(context, () => new ArgumentException("The argument is not a number (NaN)!"));
        }

        /// <summary>
        /// Throws an exception, if the argument is NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotNaN( this IConditionContext<float> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( float.IsNaN(context.Object) )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion

        #region NotNaN (double)

        /// <summary>
        /// Throws an exception, if the argument is NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotNaN( this IConditionContext<double> context )
        {
            return NotNaN(context, () => new ArgumentException("The argument is not a number (NaN)!"));
        }

        /// <summary>
        /// Throws an exception, if the argument is NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotNaN( this IConditionContext<double> context, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( double.IsNaN(context.Object) )
                throw createException().Store(context);
            else
                return context;
        }

        #endregion

        #endregion

        #region NotInfinity

        #region NotInfinity (float)

        /// <summary>
        /// Throws an exception, if the argument is positive or negative infinity.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotInfinity( this IConditionContext<float> context )
        {
            context = NotEqual(context, float.NegativeInfinity);
            return NotEqual(context, float.PositiveInfinity);
        }

        /// <summary>
        /// Throws an exception, if the argument is positive or negative infinity.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotInfinity( this IConditionContext<float> context, Func<Exception> createException )
        {
            context = NotEqual(context, float.NegativeInfinity, createException);
            return NotEqual(context, float.PositiveInfinity, createException);
        }

        #endregion

        #region NotInfinity (double)

        /// <summary>
        /// Throws an exception, if the argument is positive or negative infinity.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotInfinity( this IConditionContext<double> context )
        {
            context = NotEqual(context, double.NegativeInfinity);
            return NotEqual(context, double.PositiveInfinity);
        }

        /// <summary>
        /// Throws an exception, if the argument is positive or negative infinity.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotInfinity( this IConditionContext<double> context, Func<Exception> createException )
        {
            context = NotEqual(context, double.NegativeInfinity, createException);
            return NotEqual(context, double.PositiveInfinity, createException);
        }

        #endregion

        #endregion

        #region NotInfinityOrNaN

        #region NotInfinityOrNaN (float)

        /// <summary>
        /// Throws an exception, if the argument is (positive or negative) infinity or NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotInfinityOrNaN( this IConditionContext<float> context )
        {
            context = NotInfinity(context);
            return NotNaN(context);
        }

        /// <summary>
        /// Throws an exception, if the argument is (positive or negative) infinity or NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NotInfinityOrNaN( this IConditionContext<float> context, Func<Exception> createException )
        {
            context = NotInfinity(context, createException);
            return NotNaN(context, createException);
        }

        #endregion

        #region NotInfinityOrNaN (double)

        /// <summary>
        /// Throws an exception, if the argument is (positive or negative) infinity or NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotInfinityOrNaN( this IConditionContext<double> context )
        {
            context = NotInfinity(context);
            return NotNaN(context);
        }

        /// <summary>
        /// Throws an exception, if the argument is (positive or negative) infinity or NaN.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NotInfinityOrNaN( this IConditionContext<double> context, Func<Exception> createException )
        {
            context = NotInfinity(context, createException);
            return NotNaN(context, createException);
        }

        #endregion

        #endregion
    }
}
