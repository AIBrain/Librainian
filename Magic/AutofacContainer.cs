//namespace Librainian.Magic {
//    using System;
//    using JetBrains.Annotations;
//    using Autofac;
//    using FluentAssertions;
//    using Ninject;

//    public class AutofacContainer : IIocContainer {

//        public AutofacContainer() {
//            this.ContainerBuilder = new ContainerBuilder();
//            this.ContainerBuilder.Should().NotBeNull();
//            this.ContainerBuilder.RegisterAssemblyModules( AppDomain.CurrentDomain.GetAssemblies() );    //BUG is this correct?
//            this.BuildedContainer  = this.ContainerBuilder.Build();
//        }

//        [Obsolete( "User BuildedContainer instead" )]
//        public IKernel Kernel { get; set; }

//        [NotNull]
//        public IContainer BuildedContainer { get; set; }

//        [NotNull]
//        public ContainerBuilder ContainerBuilder { get; set; }

//        [CanBeNull]
//        public object Get( Type type ) {
//            throw new NotImplementedException();
//        }

//        public T Get< T >() {
//            return this.BuildedContainer.Resolve<T>();
//        }

//        public T Get< T >( String name, String value ) {
//            throw new NotImplementedException();
//        }

//        public void Inject( object item ) {
//            throw new NotImplementedException();
//        }

//        public T TryGet< T >() {
//            return this.BuildedContainer.Resolve<T>();
//        }
//    }
//}