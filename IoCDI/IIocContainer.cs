namespace Librainian.IoCDI {
    using System;

    public interface IIocContainer {

        object Get( Type type );

        T Get<T>();

        T Get<T>( string name, string value );

        void Inject( object item );

        T TryGet<T>();
    }
}