namespace Librainian.IoCDI {
    using System;
    using Annotations;
    using Ninject;
    using Ninject.Modules;

    public class NinjectIocContainer : IIocContainer {
        public readonly IKernel Kernel;

        public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
            if ( modules == null )
                throw new ArgumentNullException( "modules" );
            this.Kernel = new StandardKernel( modules );
        }

        private NinjectIocContainer() {
            this.Kernel = new StandardKernel();
            this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );
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