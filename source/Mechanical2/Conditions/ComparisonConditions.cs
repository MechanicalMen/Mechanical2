using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mechanical.Conditions
{
    /// <content>
    /// Comparison conditions.
    /// </content>
    public static partial class ConditionsExtensions
    {
        #region Less

        private static Exception CreateLessException()
        {
            return new ArgumentException("The value is greater than or equal to the one specified!");
        }

        #region Less (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Less( this IConditionContext<sbyte> context, sbyte right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Less( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (byte)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Less( this IConditionContext<byte> context, byte right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Less( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (short)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Less( this IConditionContext<short> context, short right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Less( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (ushort)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Less( this IConditionContext<ushort> context, ushort right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Less( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (int)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Less( this IConditionContext<int> context, int right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Less( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (uint)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Less( this IConditionContext<uint> context, uint right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Less( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (long)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Less( this IConditionContext<long> context, long right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Less( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (ulong)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Less( this IConditionContext<ulong> context, ulong right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Less( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (float)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Less( this IConditionContext<float> context, float right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Less( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Less( this IConditionContext<float> context, float right, float maxError )
        {
            return Less(context, right, maxError, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Less( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object >= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Less (double)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Less( this IConditionContext<double> context, double right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Less( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Less( this IConditionContext<double> context, double right, double maxError )
        {
            return Less(context, right, maxError, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Less( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object >= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Less (decimal)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Less( this IConditionContext<decimal> context, decimal right )
        {
            return Less(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Less( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object >= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Less( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return Less(context, right, maxError, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Less( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object >= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Less (IComparable)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Less<T>( this IConditionContext<T> context, T right )
            where T : IComparable<T>
        {
            return Less<T>(context, right, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Less<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IComparable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.CompareTo(right) >= 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Less (IComparer)

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Less<T>( this IConditionContext<T> context, T right, IComparer<T> comparer )
        {
            return Less<T>(context, right, comparer, CreateLessException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Less<T>( this IConditionContext<T> context, T right, IComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( comparer.Compare(context.Object, right) >= 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #endregion

        #region LessOrEqual

        private static Exception CreateLessOrEqualException()
        {
            return new ArgumentException("The value is greater than the one specified!");
        }

        #region LessOrEqual (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> LessOrEqual( this IConditionContext<sbyte> context, sbyte right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> LessOrEqual( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (byte)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> LessOrEqual( this IConditionContext<byte> context, byte right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> LessOrEqual( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (short)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> LessOrEqual( this IConditionContext<short> context, short right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> LessOrEqual( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (ushort)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> LessOrEqual( this IConditionContext<ushort> context, ushort right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> LessOrEqual( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (int)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> LessOrEqual( this IConditionContext<int> context, int right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> LessOrEqual( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (uint)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> LessOrEqual( this IConditionContext<uint> context, uint right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> LessOrEqual( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (long)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> LessOrEqual( this IConditionContext<long> context, long right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> LessOrEqual( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (ulong)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> LessOrEqual( this IConditionContext<ulong> context, ulong right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> LessOrEqual( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (float)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> LessOrEqual( this IConditionContext<float> context, float right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> LessOrEqual( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> LessOrEqual( this IConditionContext<float> context, float right, float maxError )
        {
            return LessOrEqual(context, right, maxError, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> LessOrEqual( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object > right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (double)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> LessOrEqual( this IConditionContext<double> context, double right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> LessOrEqual( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> LessOrEqual( this IConditionContext<double> context, double right, double maxError )
        {
            return LessOrEqual(context, right, maxError, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> LessOrEqual( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object > right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (decimal)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> LessOrEqual( this IConditionContext<decimal> context, decimal right )
        {
            return LessOrEqual(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> LessOrEqual( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object > right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> LessOrEqual( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return LessOrEqual(context, right, maxError, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> LessOrEqual( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object > right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (IComparable)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> LessOrEqual<T>( this IConditionContext<T> context, T right )
            where T : IComparable<T>
        {
            return LessOrEqual<T>(context, right, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> LessOrEqual<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IComparable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.CompareTo(right) > 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region LessOrEqual (IComparer)

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> LessOrEqual<T>( this IConditionContext<T> context, T right, IComparer<T> comparer )
        {
            return LessOrEqual<T>(context, right, comparer, CreateLessOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is greater than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> LessOrEqual<T>( this IConditionContext<T> context, T right, IComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( comparer.Compare(context.Object, right) > 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #endregion

        #region Greater

        private static Exception CreateGreaterException()
        {
            return new ArgumentException("The value is less than or equal to the one specified!");
        }

        #region Greater (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Greater( this IConditionContext<sbyte> context, sbyte right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> Greater( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (byte)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Greater( this IConditionContext<byte> context, byte right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> Greater( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (short)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Greater( this IConditionContext<short> context, short right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> Greater( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (ushort)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Greater( this IConditionContext<ushort> context, ushort right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> Greater( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (int)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Greater( this IConditionContext<int> context, int right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> Greater( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (uint)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Greater( this IConditionContext<uint> context, uint right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> Greater( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (long)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Greater( this IConditionContext<long> context, long right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> Greater( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (ulong)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Greater( this IConditionContext<ulong> context, ulong right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> Greater( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (float)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Greater( this IConditionContext<float> context, float right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Greater( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Greater( this IConditionContext<float> context, float right, float maxError )
        {
            return Greater(context, right, maxError, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> Greater( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object <= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Greater (double)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Greater( this IConditionContext<double> context, double right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Greater( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Greater( this IConditionContext<double> context, double right, double maxError )
        {
            return Greater(context, right, maxError, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> Greater( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object <= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Greater (decimal)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Greater( this IConditionContext<decimal> context, decimal right )
        {
            return Greater(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Greater( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object <= right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Greater( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return Greater(context, right, maxError, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> Greater( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object <= right
             || Math.Abs(context.Object - right) <= maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region Greater (IComparable)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Greater<T>( this IConditionContext<T> context, T right )
            where T : IComparable<T>
        {
            return Greater<T>(context, right, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Greater<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IComparable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.CompareTo(right) <= 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region Greater (IComparer)

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Greater<T>( this IConditionContext<T> context, T right, IComparer<T> comparer )
        {
            return Greater<T>(context, right, comparer, CreateGreaterException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than or equal to the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> Greater<T>( this IConditionContext<T> context, T right, IComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( comparer.Compare(context.Object, right) <= 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #endregion

        #region GreaterOrEqual

        private static Exception CreateGreaterOrEqualException()
        {
            return new ArgumentException("The value is less than the one specified!");
        }

        #region GreaterOrEqual (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> GreaterOrEqual( this IConditionContext<sbyte> context, sbyte right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> GreaterOrEqual( this IConditionContext<sbyte> context, sbyte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (byte)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> GreaterOrEqual( this IConditionContext<byte> context, byte right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> GreaterOrEqual( this IConditionContext<byte> context, byte right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (short)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> GreaterOrEqual( this IConditionContext<short> context, short right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> GreaterOrEqual( this IConditionContext<short> context, short right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (ushort)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> GreaterOrEqual( this IConditionContext<ushort> context, ushort right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> GreaterOrEqual( this IConditionContext<ushort> context, ushort right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (int)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> GreaterOrEqual( this IConditionContext<int> context, int right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> GreaterOrEqual( this IConditionContext<int> context, int right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (uint)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> GreaterOrEqual( this IConditionContext<uint> context, uint right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> GreaterOrEqual( this IConditionContext<uint> context, uint right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (long)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> GreaterOrEqual( this IConditionContext<long> context, long right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> GreaterOrEqual( this IConditionContext<long> context, long right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (ulong)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> GreaterOrEqual( this IConditionContext<ulong> context, ulong right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> GreaterOrEqual( this IConditionContext<ulong> context, ulong right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (float)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> GreaterOrEqual( this IConditionContext<float> context, float right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> GreaterOrEqual( this IConditionContext<float> context, float right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> GreaterOrEqual( this IConditionContext<float> context, float right, float maxError )
        {
            return GreaterOrEqual(context, right, maxError, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> GreaterOrEqual( this IConditionContext<float> context, float right, float maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!float.IsNaN(maxError) && !float.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object < right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (double)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> GreaterOrEqual( this IConditionContext<double> context, double right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> GreaterOrEqual( this IConditionContext<double> context, double right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> GreaterOrEqual( this IConditionContext<double> context, double right, double maxError )
        {
            return GreaterOrEqual(context, right, maxError, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> GreaterOrEqual( this IConditionContext<double> context, double right, double maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(!double.IsNaN(maxError) && !double.IsInfinity(maxError) && maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object < right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (decimal)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> GreaterOrEqual( this IConditionContext<decimal> context, decimal right )
        {
            return GreaterOrEqual(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> GreaterOrEqual( this IConditionContext<decimal> context, decimal right, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object < right )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> GreaterOrEqual( this IConditionContext<decimal> context, decimal right, decimal maxError )
        {
            return GreaterOrEqual(context, right, maxError, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> GreaterOrEqual( this IConditionContext<decimal> context, decimal right, decimal maxError, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(maxError >= 0, arg => arg.IsTrue(CreateMaxErrorException));

            if( context.Object < right
             && Math.Abs(context.Object - right) > maxError )
                throw createException().Store(context).Store("right", right).Store("maxError", maxError);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (IComparable)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> GreaterOrEqual<T>( this IConditionContext<T> context, T right )
            where T : IComparable<T>
        {
            return GreaterOrEqual<T>(context, right, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> GreaterOrEqual<T>( this IConditionContext<T> context, T right, Func<Exception> createException )
            where T : IComparable<T>
        {
            Ensure.Debug(context, c => c.NotNull());

            if( context.Object.CompareTo(right) < 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #region GreaterOrEqual (IComparer)

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> GreaterOrEqual<T>( this IConditionContext<T> context, T right, IComparer<T> comparer )
        {
            return GreaterOrEqual<T>(context, right, comparer, CreateGreaterOrEqualException);
        }

        /// <summary>
        /// Throws an exception, if the argument is less than the one specified.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="right">The object to compare to.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> GreaterOrEqual<T>( this IConditionContext<T> context, T right, IComparer<T> comparer, Func<Exception> createException )
        {
            Ensure.Debug(context, c => c.NotNull());
            Ensure.Debug(comparer, c => c.NotNull());

            if( comparer.Compare(context.Object, right) < 0 )
                throw createException().Store(context).Store("right", right);
            else
                return context;
        }

        #endregion

        #endregion


        #region IsNegative

        #region IsNegative (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> IsNegative( this IConditionContext<sbyte> context )
        {
            return Less(context, (sbyte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> IsNegative( this IConditionContext<sbyte> context, Func<Exception> createException )
        {
            return Less(context, (sbyte)0, createException);
        }

        #endregion

        //// IsNegative (byte)

        #region IsNegative (short)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> IsNegative( this IConditionContext<short> context )
        {
            return Less(context, (short)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> IsNegative( this IConditionContext<short> context, Func<Exception> createException )
        {
            return Less(context, (short)0, createException);
        }

        #endregion

        //// IsNegative (ushort)

        #region IsNegative (int)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsNegative( this IConditionContext<int> context )
        {
            return Less(context, (int)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsNegative( this IConditionContext<int> context, Func<Exception> createException )
        {
            return Less(context, (int)0, createException);
        }

        #endregion

        //// IsNegative (uint)

        #region IsNegative (long)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> IsNegative( this IConditionContext<long> context )
        {
            return Less(context, 0L);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> IsNegative( this IConditionContext<long> context, Func<Exception> createException )
        {
            return Less(context, 0L, createException);
        }

        #endregion

        //// IsNegative (ulong)

        #region IsNegative (float)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsNegative( this IConditionContext<float> context )
        {
            return Less(context, 0f);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsNegative( this IConditionContext<float> context, Func<Exception> createException )
        {
            return Less(context, 0f, createException);
        }

        #endregion

        #region IsNegative (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsNegative( this IConditionContext<float> context, float maxError )
        {
            return Less(context, 0f, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsNegative( this IConditionContext<float> context, float maxError, Func<Exception> createException )
        {
            return Less(context, 0f, maxError, createException);
        }

        #endregion

        #region IsNegative (double)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsNegative( this IConditionContext<double> context )
        {
            return Less(context, 0d);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsNegative( this IConditionContext<double> context, Func<Exception> createException )
        {
            return Less(context, 0d, createException);
        }

        #endregion

        #region IsNegative (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsNegative( this IConditionContext<double> context, double maxError )
        {
            return Less(context, 0d, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsNegative( this IConditionContext<double> context, double maxError, Func<Exception> createException )
        {
            return Less(context, 0d, maxError, createException);
        }

        #endregion

        #region IsNegative (decimal)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsNegative( this IConditionContext<decimal> context )
        {
            return Less(context, 0m);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsNegative( this IConditionContext<decimal> context, Func<Exception> createException )
        {
            return Less(context, 0m, createException);
        }

        #endregion

        #region IsNegative (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsNegative( this IConditionContext<decimal> context, decimal maxError )
        {
            return Less(context, 0m, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsNegative( this IConditionContext<decimal> context, decimal maxError, Func<Exception> createException )
        {
            return Less(context, 0m, maxError, createException);
        }

        #endregion

        #endregion

        #region NonNegative

        #region NonNegative (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NonNegative( this IConditionContext<sbyte> context )
        {
            return GreaterOrEqual(context, (sbyte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> NonNegative( this IConditionContext<sbyte> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, (sbyte)0, createException);
        }

        #endregion

        //// NonNegative (byte)

        #region NonNegative (short)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NonNegative( this IConditionContext<short> context )
        {
            return GreaterOrEqual(context, (short)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> NonNegative( this IConditionContext<short> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, (short)0, createException);
        }

        #endregion

        //// NonNegative (ushort)

        #region NonNegative (int)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NonNegative( this IConditionContext<int> context )
        {
            return GreaterOrEqual(context, (int)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> NonNegative( this IConditionContext<int> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, (int)0, createException);
        }

        #endregion

        //// NonNegative (uint)

        #region NonNegative (long)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NonNegative( this IConditionContext<long> context )
        {
            return GreaterOrEqual(context, 0L);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> NonNegative( this IConditionContext<long> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0L, createException);
        }

        #endregion

        //// NonNegative (ulong)

        #region NonNegative (float)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonNegative( this IConditionContext<float> context )
        {
            return GreaterOrEqual(context, 0f);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonNegative( this IConditionContext<float> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0f, createException);
        }

        #endregion

        #region NonNegative (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonNegative( this IConditionContext<float> context, float maxError )
        {
            return GreaterOrEqual(context, 0f, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> NonNegative( this IConditionContext<float> context, float maxError, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0f, maxError, createException);
        }

        #endregion

        #region NonNegative (double)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonNegative( this IConditionContext<double> context )
        {
            return GreaterOrEqual(context, 0d);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonNegative( this IConditionContext<double> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0d, createException);
        }

        #endregion

        #region NonNegative (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonNegative( this IConditionContext<double> context, double maxError )
        {
            return GreaterOrEqual(context, 0d, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> NonNegative( this IConditionContext<double> context, double maxError, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0d, maxError, createException);
        }

        #endregion

        #region NonNegative (decimal)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonNegative( this IConditionContext<decimal> context )
        {
            return GreaterOrEqual(context, 0m);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonNegative( this IConditionContext<decimal> context, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0m, createException);
        }

        #endregion

        #region NonNegative (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonNegative( this IConditionContext<decimal> context, decimal maxError )
        {
            return GreaterOrEqual(context, 0m, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is negative.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> NonNegative( this IConditionContext<decimal> context, decimal maxError, Func<Exception> createException )
        {
            return GreaterOrEqual(context, 0m, maxError, createException);
        }

        #endregion

        #endregion

        #region IsPositive

        #region IsPositive (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> IsPositive( this IConditionContext<sbyte> context )
        {
            return Greater(context, (sbyte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> IsPositive( this IConditionContext<sbyte> context, Func<Exception> createException )
        {
            return Greater(context, (sbyte)0, createException);
        }

        #endregion

        #region IsPositive (byte)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> IsPositive( this IConditionContext<byte> context )
        {
            return Greater(context, (byte)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> IsPositive( this IConditionContext<byte> context, Func<Exception> createException )
        {
            return Greater(context, (byte)0, createException);
        }

        #endregion

        #region IsPositive (short)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> IsPositive( this IConditionContext<short> context )
        {
            return Greater(context, (short)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> IsPositive( this IConditionContext<short> context, Func<Exception> createException )
        {
            return Greater(context, (short)0, createException);
        }

        #endregion

        #region IsPositive (ushort)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> IsPositive( this IConditionContext<ushort> context )
        {
            return Greater(context, (ushort)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> IsPositive( this IConditionContext<ushort> context, Func<Exception> createException )
        {
            return Greater(context, (ushort)0, createException);
        }

        #endregion

        #region IsPositive (int)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsPositive( this IConditionContext<int> context )
        {
            return Greater(context, (int)0);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsPositive( this IConditionContext<int> context, Func<Exception> createException )
        {
            return Greater(context, (int)0, createException);
        }

        #endregion

        #region IsPositive (uint)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> IsPositive( this IConditionContext<uint> context )
        {
            return Greater(context, 0u);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> IsPositive( this IConditionContext<uint> context, Func<Exception> createException )
        {
            return Greater(context, 0u, createException);
        }

        #endregion

        #region IsPositive (long)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> IsPositive( this IConditionContext<long> context )
        {
            return Greater(context, 0L);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> IsPositive( this IConditionContext<long> context, Func<Exception> createException )
        {
            return Greater(context, 0L, createException);
        }

        #endregion

        #region IsPositive (ulong)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> IsPositive( this IConditionContext<ulong> context )
        {
            return Greater(context, 0UL);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> IsPositive( this IConditionContext<ulong> context, Func<Exception> createException )
        {
            return Greater(context, 0UL, createException);
        }

        #endregion

        #region IsPositive (float)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsPositive( this IConditionContext<float> context )
        {
            return Greater(context, 0f);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsPositive( this IConditionContext<float> context, Func<Exception> createException )
        {
            return Greater(context, 0f, createException);
        }

        #endregion

        #region IsPositive (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsPositive( this IConditionContext<float> context, float maxError )
        {
            return Greater(context, 0f, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> IsPositive( this IConditionContext<float> context, float maxError, Func<Exception> createException )
        {
            return Greater(context, 0f, maxError, createException);
        }

        #endregion

        #region IsPositive (double)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsPositive( this IConditionContext<double> context )
        {
            return Greater(context, 0d);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsPositive( this IConditionContext<double> context, Func<Exception> createException )
        {
            return Greater(context, 0d, createException);
        }

        #endregion

        #region IsPositive (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsPositive( this IConditionContext<double> context, double maxError )
        {
            return Greater(context, 0d, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> IsPositive( this IConditionContext<double> context, double maxError, Func<Exception> createException )
        {
            return Greater(context, 0d, maxError, createException);
        }

        #endregion

        #region IsPositive (decimal)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsPositive( this IConditionContext<decimal> context )
        {
            return Greater(context, 0m);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsPositive( this IConditionContext<decimal> context, Func<Exception> createException )
        {
            return Greater(context, 0m, createException);
        }

        #endregion

        #region IsPositive (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsPositive( this IConditionContext<decimal> context, decimal maxError )
        {
            return Greater(context, 0m, maxError);
        }

        /// <summary>
        /// Throws an exception, if the argument is non-positive.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> IsPositive( this IConditionContext<decimal> context, decimal maxError, Func<Exception> createException )
        {
            return Greater(context, 0m, maxError, createException);
        }

        #endregion

        #endregion

        #region InRange

        private static Exception CreateInRangeException<T>( T inclusiveLow, T exclusiveHigh )
        {
            return new ArgumentOutOfRangeException("The value is not in the specified range!", innerException: null).Store("inclusiveLow", inclusiveLow, filePath: null).Store("exclusiveHigh", exclusiveHigh, filePath: null);
        }

        #region InRange (sbyte)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> InRange( this IConditionContext<sbyte> context, sbyte inclusiveLow, sbyte exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<sbyte> InRange( this IConditionContext<sbyte> context, sbyte inclusiveLow, sbyte exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (byte)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> InRange( this IConditionContext<byte> context, byte inclusiveLow, byte exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<byte> InRange( this IConditionContext<byte> context, byte inclusiveLow, byte exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (short)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> InRange( this IConditionContext<short> context, short inclusiveLow, short exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<short> InRange( this IConditionContext<short> context, short inclusiveLow, short exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (ushort)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> InRange( this IConditionContext<ushort> context, ushort inclusiveLow, ushort exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ushort> InRange( this IConditionContext<ushort> context, ushort inclusiveLow, ushort exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (int)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> InRange( this IConditionContext<int> context, int inclusiveLow, int exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> InRange( this IConditionContext<int> context, int inclusiveLow, int exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (uint)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> InRange( this IConditionContext<uint> context, uint inclusiveLow, uint exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<uint> InRange( this IConditionContext<uint> context, uint inclusiveLow, uint exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (long)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> InRange( this IConditionContext<long> context, long inclusiveLow, long exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<long> InRange( this IConditionContext<long> context, long inclusiveLow, long exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (ulong)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> InRange( this IConditionContext<ulong> context, ulong inclusiveLow, ulong exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<ulong> InRange( this IConditionContext<ulong> context, ulong inclusiveLow, ulong exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (float)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> InRange( this IConditionContext<float> context, float inclusiveLow, float exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> InRange( this IConditionContext<float> context, float inclusiveLow, float exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (float, maxError)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> InRange( this IConditionContext<float> context, float inclusiveLow, float exclusiveHigh, float maxError )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, maxError, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<float> InRange( this IConditionContext<float> context, float inclusiveLow, float exclusiveHigh, float maxError, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, maxError, createException);
            return Less(context, exclusiveHigh, maxError, createException);
        }

        #endregion

        #region InRange (double)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> InRange( this IConditionContext<double> context, double inclusiveLow, double exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> InRange( this IConditionContext<double> context, double inclusiveLow, double exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (double, maxError)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> InRange( this IConditionContext<double> context, double inclusiveLow, double exclusiveHigh, double maxError )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, maxError, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<double> InRange( this IConditionContext<double> context, double inclusiveLow, double exclusiveHigh, double maxError, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, maxError, createException);
            return Less(context, exclusiveHigh, maxError, createException);
        }

        #endregion

        #region InRange (decimal)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> InRange( this IConditionContext<decimal> context, decimal inclusiveLow, decimal exclusiveHigh )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> InRange( this IConditionContext<decimal> context, decimal inclusiveLow, decimal exclusiveHigh, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, createException);
            return Less(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (decimal, maxError)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> InRange( this IConditionContext<decimal> context, decimal inclusiveLow, decimal exclusiveHigh, decimal maxError )
        {
            return InRange(context, inclusiveLow, exclusiveHigh, maxError, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="maxError">The maximum amount of error allowed.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<decimal> InRange( this IConditionContext<decimal> context, decimal inclusiveLow, decimal exclusiveHigh, decimal maxError, Func<Exception> createException )
        {
            GreaterOrEqual(context, inclusiveLow, maxError, createException);
            return Less(context, exclusiveHigh, maxError, createException);
        }

        #endregion

        #region InRange (IComparable)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> InRange<T>( this IConditionContext<T> context, T inclusiveLow, T exclusiveHigh )
            where T : IComparable<T>
        {
            return InRange<T>(context, inclusiveLow, exclusiveHigh, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> InRange<T>( this IConditionContext<T> context, T inclusiveLow, T exclusiveHigh, Func<Exception> createException )
            where T : IComparable<T>
        {
            GreaterOrEqual<T>(context, inclusiveLow, createException);
            return Less<T>(context, exclusiveHigh, createException);
        }

        #endregion

        #region InRange (IComparer)

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> InRange<T>( this IConditionContext<T> context, T inclusiveLow, T exclusiveHigh, IComparer<T> comparer )
        {
            return InRange<T>(context, inclusiveLow, exclusiveHigh, comparer, () => CreateInRangeException(inclusiveLow, exclusiveHigh));
        }

        /// <summary>
        /// Throws an exception, if the argument is not in the specified range.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="inclusiveLow">The inclusive lower boundary of the range.</param>
        /// <param name="exclusiveHigh">The exclusive upper boundary of the range.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<T> InRange<T>( this IConditionContext<T> context, T inclusiveLow, T exclusiveHigh, IComparer<T> comparer, Func<Exception> createException )
        {
            GreaterOrEqual<T>(context, inclusiveLow, comparer, createException);
            return Less<T>(context, exclusiveHigh, comparer, createException);
        }

        #endregion

        #endregion

        #region IsIndex

        #region IsIndex (count)

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="count">The number of items in the list or array.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex( this IConditionContext<int> context, int count )
        {
            return InRange(context, (int)0, count);
        }

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="count">The number of items in the list or array.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex( this IConditionContext<int> context, int count, Func<Exception> createException )
        {
            return InRange(context, (int)0, count, createException);
        }

        #endregion

        #region IsIndex (ICollection)

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="collection">The collection to test the index for.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, ICollection<T> collection )
        {
            return InRange(context, (int)0, collection.Count);
        }

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="collection">The collection to test the index for.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, ICollection<T> collection, Func<Exception> createException )
        {
            return InRange(context, (int)0, collection.Count, createException);
        }

        #endregion

        #region IsIndex (IReadOnlyCollection)

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="collection">The collection to test the index for.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, IReadOnlyCollection<T> collection )
        {
            return InRange(context, (int)0, collection.Count);
        }

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="collection">The collection to test the index for.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, IReadOnlyCollection<T> collection, Func<Exception> createException )
        {
            return InRange(context, (int)0, collection.Count, createException);
        }

        #endregion

        #region IsIndex (array)

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="array">The array to test the index for.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, T[] array )
        {
            return InRange(context, (int)0, array.Length);
        }

        /// <summary>
        /// Throws an exception, if the argument is not a valid index.
        /// </summary>
        /// <typeparam name="T">The type of the items in the array.</typeparam>
        /// <param name="context">An <see cref="IConditionContext{T}"/> instance.</param>
        /// <param name="array">The array to test the index for.</param>
        /// <param name="createException">A delegate used to create the exception, if needed.</param>
        /// <returns>The <see cref="IConditionContext{T}"/> instance.</returns>
        [DebuggerHidden]
        public static IConditionContext<int> IsIndex<T>( this IConditionContext<int> context, T[] array, Func<Exception> createException )
        {
            return InRange(context, (int)0, array.Length, createException);
        }

        #endregion

        #endregion
    }
}
