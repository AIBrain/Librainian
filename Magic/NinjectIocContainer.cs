// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "NinjectIocContainer.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/NinjectIocContainer.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
    using Collections;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Ninject;
    using Ninject.Activation.Caching;
    using Ninject.Modules;

    public sealed class NinjectIocContainer : ABetterClassDispose, IIocContainer {

        public IKernel Kernel { get; }

        // ReSharper disable once NotNullMemberIsNotInitialized
        public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
            if ( modules is null ) { throw new ArgumentNullException( nameof( modules ) ); }

            this.Kernel.Should().BeNull();
            "Loading IoC kernel...".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
            this.Kernel = new StandardKernel( modules );
            this.Kernel.Should().NotBeNull();

            if ( null == this.Kernel ) { throw new InvalidOperationException( "Unable to load kernel!" ); }

            "done.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Kernel.Dispose();

        /// <summary>
        ///     Returns a new instance of the given type or throws NullReferenceException.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        [DebuggerStepThrough]
        public TType Get<TType>() {
            var tryGet = this.Kernel.TryGet<TType>();

            if ( Equals( default, tryGet ) ) {
                tryGet = this.Kernel.TryGet<TType>(); //HACK why would it work at the second time?

                if ( Equals( default, tryGet ) ) { throw new NullReferenceException( "Unable to TryGet() class " + typeof( TType ).FullName ); }
            }

            return tryGet;
        }

        public void Inject( Object item ) => this.Kernel.Inject( item );

        /// <summary>
        ///     Warning!
        /// </summary>
        public void ResetKernel() {
            this.Kernel.Should().NotBeNull();
            this.Kernel.GetModules().ForEach( module => this.Kernel.Unload( module.Name ) );
            this.Kernel.Components.Get<ICache>().Clear();
            this.Kernel.Should().NotBeNull();

            //Log.Before( "Ninject is loading assemblies..." );
            this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );

            //Log.After( $"loaded {this.Kernel.GetModules().Count()} assemblies." );
            $"{this.Kernel.GetModules().ToStrings()}".WriteLine();
        }

        /// <summary>
        ///     Re
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public TType TryGet<TType>() {
            var tryGet = this.Kernel.TryGet<TType>();

            if ( Equals( default, tryGet ) ) {
                tryGet = this.Kernel.TryGet<TType>(); //HACK wtf??
            }

            return tryGet;
        }

        //public object Get( Type type ) {
        //    return this.Kernel.Get( type );
        //}

        //public T Get<T>() {
        //    var bob = this.Kernel.TryGet<T>();
        //    return bob;
        //}

        //public T Get<T>( String name, String value ) {
        //    var result = this.Kernel.TryGet<T>( metadata => metadata.Has( name ) && metadata.Get<String>( name ).Like( value ) );

        //    if ( Equals( result, default( T ) ) ) {
        //        throw new InvalidOperationException( null );
        //    }
        //    return result;
        //}
    }
}