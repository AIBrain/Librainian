// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FileSingleton.cs" was last cleaned by Rick on 2016/08/06 at 10:30 PM

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
    ///     Uses a named semaphore to allow only ONE of name.
    /// </summary>
    /// <example>using ( new FileSingleton( anyName ) ) { DoCode(); }</example>
    public class FileSingleton : ABetterClassDispose {

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
            catch ( ObjectDisposedException exception ) {
                exception.More();
            }
            catch ( AbandonedMutexException exception ) {
                exception.More();
            }
            catch ( InvalidOperationException exception ) {
                exception.More();
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.More();
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( IOException exception ) {
                exception.More();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.More();
            }
            catch ( WaitHandleCannotBeOpenedException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
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
            catch ( ObjectDisposedException exception ) {
                exception.More();
            }
            catch ( AbandonedMutexException exception ) {
                exception.More();
            }
            catch ( InvalidOperationException exception ) {
                exception.More();
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.More();
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( IOException exception ) {
                exception.More();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.More();
            }
            catch ( WaitHandleCannotBeOpenedException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
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
            catch ( ObjectDisposedException exception ) {
                exception.More();
            }
            catch ( AbandonedMutexException exception ) {
                exception.More();
            }
            catch ( InvalidOperationException exception ) {
                exception.More();
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.More();
            }
            catch ( ArgumentException exception ) {
                exception.More();
            }
            catch ( IOException exception ) {
                exception.More();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.More();
            }
            catch ( WaitHandleCannotBeOpenedException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        private FileSingleton() {
        }

        public Boolean Snagged {
            get; private set;
        }

        [CanBeNull]
        private Semaphore Semaphore {
            get;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
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
