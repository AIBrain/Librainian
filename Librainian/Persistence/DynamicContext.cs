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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DynamicContext.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Persistence;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

/// <summary>
/// </summary>
/// <see cref="http://stackoverflow.com/a/4857322/956364" />
[JsonObject]
[Serializable]
public class DynamicContext : DynamicObject, ISerializable {

	protected DynamicContext( SerializationInfo info, StreamingContext context ) {

		// TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/Library/ty01x675(VS.80).aspx
		foreach ( var entry in info ) {
			if ( entry.Value != null ) {
				this.Context.Add( entry.Name, entry.Value );
			}
		}
	}

	public DynamicContext() {
	}

	private Dictionary<String, Object> Context { get; } = new();

	public virtual void GetObjectData( SerializationInfo? info, StreamingContext context ) {
		foreach ( var kvp in this.Context ) {
			info?.AddValue( kvp.Key, kvp.Value );
		}
	}

	public override Boolean TryGetMember( GetMemberBinder binder, out Object? result ) => this.Context.TryGetValue( binder.Name, out result );

	public override Boolean TrySetMember( SetMemberBinder binder, Object? value ) {
		if ( value != null ) {
			this.Context.Add( binder.Name, value );
		}

		return true;
	}
}