// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileSingleton.cs",
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
// "Librainian/Librainian/FileSingleton.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;
    using System.IO;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;
    using Measurement.Time;

    /// <summary>
    ///     Uses a named semaphore to allow only ONE of name.
    /// </summary>
    /// <example>using ( new FileSingleton( anyName ) ) { DoCode(); }</example>
    public class FileSingleton : ABetterClassDispose {

        // ReSharper disable once UnusedMember.Local
        private FileSingleton() {
            /* Disallow private contructor */
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of <paramref name="id" />.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
        public FileSingleton( Guid id ) {
            try {
                id.Should().NotBeEmpty();
                this.Snagged = false;
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: id.ToString( "D" ) );
                this.Snagged = this.Semaphore.WaitOne( Minutes.One );
            }
            catch ( Exception exception ) { exception.More(); }
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of name.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( info ) ) { DoCode(); }</example>
        public FileSingleton( FileSystemInfo name ) {
            name.Should().NotBeNull();

            try {
                this.Snagged = false;
                var encoded = name.FullName.Sha512().GetHexString();
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: encoded );
                this.Snagged = this.Semaphore.WaitOne( Minutes.One );
            }
            catch ( Exception exception ) { exception.More(); }
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of name.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( name ) ) { DoCode(); }</example>
        public FileSingleton( String name ) {
            name.Should().NotBeNull();

            try {
                this.Snagged = false;
                var encoded = name.Sha512().GetHexString();
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: encoded );
                this.Snagged = this.Semaphore.WaitOne( Minutes.One );
            }
            catch ( Exception exception ) { exception.More(); }
        }

        [CanBeNull]
        private Semaphore Semaphore { get; }

        public Boolean Snagged { get; private set; }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            if ( !this.Snagged ) { return; }

            var semaphore = this.Semaphore;

            if ( null == semaphore ) { return; }

            using ( semaphore ) {
                semaphore.Release();
                this.Snagged = false;
            }
        }
    }
}