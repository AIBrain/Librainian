// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "NinjectIocContainer.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "NinjectIocContainer.cs" was last formatted by Protiguous on 2019/12/10 at 7:15 AM.

namespace LibrainianCore.Magic {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Logging;
    using Utilities;

    public sealed class NinjectIocContainer : ABetterClassDispose, IIocContainer {

        public IKernel Kernel { get; }

        /// <summary>Returns a new instance of the given type or throws NullReferenceException.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        [DebuggerStepThrough]
        public T Get<T>() {
            var tryGet = this.Kernel.TryGet<T>();

            if ( Equals( default, tryGet ) ) {
                tryGet = this.Kernel.TryGet<T>(); //HACK why would it work at the second time?

                if ( Equals( default, tryGet ) ) {
                    throw new NullReferenceException( "Unable to TryGet() class " + typeof( T ).FullName );
                }
            }

            return tryGet;
        }

        public void Inject<T>( T item ) => this.Kernel.Inject( item );

        /// <summary>Warning!</summary>
        public void ResetKernel() {
            var bob = this.Kernel.GetModules().ForEach( module => this.Kernel.Unload( module.Name ) ).ToList();
            this.Kernel.Components.Get<ICache>().Clear();

            "Ninject is loading assemblies...".Log();
            this.Kernel.Load( AppDomain.CurrentDomain.GetAssemblies() );
            $"loaded {this.Kernel.GetModules().Count()} assemblies.".Log();
            $"{this.Kernel.GetModules().ToStrings()}".Log();
        }

        /// <summary>Re</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CanBeNull]
        [DebuggerStepThrough]
        public T TryGet<T>() {
            var tryGet = this.Kernel.TryGet<T>();

            if ( Equals( default, tryGet ) ) {
                tryGet = this.Kernel.TryGet<T>(); //HACK wtf??
            }

            return tryGet;
        }

        public NinjectIocContainer( [NotNull] params INinjectModule[] modules ) {
            try {
                "Loading IoC kernel...".Log();
                this.Kernel = new StandardKernel( modules );
            }
            finally {
                "Loading IoC kernel done.".Log();
            }
        }

        /// <summary>Dispose any disposable members.</summary>
        public override void DisposeManaged() {
            using ( this.Kernel ) { }
        }

    }

}