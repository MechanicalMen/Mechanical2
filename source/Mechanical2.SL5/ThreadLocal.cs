using System;
using System.Runtime.CompilerServices;

namespace System.Threading
{
    //// NOTE: based on:
    ////         http://ayende.com/blog/4825/an-elegant-threadlocal-for-silverlight

    /// <summary>
    /// Provides thread-local storage of data.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data stored per-thread.</typeparam>
    public class ThreadLocal<T>
    {
        private readonly Func<T> valueFactory;

        private class Holder
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "The encapsulating class is private.")]
            public T Val;
        }

        [ThreadStatic]
        private static ConditionalWeakTable<object, Holder> state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadLocal{T}"/> class.
        /// </summary>
        public ThreadLocal()
            : this(() => default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadLocal{T}"/> class.
        /// </summary>
        /// <param name="valueFactory">The <see cref="Func{TResult}"/> invoked to produce a lazily-initialized value when an attempt is made to retrieve <see cref="P:Value"/> without it having been previously initialized.</param>
        public ThreadLocal( Func<T> valueFactory )
        {
            this.valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets or sets the value of this instance for the current thread.
        /// </summary>
        /// <value>Returns an instance of the object that this ThreadLocal is responsible for initializing.</value>
        public T Value
        {
            get
            {
                Holder value;
                if( state == null || state.TryGetValue(this, out value) == false )
                {
                    var val = this.valueFactory();
                    this.Value = val;
                    return val;
                }
                return value.Val;
            }
            set
            {
                if( state == null )
                    state = new ConditionalWeakTable<object, Holder>();
                var holder = state.GetOrCreateValue(this);
                holder.Val = value;
            }
        }
    }
}
