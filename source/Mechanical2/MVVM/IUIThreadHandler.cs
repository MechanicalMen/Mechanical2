using System;
using System.Threading.Tasks;

namespace Mechanical.MVVM
{
    /// <summary>
    /// Exposes methods for running code on the UI thread of the implementing platform.
    /// </summary>
    public interface IUIThreadHandler
    {
        /// <summary>
        /// Determines whether the calling code is running on the UI thread.
        /// </summary>
        /// <returns><c>true</c> if the calling code is running on the UI thread; otherwise, <c>false</c>.</returns>
        bool IsOnUIThread();

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        void Invoke( Action action );

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        Task InvokeAsync( Action action );
    }
}
