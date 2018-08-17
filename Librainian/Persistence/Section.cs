// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Section.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Section.cs" was last formatted by Protiguous on 2018/07/13 at 1:37 AM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Collections;
	using JetBrains.Annotations;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	///     <para>
	///         This just wraps a <see cref="ConcurrentDictionary{TKey,TValue}" /> so we can index the <see cref="Data" />
	///         without throwing exceptions on missing or null keys.
	///     </para>
	///     <para>Does not throw <see cref="ArgumentNullException" /> on null keys passed to the indexer.</para>
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class Section : IEquatable<Section> {

		public Boolean Equals( [CanBeNull] Section other ) => Equals( left: this, right: other );

		[JsonProperty( IsReference = false, ItemIsReference = false )]
		private ConcurrentDictionary<String, String> Data { get; } = new ConcurrentDictionary<String, String>();

		/// <summary>
		///     Automatically remove any key where there is no value. Defaults to true.
		/// </summary>
		[JsonIgnore]
		public Boolean AutoCleanup { get; set; } = true;

		[JsonIgnore]
		[NotNull]
		public IReadOnlyList<String> Keys => ( IReadOnlyList<String> ) this.Data.Keys;

		[JsonIgnore]
		[NotNull]
		public IReadOnlyList<String> Values => ( IReadOnlyList<String> ) this.Data.Values;

		[JsonIgnore]
		[CanBeNull]
		public String this[ [CanBeNull] String key ] {
			[CanBeNull]
			get {
				if ( key is null ) { return null; }

				return this.Data.TryGetValue( key, out var value ) ? value : null;
			}

			set {
				if ( key is null ) { return; }

				if ( value is null && this.AutoCleanup ) {
					this.Data.TryRemove( key, out _ ); //a little cleanup
				}
				else { this.Data[ key ] = value; }
			}
		}

		/// <summary>
		///     Static comparison. Checks references and then keys and then values.
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] Section left, [CanBeNull] Section right ) {
			if ( ReferenceEquals( left, right ) ) { return true; }

			if ( left is null || right is null ) { return false; }

			if ( ReferenceEquals( left.Data, right.Data ) ) { return true; }

			return left.Data.OrderBy( pair => pair.Key ).ThenBy( pair => pair.Value ).SequenceEqual( right.Data.OrderBy( pair => pair.Key ).ThenBy( pair => pair.Value ) );
		}

		public static Boolean operator !=( [CanBeNull] Section left, [CanBeNull] Section right ) => !Equals( left: left, right: right );

		public static Boolean operator ==( [CanBeNull] Section left, [CanBeNull] Section right ) => Equals( left: left, right: right );

		/// <summary>
		///     Remove any key where there is no value.
		/// </summary>
		/// <returns></returns>
		public async Task Cleanup() =>
			await Task.Run( () => {
				foreach ( var key in this.Keys.Where( String.IsNullOrEmpty ) ) {
					if ( this.Data.TryRemove( key, out var value ) && !String.IsNullOrEmpty( value ) ) {
						this[ key ] = value; //whoops, re-add value. Cause: other threads.
					}
				}
			} ).NoUI();

		public override Boolean Equals( [CanBeNull] Object obj ) => Equals( left: this, right: obj as Section );

		public override Int32 GetHashCode() => this.Data.GetHashCode();

		/// <summary>
		///     Merges (adds keys and overwrites values) <see cref="Data" /> into <see cref="this" />.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public async Task<Boolean> Read( [NotNull] TextReader reader ) {
			if ( reader is null ) { throw new ArgumentNullException( paramName: nameof( reader ) ); }

			try {
				var that = await reader.ReadLineAsync().NoUI();

				return await Task.Run( () => {
					if ( JsonConvert.DeserializeObject( that, this.Data.GetType() ) is ConcurrentDictionary<String, String> other ) {
						Parallel.ForEach( other, pair => this[ pair.Key ] = pair.Value );

						return true;
					}

					return false;
				} ).NoUI();
			}
			catch ( Exception exception ) { exception.Log(); }

			return false;
		}

		public override String ToString() => $"{this.Keys.Take( 25 ).ToStrings()}";

		/// <summary>
		///     Write this <see cref="Section" /> to the <paramref name="writer" />.
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		public async Task<Boolean> Write( [NotNull] TextWriter writer ) {
			if ( writer is null ) { throw new ArgumentNullException( paramName: nameof( writer ) ); }

			try {
				var me = JsonConvert.SerializeObject( this, Formatting.None );
				await writer.WriteLineAsync( me ).NoUI();

				return true;
			}
			catch ( Exception exception ) { exception.Log(); }

			return false;
		}
	}
}