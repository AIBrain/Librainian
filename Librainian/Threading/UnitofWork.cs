namespace Librainian.Threading {

    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public class UnitofWork : Task {

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is <see langword="null" />.</exception>
        public UnitofWork( [NotNull] Action action ) : base( action ) { }

        /// <summary>
        ///     Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action and
        ///     <see cref="T:System.Threading.CancellationToken" />.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="T:System.Threading.CancellationToken" /> that the new  task will
        ///     observe.
        /// </param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The provided <see cref="T:System.Threading.CancellationToken" /> has
        ///     already been disposed.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        public UnitofWork( [NotNull] Action action, CancellationToken cancellationToken ) : base( action, cancellationToken ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action and creation options.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="creationOptions">
        ///     The <see cref="T:System.Threading.Tasks.TaskCreationOptions" /> used to customize the
        ///     task's behavior.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> argument specifies an
        ///     invalid value for <see cref="T:System.Threading.Tasks.TaskCreationOptions" />.
        /// </exception>
        public UnitofWork( [NotNull] Action action, TaskCreationOptions creationOptions ) : base( action, creationOptions ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action and creation options.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that the new
        ///     task will observe.
        /// </param>
        /// <param name="creationOptions">
        ///     The <see cref="T:System.Threading.Tasks.TaskCreationOptions" /> used to customize the
        ///     task's behavior.
        /// </param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The <see cref="T:System.Threading.CancellationTokenSource" /> that
        ///     created <paramref name="cancellationToken" /> has already been disposed.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> argument specifies an
        ///     invalid value for <see cref="T:System.Threading.Tasks.TaskCreationOptions" />.
        /// </exception>
        public UnitofWork( [NotNull] Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions ) : base( action, cancellationToken, creationOptions ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action and state.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="state">An object representing data to be used by the action.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        public UnitofWork( [NotNull] Action<Object> action, Object state ) : base( action, state ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action, state, and options.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="state">An object representing data to be used by the action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that that the
        ///     new task will observe.
        /// </param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The <see cref="T:System.Threading.CancellationTokenSource" /> that
        ///     created <paramref name="cancellationToken" /> has already been disposed.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        public UnitofWork( [NotNull] Action<Object> action, Object state, CancellationToken cancellationToken ) : base( action, state, cancellationToken ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action, state, and options.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="state">An object representing data to be used by the action.</param>
        /// <param name="creationOptions">
        ///     The <see cref="T:System.Threading.Tasks.TaskCreationOptions" /> used to customize the
        ///     task's behavior.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> argument specifies an
        ///     invalid value for <see cref="T:System.Threading.Tasks.TaskCreationOptions" />.
        /// </exception>
        public UnitofWork( [NotNull] Action<Object> action, Object state, TaskCreationOptions creationOptions ) : base( action, state, creationOptions ) { }

        /// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Task" /> with the specified action, state, and options.</summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="state">An object representing data to be used by the action.</param>
        /// <param name="cancellationToken">
        ///     The <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> that that the
        ///     new task will observe..
        /// </param>
        /// <param name="creationOptions">
        ///     The <see cref="T:System.Threading.Tasks.TaskCreationOptions" /> used to customize the
        ///     task's behavior.
        /// </param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///     The <see cref="T:System.Threading.CancellationTokenSource" /> that
        ///     created <paramref name="cancellationToken" /> has already been disposed.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     The <paramref name="creationOptions" /> argument specifies an
        ///     invalid value for <see cref="T:System.Threading.Tasks.TaskCreationOptions" />.
        /// </exception>
        public UnitofWork( [NotNull] Action<Object> action, Object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions ) : base( action, state, cancellationToken, creationOptions ) { }

        [DllImport( "user32.dll", ExactSpelling = true, CharSet = CharSet.Auto )]
        [ResourceExposure( ResourceScope.Process )]
        public static extern Int32 GetWindowThreadProcessId( HandleRef hWnd, out Int32 lpdwProcessId );

        public UnitofWork Start( Action action ) {

            var task = Run( action );

            /*
            var c = Control.FromHandle(1);
            Int32 hwndThread = SafeNativeMethods.GetWindowThreadProcessId(hwnd, out var pid);
            Int32 currentThread = SafeNativeMethods.GetCurrentThreadId();
            return (hwndThread != currentThread);
            */
            return ( UnitofWork ) task;
        }

    }

}