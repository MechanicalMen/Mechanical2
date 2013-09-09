using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag;

namespace Mechanical.MVVM
{
    /* NOTE:
     *    - You specify properties either through Add, the indexer, or dynamically (e.g. 'instance.NewProperty = newValue;')
     *    - Property values may be objects, or they may be based on a Func<T> delegate, or an IMagicBag.Pull<T>()
     *
     *    - WPF can bind to dynamic properties (e.g. instance.NewProperty), but Silverlight can not
     *    - Both WPF and Silverlight can bind to the indexer (e.g. instance["NewProperty"])
     */

    /// <summary>
    /// Handles dynamic properties that can be bound to.
    /// Notifies of changes on the UI thread.
    /// </summary>
    public sealed class PropertyCollection : DynamicObject,
                                             IDictionary<string, object>,
                                             INotifyPropertyChanged
    {
        #region PropertyData

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "The encapsulating class is private.")]
        private class PropertyValue
        {
            private object obj;
            private Func<object> func;

            internal PropertyValue( string propertyName )
            {
                this.ChangedArgs = new PropertyChangedEventArgs(propertyName);
            }

            internal readonly PropertyChangedEventArgs ChangedArgs;

            internal object GetValue()
            {
                if( this.func.NullReference() )
                    return this.obj;
                else
                    return this.func();
            }

            internal void SetObject( object obj )
            {
                this.obj = obj;
                this.func = null;
            }

            internal void SetFunc( Func<object> func )
            {
                Ensure.Debug(func, f => f.NotNull());

                this.obj = null;
                this.func = func;
            }
        }

        #endregion

        #region ValueCollection

        private class ValueCollection : ICollection<object>
        {
            private readonly PropertyCollection collection;

            internal ValueCollection( PropertyCollection collection )
            {
                Ensure.Debug(collection, c => c.NotNull());

                this.collection = collection;
            }

            public void Add( object item )
            {
                throw new InvalidOperationException().StoreDefault();
            }

            public void Clear()
            {
                throw new InvalidOperationException().StoreDefault();
            }

            public bool Contains( object item )
            {
                foreach( var property in this.collection.properties.Values )
                {
                    if( object.Equals(item, property.GetValue()) )
                        return true;
                }

                return false;
            }

            public void CopyTo( object[] array, int arrayIndex )
            {
                ((IEnumerable<object>)this).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return this.collection.properties.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove( object item )
            {
                throw new InvalidOperationException().StoreDefault();
            }

            public IEnumerator<object> GetEnumerator()
            {
                foreach( var property in this.collection.properties.Values )
                    yield return property.GetValue();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion

        #region Private Fields

        private static readonly PropertyChangedEventArgs IndexerChangedArgs = new PropertyChangedEventArgs("Item[]");

        private readonly Dictionary<string, PropertyValue> properties;
        private readonly ValueCollection valueCollection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
        /// </summary>
        public PropertyCollection()
        {
            this.properties = new Dictionary<string, PropertyValue>(StringComparer.Ordinal);
            this.valueCollection = new ValueCollection(this);
        }

        #endregion

        #region Private Methods

        private void RaisePropertyChanged( PropertyChangedEventArgs e )
        {
            Ensure.Debug(e, arg => arg.NotNull());

            UI.Invoke(() =>
            {
                var handler = this.PropertyChanged;
                if( handler.NotNullReference() )
                    handler(this, e);
            });
        }

        private bool TryGetProperty( string property, out object value )
        {
            PropertyValue prop;
            if( this.properties.TryGetValue(property, out prop) )
            {
                value = prop.GetValue();
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private void SetProperty( string property, object newValue )
        {
            // NOTE: we always raise the PropertyChanged event, even if there was no change
            PropertyValue prop;
            if( !this.properties.TryGetValue(property, out prop) )
            {
                prop = new PropertyValue(property);
                this.properties.Add(property, prop);
            }

            if( newValue.NullReference() )
            {
                prop.SetObject(null);
            }
            else
            {
                var newType = newValue.GetType();
                if( newType.IsGenericType
                 && newType.GetGenericTypeDefinition() == typeof(Func<>) )
                {
                    // Func<T>
                    Func<object> func;
                    var genericArgs = newType.GetGenericArguments();
                    if( genericArgs[0] == typeof(object) )
                    {
                        func = (Func<object>)newValue;
                    }
                    else
                    {
                        var method = Reveal.Method(() => GetNonGenericFunc<int>(null));
                        method = method.GetGenericMethodDefinition();
                        method = method.MakeGenericMethod(newType.GetGenericArguments());

                        func = (Func<object>)method.Invoke(obj: null, parameters: new object[] { newValue });
                    }

                    prop.SetFunc(func);
                }
                else
                {
                    // not Func<T>
                    prop.SetObject(newValue);
                }
            }

            this.RaisePropertyChanged(prop.ChangedArgs);
            this.RaisePropertyChanged(IndexerChangedArgs);
        }

        private static Func<object> GetNonGenericFunc<T>( Func<T> f )
        {
            return () => f();
        }

        private static object Pull<T>( IMagicBag magicBag )
        {
            if( magicBag.IsRegistered<T>() )
                return magicBag.Pull<T>();
            else
                return null;
        }

        #endregion

        #region DynamicObject

        /// <summary>
        /// Provides the implementation for operations that get member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation.</param>
        /// <returns><c>true</c> if the operation is successful; otherwise, <c>false</c>.</returns>
        public override bool TryGetMember( GetMemberBinder binder, out object result )
        {
            return this.TryGetProperty(binder.Name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="value">The value to set to the member.</param>
        /// <returns><c>true</c> if the operation is successful; otherwise, <c>false</c>.</returns>
        public override bool TrySetMember( SetMemberBinder binder, object value )
        {
            this.SetProperty(binder.Name, value);
            return true;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDictionary

        /// <summary>
        /// Adds an element with the provided key and value to the dictionary.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add( string key, object value )
        {
            // let's emulate dictionary behaviour
            if( this.properties.ContainsKey(key) )
                throw new ArgumentException("Key already present!").Store("Key", key).Store("Value", value);

            this.SetProperty(key, value);
        }

        /// <summary>
        /// Determines whether the dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if the contains an element that has the specified key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey( string key )
        {
            return this.properties.ContainsKey(key);
        }

        /// <summary>
        /// Gets a collection that contains the keys in the dictionary.
        /// </summary>
        /// <value>A collection that contains the keys in the dictionary.</value>
        public ICollection<string> Keys
        {
            get { return this.properties.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="key"/> was not found in the original dictionary.</returns>
        public bool Remove( string key )
        {
            return this.properties.Remove(key);
        }

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">The value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns>When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</returns>
        public bool TryGetValue( string key, out object value )
        {
            return this.TryGetProperty(key, out value);
        }

        /// <summary>
        /// Gets a collection that contains the values in the dictionary.
        /// </summary>
        /// <value>A collection that contains the values in the dictionary.</value>
        public ICollection<object> Values
        {
            get { return this.valueCollection; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key]
        {
            get
            {
                object value;
                if( this.TryGetProperty(key, out value) )
                    return value;
                else
                    throw new KeyNotFoundException().Store("Key", key);
            }
            set
            {
                this.SetProperty(key, value);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        public void Add( KeyValuePair<string, object> item )
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            this.properties.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains( KeyValuePair<string, object> item )
        {
            return this.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            ((IEnumerable<KeyValuePair<string, object>>)this).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The number of elements in the collection.</value>
        public int Count
        {
            get { return this.properties.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{T}"/> is read-only.
        /// </summary>
        /// <value><c>true</c> if the <see cref="ICollection{T}"/> is read-only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <returns><c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found in the original <see cref="ICollection{T}"/>.</returns>
        public bool Remove( KeyValuePair<string, object> item )
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach( var pair in this.properties )
                yield return new KeyValuePair<string, object>(pair.Key, pair.Value.GetValue());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Adds a new property to the collection. The value is pulled from an <see cref="IMagicBag"/>.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="key">The name of the property to add.</param>
        /// <param name="magicBag">The <see cref="IMagicBag"/> to pull the value from.</param>
        public void Add<T>( string key, IMagicBag magicBag )
        {
            Ensure.That(magicBag).NotNull();

            // emulate dictionary behaviour
            if( this.properties.ContainsKey(key) )
                throw new ArgumentException("Key already present!").Store("Key", key);

            this.SetProperty(key, new Func<object>(() => Pull<T>(magicBag)));
        }

        #endregion
    }
}
