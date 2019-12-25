// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "BackgroundThread.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "BackgroundThread.cs" was last formatted by Protiguous on 2019/12/04 at 12:00 AM.

namespace LibrainianCore.Threading {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Measurement.Time;
    using Utilities;

    /// <summary>Accepts one <see cref="Action" /> to perform (a loop) when the <see cref="Signal" /> is <see cref="Poke" />.
    /// <para>Starts 1 thread and waits for the signal.</para>
    /// </summary>
    public class BackgroundThread : ABetterClassDispose {

        [NotNull]
        private Thread thread { get; }

        [NotNull]
        public CancellationTokenSource CTS { get; }

        [NotNull]
        public Action Loop { get; }

        /// <summary>True if the loop is currently running.</summary>
        public Boolean RunningLoop { get; private set; }

        [NotNull]
        public AutoResetEvent Signal { get; } = new AutoResetEvent( true );

        private CancellationToken Token => this.CTS.Token;

        /// <summary></summary>
        /// <param name="loop">Action to perform on each <see cref="Signal" />.</param>
        /// <param name="autoStart"></param>
        /// <param name="cancellationTokenSource"></param>
        public BackgroundThread( [NotNull] Action loop, Boolean autoStart, [NotNull] out CancellationTokenSource cancellationTokenSource ) {
            this.Loop = loop ?? throw new ArgumentNullException( paramName: nameof( loop ) );
            this.CTS = cancellationTokenSource = new CancellationTokenSource();

            this.thread = new Thread( () => {
                while ( !this.Token.IsCancellationRequested ) {
                    if ( this.Signal.WaitOne( Seconds.Five ) ) {
                        try {
                            this.RunningLoop = true;
                            this.Loop();
                        }
                        finally {
                            this.RunningLoop = false;
                        }
                    }
                }
            } ) {
                IsBackground = true
            };

            if ( autoStart ) {
                this.Start();
            }
        }

        private void Start() => this.thread.Start();

        /// <summary>Marks to exit the loop.</summary>
        public void Cancel() => this.CTS.Cancel();

        /// <summary>Calling this or <see cref="IDisposable.Dispose" /> will cancel the signal-waiting loop.</summary>
        public override void DisposeManaged() => this.Cancel();

        /// <summary>Do anything needed in this method before the loops run.</summary>
        public virtual void Initialize() { }

        /// <summary>Set the signal.</summary>
        public void Poke() => this.Signal.Set();

        /// <summary>Calls <see cref="Cancel" />.</summary>
        public void Quit() => this.Cancel();
    }
}