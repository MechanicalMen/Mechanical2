using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mechanical.Conditions;
using Mechanical.Core;

namespace Mechanical.MVVM
{
    /// <summary>
    /// Implements <see cref="INotifyPropertyChanged"/>.
    /// Raises events on the UI thread.
    /// </summary>
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedBase"/> class.
        /// </summary>
        public PropertyChangedBase()
        {
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event synchronously on the UI thread.
        /// </summary>
        /// <param name="e">Specifies the property that changed.</param>
        protected void RaisePropertyChanged( PropertyChangedEventArgs e )
        {
            if( e.NullReference() )
                throw new ArgumentNullException().StoreDefault();

            UI.Invoke(() =>
            {
                var handler = this.PropertyChanged;
                if( handler.NotNullReference() )
                    handler(this, e);
            });
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event asynchronously on the UI thread.
        /// </summary>
        /// <param name="e">Specifies the property that changed.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        protected Task RaisePropertyChangedAsync( PropertyChangedEventArgs e )
        {
            if( e.NullReference() )
                throw new ArgumentNullException().StoreDefault();

            return UI.InvokeAsync(() =>
            {
                var handler = this.PropertyChanged;
                if( handler.NotNullReference() )
                    handler(this, e);
            });
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event synchronously on the UI thread.
        /// </summary>
        /// <param name="property">The name of the property that changed.</param>
        protected void RaisePropertyChanged( [CallerMemberName] string property = null )
        {
            if( property.NotNullReference() )
                throw new ArgumentNullException().StoreDefault();

            this.RaisePropertyChanged(new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event asynchronously on the UI thread.
        /// </summary>
        /// <param name="property">The name of the property that changed.</param>
        /// <returns>The <see cref="Task"/> representing the operation.</returns>
        protected Task RaisePropertyChangedAsync( [CallerMemberName] string property = null )
        {
            if( property.NotNullReference() )
                throw new ArgumentNullException().StoreDefault();

            return this.RaisePropertyChangedAsync(new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
