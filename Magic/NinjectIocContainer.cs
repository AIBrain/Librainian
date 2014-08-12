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
// "Librainian/NinjectIocContainer.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Magic {
    using System;
    using Annotations;
    using Ninject;
    using Ninject.Modules;
    using Threading;

    public class NinjectIocContainer : IIocContainer {

        [NotNull]
        public readonly IKernel Kernel;

        public NinjectIocContainer( [CanBeNull] params INinjectModule[] modules ) {
            String.Format( "Wiring up Ninject..." ).TimeDebug();
            this.Kernel = null == modules ? new StandardKernel() : new StandardKernel( modules );
            this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );
            String.Format( "Wired up Ninject..." ).TimeDebug();
        }

        public object Get( Type type ) {
            return this.Kernel.Get( type );
        }

        public T Get<T>() {
            return this.Kernel.Get<T>();
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
    }
}
