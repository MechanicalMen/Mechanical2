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
        /// Gets the name of the object to test.
        /// </summary>
        /// <value>The name of the object to test.</value>
        string ObjectName { get; }

        /// <summary>
        /// Gets the full path to the source file where the test originated.
        /// </summary>
        /// <value>The full path to the source file where the test originated.</value>
        string FilePath { get; }

        /// <summary>
        /// Gets the method or property name of the caller to the method.
        /// </summary>
        /// <value>The method or property name of the caller to the method.</value>
        string MemberName { get; }

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
        private readonly string objName;
        private readonly string filePath;
        private readonly string memberName;
        private readonly int lineNumber;

        internal ConditionContext( T obj, string objName, string filePath, string memberName, int lineNumber )
        {
            this.obj = obj;
            this.objName = objName;
            this.filePath = filePath;
            this.memberName = memberName;
            this.lineNumber = lineNumber;
        }

        public T Object
        {
            get { return this.obj; }
        }

        public string ObjectName
        {
            get { return this.objName; }
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        public string MemberName
        {
            get { return this.memberName; }
        }

        public int LineNumber
        {
            get { return this.lineNumber; }
        }
    }

    #endregion
}
