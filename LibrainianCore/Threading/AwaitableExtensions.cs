// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "AwaitableExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "AwaitableExtensions.cs" was last formatted by Protiguous on 2019/11/20 at 6:45 AM.

// ReSharper disable once CheckNamespace

namespace ExtremeConfigAwait {

    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    namespace ExtremeConfigAwait {

        using System;
        using System.Runtime.CompilerServices;
        using System.Threading;
        using JetBrains.Annotations;

        /// <summary>From https://github.com/negativeeddy/blog-examples/blob/master/ConfigureAwaitBehavior/ExtremeConfigAwaitLibrary/SynchronizationContextRemover.cs</summary>
        public struct SynchronizationContextRemover : INotifyCompletion {

            public Boolean IsCompleted => SynchronizationContext.Current is null;

            public SynchronizationContextRemover GetAwaiter() => this;

            public void GetResult() { }

            public void OnCompleted( [CanBeNull] Action continuation ) {
                var prevContext = SynchronizationContext.Current;

                try {
                    SynchronizationContext.SetSynchronizationContext( null );
                    continuation?.Invoke();
                }
                finally {
                    SynchronizationContext.SetSynchronizationContext( prevContext );
                }
            }

        }

    }

    /// <summary>From https://github.com/negativeeddy/blog-examples/blob/master/ConfigureAwaitBehavior/ExtremeConfigAwaitLibrary/AwaitableExtensions.cs</summary>
    public static class AwaitableExtensions {

        private static void PrintContext( [CallerMemberName] [CanBeNull] String callerName = null, [CallerLineNumber] Int32 line = 0 ) {
            var ctx = SynchronizationContext.Current;

            if ( ctx != null ) {
                Console.WriteLine( "{0}:{1:D4} await context will be {2}:", callerName, line, ctx );
                Console.WriteLine( "    TSCHED:{0}", TaskScheduler.Current );
            }
            else {
                Console.WriteLine( "{0}:{1:D4} await context will be <NO CONTEXT>", callerName, line );
                Console.WriteLine( "    TSCHED:{0}", TaskScheduler.Current );
            }
        }

        public static ConfiguredTaskAwaitable PrintContext( this ConfiguredTaskAwaitable t, [CallerMemberName] [CanBeNull] String callerName = null,
            [CallerLineNumber] Int32 line = 0 ) {
            PrintContext( callerName, line );

            return t;
        }

        [CanBeNull]
        public static Task PrintContext( [CanBeNull] this Task t, [CallerMemberName] [CanBeNull] String callerName = null, [CallerLineNumber] Int32 line = 0 ) {
            PrintContext( callerName, line );

            return t;
        }

    }

}