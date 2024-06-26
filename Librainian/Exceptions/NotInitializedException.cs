﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "NotInitializedException.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

namespace Librainian.Exceptions;

using System;
using System.Runtime.Serialization;
using Logging;

/// <summary>
/// Throw when the object has not been initialized.
/// <para><see cref="Logging.Log{T}" /> gets called.</para>
/// </summary>
[Serializable]
public class NotInitializedException : Exception {

	/// <summary>Initializes a new instance of the <see cref="NotInitializedException" /> class with serialized data.</summary>
	/// <param name="info">
	/// The <see cref="SerializationInfo" /> instance that holds the serialized object data about the exception being thrown.
	/// </param>
	/// <param name="context">
	/// The <see cref="StreamingContext" /> instance that contains contextual information about the source or destination.
	/// </param>
	/// <remarks>This constructor overload is provided in order to adhere to custom exception design best practice guidelines.</remarks>
	protected NotInitializedException( SerializationInfo info, StreamingContext context ) : base( info, context ) => this.Parameter = "ParamFromSerialization";

	public NotInitializedException( String paramName ) {
		this.Parameter = paramName;
		this.Log( BreakOrDontBreak.Break );
	}

	public String Parameter { get; }
}