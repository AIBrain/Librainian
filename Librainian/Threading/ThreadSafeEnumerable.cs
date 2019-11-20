// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ThreadSafeEnumerable.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ThreadSafeEnumerable.cs" was last formatted by Protiguous on 2019/11/20 at 5:54 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using JetBrains.Annotations;

    public class ThreadSafeEnumerable<T> : IEnumerable<T> {

        [NotNull]
        private IEnumerable<T> original { get; }

        public ThreadSafeEnumerable( [NotNull] IEnumerable<T> original ) => this.original = original ?? throw new ArgumentNullException( nameof( original ) );

        [NotNull]
        public IEnumerator<T> GetEnumerator() => new ThreadSafeEnumerator( this.original.GetEnumerator() );

        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private sealed class ThreadSafeEnumerator : IEnumerator<T> {

            [NotNull]
            private ThreadLocal<T> current { get; } = new ThreadLocal<T>();

            [NotNull]
            private IEnumerator<T> original { get; }

            [NotNull]
            private Object padlock { get; } = new Object();

            [NotNull]
            public T Current => this.current.Value;

            [NotNull]
            Object IEnumerator.Current => this.Current;

            internal ThreadSafeEnumerator( [NotNull] IEnumerator<T> original ) => this.original = original ?? throw new ArgumentNullException( nameof( original ) );

            public void Dispose() {
                this.original.Dispose();
                this.current.Dispose();
            }

            public Boolean MoveNext() {
                lock ( this.padlock ) {
                    var ret = this.original.MoveNext();

                    if ( ret ) {
                        this.current.Value = this.original.Current;
                    }

                    return ret;
                }
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}