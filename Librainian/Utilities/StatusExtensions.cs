// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "StatusExtensions.cs" last formatted on 2020-08-28 at 1:38 AM.

// ReSharper disable once CheckNamespace
namespace Librainian;

using System;
using System.Runtime.CompilerServices;
using Extensions;
using Parsing;
using Utilities;

public static class StatusExtensions {

	static StatusExtensions() {
		if ( Status.Good.IsBad() ) {
			throw new InvalidOperationException( "Someone blinked." );
		}

		if ( Status.Failure.IsGood() ) {
			throw new InvalidOperationException( "Someone blinked." );
		}

		if ( Status.Success.IsBad() ) {
			throw new InvalidOperationException( "Someone blinked." );
		}

		if ( !Status.Unknown.IsUnknown() ) {
			throw new InvalidOperationException( "Someone blinked." );
		}
	}

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean Failed( this Status status ) => status <= Status.Failure;

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsBad( this Status status ) => status.Failed();

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsGood( this Status status ) => status.Succeeded();

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean IsUnknown( this Status status ) => status == Status.Unknown || !status.IsBad() && !status.IsGood();

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Boolean Succeeded( this Status status ) => status >= Status.Success;

	[NeedsTesting]
	public static String Symbol( this Status status ) => status.GetDescription() ?? Symbols.Null;

	[NeedsTesting]
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static Status ToStatus( this Boolean status ) => status ? Status.Success : Status.Failure;
}