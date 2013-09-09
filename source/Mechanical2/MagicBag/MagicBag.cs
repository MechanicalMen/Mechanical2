using System;
using System.Collections.Generic;
using System.Threading;
using Mechanical.Collections;
using Mechanical.Conditions;
using Mechanical.Core;
using Mechanical.MagicBag.Generators;

namespace Mechanical.MagicBag
{
    /// <summary>
    /// A basic IoC container.
    /// </summary>
    public static class MagicBag
    {
        #region Basic

        /// <summary>
        /// A simple magic bag that is based solely on mappings.
        /// </summary>
        public class Basic : IMagicBag
        {
            #region Private Fields

            private readonly Dictionary<Type, Mapping> mappings;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Basic"/> class.
            /// </summary>
            /// <param name="mappings">The mappings this magic bag directly consists of.</param>
            public Basic( params Mapping[] mappings )
                : this(mappings, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Basic"/> class.
            /// </summary>
            /// <param name="mappings">The mappings this magic bag directly consists of.</param>
            /// <param name="generators">The generators extending the mappings to be used.</param>
            public Basic( Mapping[] mappings, params IMappingGenerator[] generators )
            {
                this.mappings = new Dictionary<Type, Mapping>();

                if( mappings.NotNullReference() )
                {
                    foreach( var m in mappings )
                    {
                        // NOTE: In case of ambiguity, this first mapping wins.
                        //       The rest may be accessed through the use of a generator.
                        if( !this.mappings.ContainsKey(m.From) )
                            this.mappings.Add(m.From, m);
                    }

                    if( generators.NotNullReference() )
                    {
                        foreach( var g in generators )
                        {
                            foreach( var m in g.Generate(mappings) )
                            {
                                // NOTE: we don't want to overwrite the (directly) specified mappings!
                                if( !this.mappings.ContainsKey(m.From) )
                                    this.mappings.Add(m.From, m);
                            }
                        }
                    }
                }
            }

            #endregion

            #region IMagicBag

            /// <summary>
            /// Resolves the specified type.
            /// </summary>
            /// <typeparam name="T">The type to resolve.</typeparam>
            /// <returns>An object of type <typeparamref name="T"/>.</returns>
            public virtual T Pull<T>()
            {
                Mapping mapping;
                if( this.mappings.TryGetValue(typeof(T), out mapping) )
                    return (T)mapping.Get(this);
                else
                    throw new KeyNotFoundException().Store("T", typeof(T));
            }

            /// <summary>
            /// Determines whether the specified type is registered into the magic bag.
            /// Does not guarantee that the type can be resolved.
            /// </summary>
            /// <typeparam name="T">The type to check.</typeparam>
            /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
            public virtual bool IsRegistered<T>()
            {
                return this.mappings.ContainsKey(typeof(T));
            }

            #endregion
        }

        #endregion

        #region Inherit

        /// <summary>
        /// A magic bag that looks at it's mappings first, before falling back to it's parent.
        /// </summary>
        public class Inherit : Basic
        {
            #region Private Fields

            private readonly IMagicBag parent;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Inherit"/> class.
            /// </summary>
            /// <param name="parent">The parent to inherit from.</param>
            /// <param name="mappings">The mappings to use for this magic bag.</param>
            public Inherit( IMagicBag parent, params Mapping[] mappings )
                : base(mappings)
            {
                Ensure.That(parent).NotNull();

                this.parent = parent;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Inherit"/> class.
            /// </summary>
            /// <param name="parent">The parent to inherit from.</param>
            /// <param name="mappings">The mappings to use for this magic bag.</param>
            /// <param name="generators">The generators extending the mappings to be used.</param>
            public Inherit( IMagicBag parent, Mapping[] mappings, params IMappingGenerator[] generators )
                : base(mappings, generators)
            {
                Ensure.That(parent).NotNull();

                this.parent = parent;
            }

            #endregion

            #region IMagicBag

            /// <summary>
            /// Resolves the specified type.
            /// </summary>
            /// <typeparam name="T">The type to resolve.</typeparam>
            /// <returns>An object of type <typeparamref name="T"/>.</returns>
            public override T Pull<T>()
            {
                if( base.IsRegistered<T>() )
                    return base.Pull<T>();
                else
                    return this.parent.Pull<T>();
            }

            /// <summary>
            /// Determines whether the specified type is registered into the magic bag.
            /// Does not guarantee that the type can be resolved.
            /// </summary>
            /// <typeparam name="T">The type to check.</typeparam>
            /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
            public override bool IsRegistered<T>()
            {
                return base.IsRegistered<T>() || this.parent.IsRegistered<T>();
            }

            #endregion
        }

        #endregion

        #region Supplement

        /// <summary>
        /// A magic bag that looks at it's parent first, before falling back to it's mappings.
        /// </summary>
        public class Supplement : Basic
        {
            #region Private Fields

