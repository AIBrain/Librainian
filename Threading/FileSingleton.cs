// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FileSingleton.cs" was last cleaned by Protiguous on 2018/05/09 at 1:14 PM

namespace Librainian.Threading {

    using System;
    using System.IO;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Magic;
    using Measurement.Time;
    using Security;

    /// <summary>
    /// Uses a named semaphore to allow only ONE of name.
    /// </summary>
    /// <example>using ( new FileSingleton( anyName ) ) { DoCode(); }</example>
    public class FileSingleton : ABetterClassDispose {

        // ReSharper disable once UnusedMember.Local
        private FileSingleton() {
            /* Disallow private contructor */
        }

        /// <summary>
        /// Uses a named semaphore to allow only ONE of <paramref name="id"/>.
        /// </summary>
        /// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
        public FileSingleton( Guid id ) {
            try {
                id.Should().NotBeEmpty();
                this.Snagged = false;
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: id.ToString( "D" ) );
                this.Snagged = this.Semaphore.WaitOne( Minutes.One );
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        /// <summary>
        /// Uses a named semaphore to allow only ONE of name.
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
            catch ( Exception exception ) {
                exception.More();
            }
        }

        /// <summary>
        /// Uses a named semaphore to allow only ONE of name.
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
            catch ( Exception exception ) {
                exception.More();
            }
        }

        [CanBeNull]
        private Semaphore Semaphore { get; }

        public Boolean Snagged { get; private set; }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            if ( !this.Snagged ) {
                return;
            }

            var semaphore = this.Semaphore;
            if ( null == semaphore ) {
                return;
            }

            using ( semaphore ) {
                semaphore.Release();
                this.Snagged = false;
            }
        }
    }
}