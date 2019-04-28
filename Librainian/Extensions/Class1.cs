// Copyright 2018, RS&I Inc.
// Authors: Rick Harker, Scott Rydalch, and Dave Reese.
//
// Error.cs last touched on 2019-02-22 at 8:06 AM

// ReSharper disable once CheckNamespace
namespace System {

    using Diagnostics;
    using JetBrains.Annotations;
    using Librainian.Logging;

    public static class Error {

        /// <summary>
        /// Wrap an action with a try/catch.
        /// <returns>Returns true if <paramref name="action"/> had no exception.</returns>
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

            return false;
        }

        /// <summary>
        /// Wrap an each action with a try/catch.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns>Returns true if successful.</returns>
        [DebuggerStepThrough]
        public static Boolean Trap( [InstantHandle] [CanBeNull] params Action[] actions ) {
            try {
                if ( actions == null ) {
                    throw new ArgumentNullException( $"Null list of {nameof( actions )} given. Unable to execute {nameof( actions )}." );
                }

                foreach ( var action in actions ) {
                    action.Trap();
                }

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            return false;
        }

        /// <summary>
        /// Wrap a function with a try/catch.
        /// </summary>
        /// <param name="func"> </param>
        /// <param name="final"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        public static T Trap<T>( [InstantHandle] [CanBeNull] this Func<T> func, [InstantHandle] [CanBeNull] Action final = default ) {
            try {
                return func == default ? default : func();
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

        /// <summary>
        /// Wrap a function with a try/catch.
        /// </summary>
        /// <param name="func">    </param>
        /// <param name="argument"></param>
        /// <param name="final">   </param>
        /// <param name="actions"></param>
        /// <returns></returns>
        [CanBeNull]
        [DebuggerStepThrough]
        public static R Trap<T, R>(
            [InstantHandle] [CanBeNull] this Func<T, R> func,
            [CanBeNull] T argument,
            [InstantHandle] [CanBeNull] Action final = default, [CanBeNull] params Action[] actions ) {
            try {
                return func == default ? default : func( argument );
            }
            catch ( Exception exception ) {
                exception.Log();
            }
            finally {
                try {
                    if ( actions != null ) {
                        Trap( actions );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();
                }

                try {
                    final?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }

            return default;
        }
    }
}