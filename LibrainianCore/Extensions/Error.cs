// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Error.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by
// formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Error.cs" was last formatted by Protiguous on 2020/02/01 at 10:16 AM.

// ReSharper disable once CheckNamespace

namespace System {

    using Diagnostics;
    using JetBrains.Annotations;
    using LibrainianCore.Logging;

    public static class Error {

        /// <summary>Wrap an action with a try/catch.
        /// <returns>Returns true if <paramref name="action" /> had no exception.</returns>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="final"> </param>
        /// <returns>Returns true if successful.</returns>
        [DebuggerStepThrough]
        public static Boolean Trap( [InstantHandle] [CanBeNull] this Action action, [InstantHandle] [CanBeNull] Action final = default ) {
            try {
                action?.Invoke();

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                try {
                    final?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }

            return default;
        }

        /// <summary>Wrap an each action with a try/catch.</summary>
        /// <param name="actions"></param>
        /// <returns>Returns true if successful.</returns>
        [DebuggerStepThrough]
        public static Boolean Trap( [InstantHandle] [CanBeNull] params Action[] actions ) {
            try {
                if ( actions is null ) {

                    if ( Debugger.IsAttached ) {
                        throw new ArgumentNullException( $"Null list of {nameof( actions )} given. Unable to execute {nameof( actions )}." );
                    }

                    return default;
                }

                foreach ( var action in actions ) {
                    action.Trap();
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return default;
        }

        /// <summary>Wrap a function with a try/catch.</summary>
        /// <param name="func"> </param>
        /// <param name="final"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        public static T Trap<T>( [InstantHandle] [CanBeNull] this Func<T> func, [InstantHandle] [CanBeNull] Action final = default ) {
            if ( func is null ) {
                if ( Debugger.IsAttached ) {
                    throw new ArgumentNullException( nameof( func ) );
                }

                return default;
            }

            try {
                return func();
            }
            catch ( Exception e ) {
                e.Log();
            }
            finally {
                try {
                    final?.Invoke();
                }
                catch ( Exception e ) {
                    e.Log();
                }
            }

            return default;
        }

        /// <summary>Wrap a function with a try/catch.</summary>
        /// <param name="func">    </param>
        /// <param name="argument"></param>
        /// <param name="exception"></param>
        /// <param name="final">   </param>
        /// <param name="actions"></param>
        /// <returns></returns>
        [CanBeNull]
        [DebuggerStepThrough]
        public static R Trap<T, R>( [InstantHandle] [CanBeNull] this Func<T, R> func, [CanBeNull] T argument, [CanBeNull] out Exception exception,
            [InstantHandle] [CanBeNull] Action final = default, [CanBeNull] params Action[] actions ) {
            if ( func is null ) {
                if ( Debugger.IsAttached ) {
                    throw new ArgumentNullException( nameof( func ) );
                }

                exception = new ArgumentNullException( nameof( func ) );

                return default;
            }

            try {
                exception = default;

                return func( argument );
            }
            catch ( Exception e ) {
                exception = e.Log();
            }
            finally {
                try {
                    if ( actions != null ) {
                        Trap( actions );
                    }
                }
                catch ( Exception e ) {
                    exception = e.Log();
                }

                try {
                    final?.Invoke();
                }
                catch ( Exception e ) {
                    exception = e.Log();
                }
            }

            return default;
        }

    }

}