// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "UnitofWork.cs" last touched on 2021-03-07 at 1:44 PM by Protiguous.

#nullable enable

namespace Librainian.Threading {

	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.Versioning;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary>Yah.. dunno where I was heading with this class..?</summary>
	public class UnitofWork : Task {

		/// <summary>Initializes a new <see cref="Task" /> with the specified action.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is <see langword="null" />.</exception>
		public UnitofWork( [NotNull] Action action ) : base( action ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action and <see cref="CancellationToken" />.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken" /> that the new  task will observe.</param>
		/// <exception cref="ObjectDisposedException">The provided <see cref="CancellationToken" /> has already been disposed.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public UnitofWork( [NotNull] Action action, CancellationToken cancellationToken ) : base( action, cancellationToken ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action and creation options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     The <paramref name="creationOptions" /> argument specifies an invalid
		///     value for <see cref="TaskCreationOptions" />.
		/// </exception>
		public UnitofWork( [NotNull] Action action, TaskCreationOptions creationOptions ) : base( action, creationOptions ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action and creation options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <param name="cancellationToken">The <see cref="TaskFactory.CancellationToken" /> that the new task will observe.</param>
		/// <exception cref="ObjectDisposedException">
		///     The <see cref="CancellationTokenSource" /> that created
		///     <paramref name="cancellationToken" /> has already been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     The <paramref name="creationOptions" /> argument specifies an invalid
		///     value for <see cref="TaskCreationOptions" />.
		/// </exception>
		public UnitofWork( [NotNull] Action action, TaskCreationOptions creationOptions, CancellationToken cancellationToken ) : base( action, cancellationToken,
			creationOptions ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action and state.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public UnitofWork( [CanBeNull] Action<Object?> action, [CanBeNull] Object? state ) : base( action, state ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="cancellationToken">The <see cref="TaskFactory.CancellationToken" /> that that the new task will observe.</param>
		/// <exception cref="ObjectDisposedException">
		///     The <see cref="CancellationTokenSource" /> that created
		///     <paramref name="cancellationToken" /> has already been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		public UnitofWork( [NotNull] Action<Object?> action, [CanBeNull] Object? state, CancellationToken cancellationToken ) : base( action, state, cancellationToken ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     The <paramref name="creationOptions" /> argument specifies an invalid
		///     value for <see cref="TaskCreationOptions" />.
		/// </exception>
		public UnitofWork( [NotNull] Action<Object?> action, [CanBeNull] Object? state, TaskCreationOptions creationOptions ) : base( action, state, creationOptions ) { }

		/// <summary>Initializes a new <see cref="Task" /> with the specified action, state, and options.</summary>
		/// <param name="action">The delegate that represents the code to execute in the task.</param>
		/// <param name="state">An object representing data to be used by the action.</param>
		/// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken" /> that that the new task will observe..</param>
		/// <exception cref="ObjectDisposedException">
		///     The <see cref="CancellationTokenSource" /> that created
		///     <paramref name="cancellationToken" /> has already been disposed.
		/// </exception>
		/// <exception cref="ArgumentNullException">The <paramref name="action" /> argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///     The <paramref name="creationOptions" /> argument specifies an invalid
		///     value for <see cref="TaskCreationOptions" />.
		/// </exception>
		public UnitofWork( [NotNull] Action<Object?> action, [CanBeNull] Object? state, TaskCreationOptions creationOptions, CancellationToken cancellationToken ) : base(
			action, state, cancellationToken, creationOptions ) { }

		[DllImport( "user32.dll", ExactSpelling = true, CharSet = CharSet.Auto )]
		[ResourceExposure( ResourceScope.Process )]
		public static extern Int32 GetWindowThreadProcessId( HandleRef hWnd, out Int32 lpdwProcessId );

		[NotNull]
		public UnitofWork Start( [NotNull] Action action ) {
			var task = Run( action );

			/*
			var c = Control.FromHandle(1);
			Int32 hwndThread = SafeNativeMethods.GetWindowThreadProcessId(hwnd, out var pid);
			Int32 currentThread = SafeNativeMethods.GetCurrentThreadId();
			return (hwndThread != currentThread);
			*/
			return ( UnitofWork )task;
		}
	}
}