// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NinjectIocContainer.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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

    public sealed class NinjectIocContainer : ABetterClassDispose ,IIocContainer {

        // ReSharper disable once NotNullMemberIsNotInitialized
        public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
            if ( modules is null ) {
                throw new ArgumentNullException( nameof( modules ) );
            }
            this.Kernel.Should().BeNull();
            "Loading IoC kernel...".WriteColor( ConsoleColor.White, ConsoleColor.Blue );
            this.Kernel = new StandardKernel( modules );
            this.Kernel.Should().NotBeNull();
            if ( null == this.Kernel ) {
                throw new InvalidOperationException( "Unable to load kernel!" );
            }
            "done.".WriteLineColor( ConsoleColor.White, ConsoleColor.Blue );
        }

        public IKernel Kernel {
            get;
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
		        tryGet = this.Kernel.TryGet< TType >(); //HACK why would it work at the second time?
		        if ( Equals( default, tryGet ) ) {
			        throw new NullReferenceException( "Unable to TryGet() class " + typeof( TType ).FullName );
		        }
	        }
	        return tryGet;
        }

        public void Inject( Object item ) => this.Kernel.Inject( item );

        /// <summary>Warning!</summary>
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

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this.Kernel.Dispose();

	}
}