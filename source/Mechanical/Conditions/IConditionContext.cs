using System;

namespace Mechanical.Conditions
{
    //// NOTE: we want to be as independent of all other namespaces as possible.
    //// NOTE: we need to return an interface, instead of simply a class,
    ////       to get the benefit of covariance (which is necessary to get some things to work).

    /// <summary>
    /// An immutable type encapsulating information about an object being tested.
    /// </summary>
    /// <typeparam name="T">The type of the object to be validated.</typeparam>
    public interface IConditionContext<out T>
    {
        /// <summary>
        /// Gets the object to test.
        /// </summary>
        /// <value>The object to test.</value>
        T Object { get; }

        /// <summary>
        /// Gets the full path to the source file where the test originated.
        /// </summary>
        /// <value>The full path to the source file where the test originated.</value>
        string FilePath { get; }

        /// <summary>
        /// Gets the line in the source file where the test originated.
        /// </summary>
        /// <value>The line in the source file where the test originated.</value>
        int LineNumber { get; }
    }

    #region ConditionContext

    internal class ConditionContext<T> : IConditionContext<T>
    {
        private readonly T obj;
        private readonly string filePath;
        private readonly int lineNumber;

        internal ConditionContext( T obj, string filePath, int lineNumber )
        {
            this.obj = obj;
            this.filePath = filePath;
            this.lineNumber = lineNumber;
        }

        public T Object
        {
            get { return this.obj; }
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        public int LineNumber
        {
            get { return this.lineNumber; }
        }
    }

    #endregion
}
