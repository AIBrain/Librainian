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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Error.cs" last touched on 2021-10-15 at 5:41 AM by Protiguous.

namespace Librainian.Extensions;

using System;
using System.Diagnostics;
using Exceptions;
using JetBrains.Annotations;
using Logging;

public static class Error {

	/// <summary>
	///     Wrap an action with a try/catch.
	///     <returns>Returns true if <paramref name="action" /> had no exception.</returns>
	/// </summary>
	/// <param name="action"></param>
	/// <param name="final"> </param>
	/// <returns>Returns true if successful.</returns>
	[DebuggerStepThrough]
	public static Boolean Trap( [InstantHandle] this Action action, [InstantHandle] Action? final = default( Action? ) ) {
		try {
			action();

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

	/// <summary>Wrap an each action with a try/catch.</summary>
	/// <param name="actions"></param>
	/// <returns>Returns true if successful.</returns>
	[DebuggerStepThrough]
	public static Boolean Trap( [InstantHandle] params Action[]? actions ) {
		try {
			if ( actions is null ) {
				if ( Debugger.IsAttached ) {
					throw new NullException( $"Null list of {nameof( actions )} given. Unable to execute {nameof( actions )}." );
				}

				return false;
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

	/// <summary>Wrap a function with a try/catch.</summary>
	/// <param name="function"> </param>
	/// <param name="final"></param>
	[DebuggerStepThrough]
	public static T? Trap<T>( /*[InstantHandle]*/ this Func<T>? function, /*[InstantHandle]*/ Action? final = default( Action? ) ) {
		if ( function is null ) {
			if ( Debugger.IsAttached ) {
				throw new NullException( nameof( function ) );
			}

			return default( T? );
		}

		try {
			return function();
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

		return default( T );
	}

	/// <summary>Wrap a function with a try/catch.</summary>
	/// <param name="func">    </param>
	/// <param name="argument"></param>
	/// <param name="exception"></param>
	/// <param name="final">   </param>
	/// <param name="actions"></param>
	[DebuggerStepThrough]
	public static TR? Trap<T, TR>(
		[InstantHandle] this Func<T?, TR>? func,
		T? argument,
		out Exception? exception,
		[InstantHandle] Action? final = default( Action? ),
		params Action[]? actions
	) {
		if ( func is null ) {
			if ( Debugger.IsAttached ) {
				throw new NullException( nameof( func ) );
			}

			exception = new NullException( nameof( func ) );

			return default( TR );
		}

		try {
			exception = default( Exception );

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

		return default( TR );
	}

}