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
// File "ConcurrentListFile.cs" last formatted on 2020-08-14 at 8:44 PM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using Collections.Lists;
	using FileSystem;
	using JetBrains.Annotations;
	using Logging;
	using Maths.Numbers;
	using Newtonsoft.Json;

	/// <summary>Persist a list to and from a JSON formatted text document.</summary>
	[JsonObject]
	public class ConcurrentListFile<TValue> : ConcurrentList<TValue> {

		// ReSharper disable once NotNullMemberIsNotInitialized
		private ConcurrentListFile() => throw new NotImplementedException();

		/// <summary>Persist a dictionary to and from a JSON formatted text document.</summary>
		/// <param name="document"></param>
		public ConcurrentListFile( [NotNull] Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			document.ContainingingFolder().Create();

			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );
			this.Read().Wait(); //TODO I don't like this Wait being here.
		}

		/// <summary>
		///     Persist a dictionary to and from a JSON formatted text document.
		///     <para>Defaults to user\appdata\Local\productname\filename</para>
		/// </summary>
		/// <param name="filename"></param>
		public ConcurrentListFile( [NotNull] String filename ) : this( new Document( filename ) ) { }

		/// <summary>disallow constructor without a document/filename</summary>
		/// <summary></summary>
		[JsonProperty]
		[NotNull]
		public Document Document { get; set; }

		public async Task<Boolean> Read( CancellationToken token = default ) {
			if ( this.Document.Exists() == false ) {
				return default( Boolean );
			}

			try {
				var progress = new Progress<ZeroToOne>( pro => { } );
				var result = await this.Document.LoadJSON<IEnumerable<TValue>>( progress, token );

				if ( result.status.IsGood() ) {
					var data = result.obj;
					await this.AddRangeAsync( data, token ).ConfigureAwait( false );

					return true;
				}
			}
			catch ( JsonException exception ) {
				exception.Log();
			}
			catch ( IOException exception ) {
				//file in use by another app
				exception.Log();
			}
			catch ( OutOfMemoryException exception ) {
				//file is huge
				exception.Log();
			}

			return default( Boolean );
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.Count} items";

		/// <summary>Saves the data to the <see cref="Document" />.</summary>
		/// <returns></returns>
		public Boolean Write() {
			var document = this.Document;

			if ( !document.ContainingingFolder().Exists() ) {
				document.ContainingingFolder().Create();
			}

			if ( document.Exists() ) {
				document.Delete();
			}

			document.AppendText( this.ToJSON( Formatting.Indented ) );

			return true;
		}

	}

}