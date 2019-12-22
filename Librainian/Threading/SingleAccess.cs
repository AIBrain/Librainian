// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "SingleAccess.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "SingleAccess.cs" was last formatted by Protiguous on 2019/08/08 at 9:38 AM.

namespace Librainian.Threading {

    using System;
    using System.IO;
    using System.Threading;
    using JetBrains.Annotations;
    using Logging;
    using Measurement.Time;
    using OperatingSystem.FileSystem;
    using Persistence;
    using Security;
    using Utilities;

    /// <summary>
    ///     Uses a named <see cref="Semaphore" /> to allow only 1 access to "name".
    ///     <para></para>
    /// </summary>
    /// <example>
    ///     <code>using ( new SingleAccess( anyName ) ) { DoCode(); }</code>
    /// </example>
    public class SingleAccess : ABetterClassDispose {

        [CanBeNull]
        private Semaphore Semaphore { get; }

        public Boolean Snagged { get; private set; }

        private SingleAccess() {
            /* Disallow private contructor */
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of <paramref name="id" />.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
        public SingleAccess( Guid id, TimeSpan? timeout = null ) {
            try {
                if ( !timeout.HasValue ) {
                    timeout = Minutes.One;
                }

                this.Snagged = false;
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: id.ToString( "D" ) );
                this.Snagged = this.Semaphore.WaitOne( timeout.Value );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of <paramref name="name" />.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( info ) ) { DoCode(); }</example>
        public SingleAccess( [NotNull] FileSystemInfo name, TimeSpan? timeout = null ) {
            if ( name is null ) {
                throw new ArgumentNullException( paramName: nameof( name ) );
            }

            try {
                if ( !timeout.HasValue ) {
                    timeout = Minutes.One;
                }

                this.Snagged = false;
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: name.FullName.GetMD5Hash() );
                this.Snagged = this.Semaphore.WaitOne( timeout.Value );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of <paramref name="name" />.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( name ) ) { DoCode(); }</example>
        public SingleAccess( [CanBeNull] String name, TimeSpan? timeout = null ) {

            try {
                if ( !timeout.HasValue ) {
                    timeout = Minutes.One;
                }

                this.Snagged = false;
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: name.GetMD5Hash() );
                this.Snagged = this.Semaphore.WaitOne( timeout.Value );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        public SingleAccess( [NotNull] IDocument document, TimeSpan? timeout = null ) : this( name: document.FullPath, timeout: timeout ) { }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            if ( !this.Snagged ) {
                return;
            }

            using ( this.Semaphore ) {
                try {
                    this.Semaphore?.Release();
                }
                finally {
                    this.Snagged = false;
                }
            }
        }
    }

    /// <summary>
    ///     Uses a named <see cref="Semaphore" /> to allow only 1 access to "self".
    ///     <para></para>
    /// </summary>
    /// <example>
    ///     <code>using ( new SingleAccess( anyName ) ) { DoCode(); }</code>
    /// </example>
    public class SingleAccess<T> : ABetterClassDispose {

        [CanBeNull]
        private Semaphore Semaphore { get; }

        public Boolean Snagged { get; private set; }

        private SingleAccess() {
            /* Disallow private contructor */
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of <paramref name="self" />.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
        public SingleAccess( [CanBeNull] T self, TimeSpan? timeout = null ) {
            try {
                if ( !timeout.HasValue ) {
                    timeout = Minutes.One;
                }

                this.Snagged = false;
                var hex = self.Serializer()?.ToHexString();
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: hex ); //what happens on a null?
                this.Snagged = this.Semaphore.WaitOne( timeout.Value );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            if ( !this.Snagged ) {
                return;
            }

            using ( this.Semaphore ) {
                try {
                    this.Semaphore?.Release();
                }
                finally {
                    this.Snagged = false;
                }
            }
        }
    }
}