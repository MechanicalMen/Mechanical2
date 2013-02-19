using System;
using System.Threading;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// The base class of view-models.
    /// </summary>
    public class ViewModelBase : PropertyChangedBase.Disposable
    {
        #region Private Fields

        private PropertyChangedHandlers propertyChange;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        public ViewModelBase()
        {
        }

        #endregion

        #region IDisposableObject

        /// <summary>
        /// Called when the object is being disposed of. Inheritors must call base.OnDispose to be properly disposed.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, release both managed and unmanaged resources; otherwise release only the unmanaged resources.</param>
        protected override void OnDispose( bool disposing )
        {
            if( disposing )
            {
                //// dispose-only (i.e. non-finalizable) logic
                //// (managed, disposable resources you own)

                if( this.propertyChange.NotNullReference() )
                {
                    this.propertyChange.Dispose();
                    this.propertyChange = null;
                }
            }

            //// shared cleanup logic
            //// (unmanaged resources)

            base.OnDispose(disposing);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets a <see cref="PropertyChangedHandlers"/> instance.
        /// </summary>
        /// <value>The <see cref="PropertyChangedHandlers"/> instance.</value>
        protected PropertyChangedHandlers PropertyChange
        {
            get
            {
                Ensure.That(this).NotDisposed();

                if( this.propertyChange.NullReference() )
                    Interlocked.CompareExchange(ref this.propertyChange, new PropertyChangedHandlers(), comparand: null);

                return this.propertyChange;
            }
        }

        #endregion
    }
}
