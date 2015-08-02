using System;
using Mechanical.Core;

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
        /// Gets the source file position where the test originated.
        /// </summary>
        /// <value>The source file position where the test originated.</value>
        FileLine SourcePos { get; }
    }

    #region ConditionContext

    internal class ConditionContext<T> : IConditionContext<T>
    {
        private readonly T obj;
        private readonly string objName;
        private readonly FileLine srcPos;

        internal ConditionContext( T obj, string objName, FileLine srcPos )
        {
            this.obj = obj;
            this.objName = objName;
            this.srcPos = srcPos;
        }

        public T Object
        {
            get { return this.obj; }
        }

        public string ObjectName
        {
            get { return this.objName; }
        }

        public FileLine SourcePos
        {
            get { return this.srcPos; }
        }
    }

    #endregion
}
