using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Allocators;
using Mechanical.MagicBag.Caches;
using Mechanical.MagicBag.Initializers;
using Mechanical.MagicBag.Parameters;

namespace Mechanical.MagicBag
{
    /// <summary>
    /// Declares the <see cref="Mapping"/> source.
    /// </summary>
    /// <typeparam name="TFrom">The type of the <see cref="Mapping"/> source.</typeparam>
    public static class Map<TFrom>
    {
        #region To

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> To<TTo>( IMagicBagAllocator<TTo> allocator )
            where TTo : TFrom
        {
            return new Builder<TTo>(allocator);
        }

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <param name="method">The method returning the object instance.</param>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> To<TTo>( Func<TTo> method )
            where TTo : TFrom
        {
            return To<TTo>(new DelegateAllocator<TTo>(method));
        }

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <param name="method">The method returning the object instance.</param>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> To<TTo>( Func<IMagicBag, TTo> method )
            where TTo : TFrom
        {
            return To<TTo>(new DelegateAllocator<TTo>(method));
        }

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <param name="ctor">The constructor to use.</param>
        /// <param name="parameters">The parameters to invoke the constructor with.</param>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> To<TTo>( ConstructorInfo ctor, Action<MagicParameters> parameters = null )
            where TTo : TFrom
        {
            return To<TTo>(new ConstructorAllocator<TTo>(ctor, MagicParameters.From(parameters)));
        }

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// The default constructor will be used.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> ToDefault<TTo>()
            where TTo : TFrom
        {
            return To<TTo>(ConstructorAllocator<TTo>.FromDefault());
        }

        /// <summary>
        /// Declares the <see cref="Mapping"/> target.
        /// All parameters will be resolved using a magic bag.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        /// <param name="ctor">The constructor to use.</param>
        /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
        public static Builder<TTo> ToInject<TTo>( ConstructorInfo ctor )
            where TTo : TFrom
        {
            return To<TTo>(ConstructorAllocator<TTo>.InjectAll(ctor));
        }

        #endregion

        /// <summary>
        /// Helps building <see cref="Mapping"/> instances.
        /// </summary>
        /// <typeparam name="TTo">The type of the <see cref="Mapping"/> target.</typeparam>
        public class Builder<TTo>
            where TTo : TFrom
        {
            #region Private Fields

            private readonly ParameterExpression magicBagExpr;
            private IMagicBagAllocator<TTo> allocator;
            private IMagicBagCache<TTo> cache;
            private List<IMagicBagInitializer<TTo>> initializers;

            #endregion

            #region Constructors

            internal Builder( IMagicBagAllocator<TTo> allocator )
            {
                Ensure.That(allocator).NotNull();

                this.magicBagExpr = Expression.Parameter(typeof(IMagicBag));
                this.allocator = allocator;
            }

            #endregion

            #region As

            /// <summary>
            /// Declares the <see cref="Mapping"/> cache.
            /// </summary>
            /// <param name="cache">The life span to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> As( IMagicBagCache<TTo> cache )
            {
                Ensure.That(cache).NotNull();
                Ensure.That(this.cache).Null(() => new InvalidOperationException("Cache already specified!"));

                this.cache = cache;
                return this;
            }

            /// <summary>
            /// Declares the <see cref="Mapping"/> cache.
            /// Allocates objects for each call.
            /// </summary>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> AsTransient()
            {
                return this.As(TransientCache<TTo>.Default);
            }

            /// <summary>
            /// Declares the <see cref="Mapping"/> cache.
            /// Allocates objects once per application (technically once per <see cref="AppDomain"/>).
            /// </summary>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> AsSingleton()
            {
                return this.As(new SingletonCache<TTo>());
            }

            /// <summary>
            /// Declares the <see cref="Mapping"/> cache.
            /// Allocates objects once per thread.
            /// </summary>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> AsThreadLocal()
            {
                return this.As(new ThreadLocalCache<TTo>());
            }

            #endregion

            #region Call

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="initializer">The initializer to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( IMagicBagInitializer<TTo> initializer )
            {
                Ensure.That(initializer).NotNull();

                if( this.initializers == null )
                    this.initializers = new List<IMagicBagInitializer<TTo>>();

                this.initializers.Add(initializer);
                return this;
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="init">The initializer to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( Action<TTo, IMagicBag> init )
            {
                return this.Call(new DelegateInitializer<TTo>(init));
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="init">The initializer to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( Action<TTo> init )
            {
                Ensure.That(init).NotNull();

                return this.Call(( allocated, bag ) => init(allocated));
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// The allocated object may be replaced.
            /// </summary>
            /// <param name="init">The initializer to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( Func<TTo, IMagicBag, TTo> init )
            {
                return this.Call(new DelegateInitializer<TTo>(init));
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// The allocated object may be replaced.
            /// </summary>
            /// <param name="init">The initializer to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( Func<TTo, TTo> init )
            {
                Ensure.That(init).NotNull();

                return this.Call(( allocated, bag ) => init(allocated));
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="method">The method to use.</param>
            /// <param name="parameters">The parameters to invoke the method with.</param>
            /// <param name="useMethodResult">If <c>true</c>, returns the method's return values instead of the allocated objects.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Call( MethodInfo method, Action<MagicParameters> parameters = null, bool useMethodResult = false )
            {
                return this.Call(new MethodInitializer<TTo>(method, MagicParameters.From(parameters), useMethodResult));
            }

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="method">The method to use.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> CallInject( MethodInfo method )
            {
                return this.Call(MethodInitializer<TTo>.InjectAll(method));
            }

            #endregion

            #region Set

            /// <summary>
            /// Adds an initializer to the <see cref="Mapping"/>.
            /// </summary>
            /// <param name="member">The field or property to assign to.</param>
            /// <param name="parameter">The parameter being assigned.</param>
            /// <returns>An object that helps building <see cref="Mapping"/> instances.</returns>
            public Builder<TTo> Set( MemberInfo member, Action<MagicParameters> parameter )
            {
                Ensure.That(parameter).NotNull();

                var p = MagicParameters.From(parameter);
                Ensure.That(p.Count).Equal(1, () => new ArgumentException("Only one parameter can initialize a field or property!"));

                return this.Call(new PropertyOrFieldInitializer<TTo>(member, p[0]));
            }

            #endregion

            #region ToMapping

            /// <summary>
            /// Creates a <see cref="Mapping"/> from this instance.
            /// </summary>
            /// <returns>The <see cref="Mapping"/> created.</returns>
            public Mapping ToMapping()
            {
                Ensure.That(this.allocator).NotNull(() => new InvalidOperationException("Allocator not specified!"));
                Ensure.That(this.cache).NotNull(() => new InvalidOperationException("Cache not specified!"));

                if( this.initializers == null
                 || this.initializers.Count == 0 )
                    return Mapping.Create<TFrom, TTo>(this.allocator, this.cache);
                else
                    return Mapping.Create<TFrom, TTo>(new InitializingAllocatorWrapper<TTo>(this.allocator, this.initializers.ToArray()), this.cache);
            }

            /// <summary>
            /// Converts <see cref="Builder{TTo}"/> instances to <see cref="Mapping"/> instances.
            /// </summary>
            /// <param name="mappingBuilder">The mapping builder to use.</param>
            /// <returns>The mapping created.</returns>
            public static implicit operator Mapping( Builder<TTo> mappingBuilder )
            {
                if( mappingBuilder.NullReference() )
                    return null;
                else
                    return mappingBuilder.ToMapping();
            }

            #endregion
        }
    }
}
