using System;
using System.Threading;
using System.Threading.Tasks;
using Mechanical.Conditions;

namespace Mechanical.Bootstrap
{
    /// <summary>
    /// Wraps each invoke in a <see cref="Task"/> using the specified <see cref="TaskScheduler"/>.
    /// Use <see cref="DispatcherUIHandler"/> if possible.
    /// This class was created for use in console applications.
    /// </summary>
    public class TaskSchedulerUIHandler : IUIThreadHandler
    {
        #region Private Fields

        private readonly TaskScheduler scheduler;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSchedulerUIHandler"/> class.
        /// </summary>
        /// <param name="taskScheduler">The <see cref="TaskScheduler"/> to wrap.</param>
        public TaskSchedulerUIHandler( TaskScheduler taskScheduler )
        {
            Ensure.That(taskScheduler).NotNull();

            this.scheduler = taskScheduler;
        }

        #endregion

        #region IUIThreadHandler

        /// <summary>
        /// Determines whether the calling code is running on the UI thread.
        /// </summary>
        /// <returns><c>true</c> if the calling code is running on the UI thread; otherwise, <c>false</c>.</returns>
        public bool IsOnUIThread()
        {
            return false; // no way to determine this, so assume "no"
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        public void Invoke( Action action )
        {
            this.InvokeAsync(action).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The delegate to invoke.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        public Task InvokeAsync( Action action )
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        #endregion
    }
}
