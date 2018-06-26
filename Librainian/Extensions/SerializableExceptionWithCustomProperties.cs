// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "SerializableExceptionWithCustomProperties.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "SerializableExceptionWithCustomProperties.cs" was last formatted by Protiguous on 2018/06/26 at 1:04 AM.

namespace Librainian.Extensions {

	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Security.Permissions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	// Important: This attribute is NOT inherited from Exception, and MUST be specified otherwise serialization will fail with a SerializationException stating that "Type X in Assembly Y is not marked as serializable."
	[JsonObject]
	[Serializable]
	public class SerializableExceptionWithCustomProperties : Exception {

		public String ResourceName { get; }

		public IList<String> ValidationErrors { get; }

		/// <summary>
		///     Pulled from
		/// </summary>
		/// <param name="info">   </param>
		/// <param name="context"></param>
		[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

		// Constructor should be protected for unsealed classes, private for sealed classes. (The Serializer invokes this constructor through reflection, so it can be private)
		protected SerializableExceptionWithCustomProperties( [NotNull] SerializationInfo info, StreamingContext context ) : base( info, context ) {
			this.ResourceName = info.GetString( "ResourceName" );
			this.ValidationErrors = ( IList<String> ) info.GetValue( "ValidationErrors", typeof( IList<String> ) );
		}

		public SerializableExceptionWithCustomProperties() { }

		public SerializableExceptionWithCustomProperties( String message ) : base( message ) { }

		public SerializableExceptionWithCustomProperties( String message, Exception innerException ) : base( message, innerException ) { }

		public SerializableExceptionWithCustomProperties( String message, String resourceName, IList<String> validationErrors ) : base( message ) {
			this.ResourceName = resourceName;
			this.ValidationErrors = validationErrors;
		}

		public SerializableExceptionWithCustomProperties( String message, String resourceName, IList<String> validationErrors, Exception innerException ) : base( message, innerException ) {
			this.ResourceName = resourceName;
			this.ValidationErrors = validationErrors;
		}

		[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) {
			if ( info is null ) {
				throw new ArgumentNullException( nameof( info ) );
			}

			info.AddValue( "ResourceName", this.ResourceName );

			// Note: if "List<T>" isn't serializable you may need to work out another method of adding your list, this is just for show...
			info.AddValue( "ValidationErrors", this.ValidationErrors, typeof( IList<String> ) );

			// MUST call through to the base class to let it save its own state
			base.GetObjectData( info, context );
		}
	}
}