            private readonly IMagicBag parent;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Supplement"/> class.
            /// </summary>
            /// <param name="parent">The parent to supplement.</param>
            /// <param name="mappings">The mappings to use for this magic bag.</param>
            public Supplement( IMagicBag parent, params Mapping[] mappings )
                : base(mappings)
            {
                Ensure.That(parent).NotNull();

                this.parent = parent;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Supplement"/> class.
            /// </summary>
            /// <param name="parent">The parent to supplement.</param>
            /// <param name="mappings">The mappings to use for this magic bag.</param>
            /// <param name="generators">The generators extending the mappings to be used.</param>
            public Supplement( IMagicBag parent, Mapping[] mappings, params IMappingGenerator[] generators )
                : base(mappings, generators)
            {
                Ensure.That(parent).NotNull();

                this.parent = parent;
            }

            #endregion

            #region IMagicBag

            /// <summary>
            /// Resolves the specified type.
            /// </summary>
            /// <typeparam name="T">The type to resolve.</typeparam>
            /// <returns>An object of type <typeparamref name="T"/>.</returns>
            public override T Pull<T>()
            {
                if( this.parent.IsRegistered<T>() )
                    return this.parent.Pull<T>();
                else
                    return base.Pull<T>();
            }

            /// <summary>
            /// Determines whether the specified type is registered into the magic bag.
            /// Does not guarantee that the type can be resolved.
            /// </summary>
            /// <typeparam name="T">The type to check.</typeparam>
            /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
            public override bool IsRegistered<T>()
            {
                return this.parent.IsRegistered<T>() || base.IsRegistered<T>();
            }

            #endregion
        }

        #endregion

        #region Blacklist

        /// <summary>
        /// Exposes all but a certain set of types from it's parent.
        /// </summary>
        public class Blacklist : IMagicBag
        {
            #region Private Fields

            private readonly IMagicBag parent;
            private readonly HashSet<Type> blacklist;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Blacklist"/> class.
            /// </summary>
            /// <param name="parent">The parent to wrap.</param>
            /// <param name="types">The types to blacklist.</param>
            public Blacklist( IMagicBag parent, params Type[] types )
            {
                Ensure.That(parent).NotNull();
                Ensure.That(types).NotNullOrSparse();

                this.parent = parent;

                this.blacklist = new HashSet<Type>();
                foreach( var t in types )
                {
                    if( !this.blacklist.Contains(t) )
                        this.blacklist.Add(t);
                }
            }

            #endregion

            #region IMagicBag

            /// <summary>
            /// Resolves the specified type.
            /// </summary>
            /// <typeparam name="T">The type to resolve.</typeparam>
            /// <returns>An object of type <typeparamref name="T"/>.</returns>
            public T Pull<T>()
            {
                if( this.blacklist.Contains(typeof(T)) )
                    throw new KeyNotFoundException().Store("T", typeof(T));
                else
                    return this.parent.Pull<T>();
            }

            /// <summary>
            /// Determines whether the specified type is registered into the magic bag.
            /// Does not guarantee that the type can be resolved.
            /// </summary>
            /// <typeparam name="T">The type to check.</typeparam>
            /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
            public bool IsRegistered<T>()
            {
                if( this.blacklist.Contains(typeof(T)) )
                    return false;
                else
                    return this.parent.IsRegistered<T>();
            }

            #endregion
        }

        #endregion

        #region Whitelist

        /// <summary>
        /// Exposes none but a certain set of types from it's parent.
        /// </summary>
        public class Whitelist : IMagicBag
        {
            #region Private Fields

            private readonly IMagicBag parent;
            private readonly HashSet<Type> whitelist;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Whitelist"/> class.
            /// </summary>
            /// <param name="parent">The parent to wrap.</param>
            /// <param name="types">The types to whitelist.</param>
            public Whitelist( IMagicBag parent, params Type[] types )
            {
                Ensure.That(parent).NotNull();
                Ensure.That(types).NotNullOrSparse();

                this.parent = parent;

                this.whitelist = new HashSet<Type>();
                foreach( var t in types )
                {
                    if( !this.whitelist.Contains(t) )
                        this.whitelist.Add(t);
                }
            }

            #endregion

            #region IMagicBag

            /// <summary>
            /// Resolves the specified type.
            /// </summary>
            /// <typeparam name="T">The type to resolve.</typeparam>
            /// <returns>An object of type <typeparamref name="T"/>.</returns>
            public T Pull<T>()
            {
                if( this.whitelist.Contains(typeof(T)) )
                    return this.parent.Pull<T>();
                else
                    throw new KeyNotFoundException().Store("T", typeof(T));
            }

            /// <summary>
            /// Determines whether the specified type is registered into the magic bag.
            /// Does not guarantee that the type can be resolved.
            /// </summary>
            /// <typeparam name="T">The type to check.</typeparam>
            /// <returns><c>true</c> if type <typeparamref name="T"/> is registered; otherwise <c>false</c>.</returns>
            public bool IsRegistered<T>()
            {
                if( this.whitelist.Contains(typeof(T)) )
                    return this.parent.IsRegistered<T>();
                else
                    return false;
            }

            #endregion
        }

        #endregion


        #region Default

        private static IMagicBag defaultBag = null;

        internal static void CreateDefault( IMagicBag parentBag )
        {
            Bootstrap.ThrowIfAlreadyInitialized();

            // NOTE: specify default mappings here
            var mappings = new List<Mapping>();
            mappings.AddRange(Mechanical.DataStores.BasicSerialization.GetMappings());
            mappings.AddRange(Mechanical.DataStores.Node.DataStoreNode.GetMappings());

            if( parentBag.NullReference() )
                Interlocked.CompareExchange(ref defaultBag, new Basic(mappings.ToArray(), MappingGenerators.Defaults), comparand: null);
            else
                Interlocked.CompareExchange(ref defaultBag, new Supplement(parentBag, mappings.ToArray(), MappingGenerators.Defaults), comparand: null);
        }

        /// <summary>
        /// Gets the default <see cref="IMagicBag"/>. It supplements the magic bag the library was initialized with, using default mappings.
        /// </summary>
        public static IMagicBag Default
        {
            get
            {
                Bootstrap.ThrowIfUninitialized();

                return defaultBag;
            }
        }

        #endregion
    }
}
