using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mechanical.Core
{
    //// NOTE: code here needs to depend only on the .NET framework!

    /// <summary>
    /// Represents a point in 
    /// </summary>
    public class CallStackBuilder : IReadOnlyCollection<FileLine>
    {
        #region Private Fields

        private readonly Stack<FileLine> stack;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CallStackBuilder"/> class.
        /// </summary>
        public CallStackBuilder()
        {
            this.stack = new Stack<FileLine>();
        }

        #endregion

        #region IReadOnlyCollection

        /// <summary>
        /// Gets the number of items on the call stack.
        /// </summary>
        /// <value>The number of items on the call stack.</value>
        public int Count
        {
            get { return this.stack.Count; }
        }

        /// <summary>
        /// Returns an enumerator for the call stack.
        /// </summary>
        /// <returns>A new call stack enumerator.</returns>
        public IEnumerator<FileLine> GetEnumerator()
        {
            return this.stack.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator for the call stack.
        /// </summary>
        /// <returns>A new call stack enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified source file position to the call stack.
        /// </summary>
        /// <param name="sourcePos">The source file position to add.</param>
        public void Push( FileLine sourcePos )
        {
            this.stack.Push(sourcePos);
        }

        /// <summary>
        /// Adds the current source file position to the call stack.
        /// </summary>
        /// <param name="file">The source file that contains the caller.</param>
        /// <param name="member">The method or property name of the caller to this method.</param>
        /// <param name="line">The line number in the source file at which this method is called.</param>
        public void Push(
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0 )
        {
            this.Push(new FileLine(file, member, line));
        }

        /// <summary>
        /// Removes and returns the source file position at the top of the call stack.
        /// </summary>
        /// <returns>The source file position removed.</returns>
        public FileLine Pop()
        {
            if( this.stack.Count == 0 )
                throw new InvalidOperationException("The call stack is empty!");

            return this.stack.Pop();
        }

        /// <summary>
        /// Returns a string that represents this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach( var srcPos in this.stack )
            {
                srcPos.ToString(sb);
                sb.Append("\r\n"); // always use windows line terminator
            }
            return sb.ToString();
        }

        #endregion
    }
}
