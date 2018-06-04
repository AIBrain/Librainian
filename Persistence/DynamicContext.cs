// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DynamicContext.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "DynamicContext.cs" was last formatted by Protiguous on 2018/06/04 at 4:21 PM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Runtime.Serialization;
	using System.Security.Permissions;
	using Newtonsoft.Json;

	/// <summary></summary>
	/// <seealso cref="http://stackoverflow.com/a/4857322/956364" />
	[JsonObject]
	[Serializable]
	public class DynamicContext : DynamicObject, ISerializable {

		[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
			foreach ( var kvp in this.Context ) { info.AddValue( kvp.Key, kvp.Value ); }
		}

		private Dictionary<String, Object> Context { get; } = new Dictionary<String, Object>();

		public override Boolean TryGetMember( GetMemberBinder binder, out Object result ) => this.Context.TryGetValue( binder.Name, out result );

		public override Boolean TrySetMember( SetMemberBinder binder, Object value ) {
			this.Context.Add( binder.Name, value );

			return true;
		}

		protected DynamicContext( SerializationInfo info, StreamingContext context ) {

			// TODO: validate inputs before deserializing. See http://msdn.microsoft.com/en-us/Library/ty01x675(VS.80).aspx
			foreach ( var entry in info ) { this.Context.Add( entry.Name, entry.Value ); }
		}

		public DynamicContext() { }

	}

}