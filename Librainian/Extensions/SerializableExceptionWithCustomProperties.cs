// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

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

		/// <summary>Pulled from</summary>
		/// <param name="info">   </param>
		/// <param name="context"></param>
		[SecurityPermission( SecurityAction.Demand, SerializationFormatter = true )]

		// Constructor should be protected for unsealed classes, private for sealed classes. (The Serializer invokes this constructor through reflection, so it can be private)
		protected SerializableExceptionWithCustomProperties( [NotNull] SerializationInfo info, StreamingContext context ) : base( info, context ) {
			this.ResourceName = info.GetString( "ResourceName" );
			this.ValidationErrors = ( IList<String> )info.GetValue( "ValidationErrors", typeof( IList<String> ) );
		}

		public SerializableExceptionWithCustomProperties() { }

		public SerializableExceptionWithCustomProperties( [CanBeNull] String message ) : base( message ) { }

		public SerializableExceptionWithCustomProperties( [CanBeNull] String message, [CanBeNull] Exception innerException ) : base( message,
			innerException ) { }

		public SerializableExceptionWithCustomProperties( [CanBeNull] String message, [CanBeNull] String resourceName, [CanBeNull] IList<String> validationErrors ) :
			base( message ) {
			this.ResourceName = resourceName;
			this.ValidationErrors = validationErrors;
		}

		public SerializableExceptionWithCustomProperties( [CanBeNull] String message, [CanBeNull] String resourceName, [CanBeNull] IList<String> validationErrors,
			[CanBeNull] Exception innerException ) : base( message, innerException ) {
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