// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code
//  (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
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
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Warning.cs" last formatted on 2021-02-26 at 6:01 AM.

#nullable enable

namespace Librainian.Exceptions.Warnings {

	using System;
	using System.Diagnostics;
	using System.Runtime.Serialization;
	using Logging;

	public class UnknownWarning : Warning {

		public UnknownWarning( String? message ) : base( message ) {
			message.Break();
		}
	}

	/// <inheritdoc />
	/// <summary>
	///     <para>Generic Warning</para>
	///     <para><see cref="Debugger.Break" /> if a <see cref="Debugger" /> is attached.</para>
	///     <para>This should be handled, but allow program to continue.</para>
	/// </summary>
	[Serializable]
	public abstract class Warning : Exception {

		protected Warning( SerializationInfo serializationInfo, StreamingContext streamingContext ) : base( serializationInfo, streamingContext ) { }

		protected Warning() {

			//String.Empty.Break();
		}

		protected Warning( String? message ) : base( message ) {

			//message.Break();
		}

		protected Warning( String? message, Exception? inner ) : base( message, inner ) {

			//message.Break();
		}
	}
}