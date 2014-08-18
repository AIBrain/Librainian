#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NinjectIocContainer.cs" was last cleaned by Rick on 2014/08/17 at 10:28 PM

#endregion License & Information

namespace Librainian.Magic {

    using System;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Ninject;
    using Ninject.Activation.Caching;
    using Ninject.Modules;
    using Threading;

    public sealed class NinjectIocContainer : IIocContainer {
        public NinjectIocContainer() {
            this.Kernel = new StandardKernel();

            this.Kernel.Should().NotBeNull();
            this.LoadAndBindInterfaces();
        }

        public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
            if ( modules == null ) {
                throw new ArgumentNullException( "modules" );
            }
            this.Kernel = new StandardKernel( modules );
            this.Kernel.Should().NotBeNull();
            this.LoadAndBindInterfaces();
        }

        public IKernel Kernel { get; set; }

        public object Get( Type type ) {
            return this.Kernel.Get( type );
        }

        public T Get<T>() {
            var bob = this.Kernel.TryGet<T>();
            return bob;
        }

        public T Get<T>( string name, string value ) {
            var result = this.Kernel.TryGet<T>( metadata => metadata.Has( name ) && ( string.Equals( metadata.Get<string>( name ), value, StringComparison.InvariantCultureIgnoreCase ) ) );

            if ( Equals( result, default( T ) ) ) {
                throw new InvalidOperationException( null );
            }
            return result;
        }

        public void Inject( object item ) {
            this.Kernel.Inject( item );
        }

        public T TryGet<T>() {
            return this.Kernel.TryGet<T>();
        }

        private void LoadAndBindInterfaces() {
            String.Format( "Wiring up Ninject..." ).TimeDebug();

            this.Kernel.Should().NotBeNull();

            //Kernel.Scan( scanner => {
            //    scanner.From( typeof( BusinessProcess ).Assembly );
            //    scanner.BindWith<DefaultBindingGenerator>();
            //} );

            //        this.Kernel.Bind( x => x
            //.From( this.GetType().Assembly )
            //.SelectAllClasses()
            //.InNamespaceOf( this.GetType() )
            //.BindAllInterfaces()
            //.Configure( b => b.InSingletonScope() )
            //);

            this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );
            String.Format( "Modules loaded: {0}", this.Kernel.GetModules().ToStrings() ).TimeDebug();

            this.Kernel.Should().NotBeNull();
            String.Format( "Wired up Ninject..." ).TimeDebug();
        }

        public void ResetKernel() {
            this.Kernel.Should().NotBeNull();
            this.Kernel.GetModules().ForEach( m => this.Kernel.Unload( m.Name ) );
            this.Kernel.Components.Get<ICache>().Clear();
            this.Kernel.Should().NotBeNull();

            this.LoadAndBindInterfaces();
        }
    }
}