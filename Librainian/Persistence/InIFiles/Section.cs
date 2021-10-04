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
// File "Section.cs" last formatted on 2020-08-14 at 8:42 PM.

#nullable enable

namespace Librainian.Persistence.InIFiles {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;
	using Parsing;

	using DataType = System.Collections.Concurrent.ConcurrentDictionary<System.String, System.String?>;

	/// <summary>
	///     <para>
	///         This just wraps a <see cref="ConcurrentDictionary{TKey,TValue}" /> so we can index the <see cref="Data" />
	///         without throwing exceptions on missing or null keys.
	///     </para>
	///     <para>Does not throw <see cref="ArgumentEmptyException" /> on null keys passed to the indexer.</para>
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public class Section : IEquatable<Section> {

		[JsonProperty( IsReference = false, ItemIsReference = false )]
		private DataType Data { get; } = new();

		/// <summary>Automatically remove any key where there is no value. Defaults to true.</summary>
		[JsonIgnore]
		public Boolean AutoCleanup { get; set; } = true;

		[JsonIgnore]
		public IReadOnlyList<String> Keys => ( IReadOnlyList<String> )this.Data.Keys;

		[JsonIgnore]
		public IReadOnlyList<String> Values => ( IReadOnlyList<String> )this.Data.Values;

		[JsonIgnore]
		public String? this[String? key] {
			[CanBeNull]
			get {
				if ( key is null ) {
					return default( String );
				}

				return this.Data.TryGetValue( key, out var value ) ? value : null;
			}

			set {
				if ( key is null ) {
					return;
				}

				if ( value is null && this.AutoCleanup ) {
					this.Data.TryRemove( key, out var _ ); //a little cleanup
				}
				else {
					this.Data[key] = value;
				}
			}
		}

		/// <summary>Static comparison. Checks references and then keys and then values.</summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		public static Boolean Equals( Section? left, Section? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			if ( ReferenceEquals( left.Data, right.Data ) ) {
				return true;
			}

			return left.Data.OrderBy( pair => pair.Key ).SequenceEqual( right.Data.OrderBy( pair => pair.Key ) ); //will this work? //TODO what about comparing values also?
		}

		public static Boolean operator !=( Section? left, Section? right ) => !Equals( left, right );

		public static Boolean operator ==( Section? left, Section? right ) => Equals( left, right );

		/// <summary>Remove any key where there is no value.</summary>
		public Task CleanupAsync( CancellationToken cancellationToken ) =>
			Task.Run( () => {

				//TODO Unit test this.
				foreach ( var key in this.Keys ) {
					if ( cancellationToken.IsCancellationRequested ) {
						return;
					}
					if ( this.Data.TryRemove( key, out var value ) && !String.IsNullOrEmpty( value ) ) {
						this[key] = value; //whoops, re-add value. Cause: other threads.
					}
				}
			}, cancellationToken );

		public Boolean Equals( Section? other ) => Equals( this, other );

		public override Boolean Equals( Object? obj ) => Equals( this, obj as Section );

		public override Int32 GetHashCode() => this.Data.GetHashCode();

		/// <summary>Merges (adds keys and overwrites values) <see cref="Data" /> into <see cref="this" />.</summary>
		/// <param name="reader"></param>
		public async Task<Boolean> ReadAsync( TextReader reader, CancellationToken cancellationToken ) {
			if ( reader is null ) {
				throw new ArgumentEmptyException( nameof( reader ) );
			}

			try {
				var that = await reader.ReadLineAsync().ConfigureAwait( false );

				if ( that != null && JsonConvert.DeserializeObject( that, this.Data.GetType() ) is DataType other ) {
					Parallel.ForEach( other.TakeWhile( _ => !cancellationToken.IsCancellationRequested ), pair => {
						(var key, var value) = pair;
						this[key] = value;
					} );

					return true;
				}

				return false;
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return false;
		}

		public override String ToString() => $"{this.Keys.Take( 25 ).ToStrings()}";

		/// <summary>Write this <see cref="Section" /> to the <paramref name="writer" />.</summary>
		/// <param name="writer"></param>
		public Task Write( TextWriter writer ) {
			if ( writer is null ) {
				throw new ArgumentEmptyException( nameof( writer ) );
			}

			try {
				var me = JsonConvert.SerializeObject( this, Formatting.None );

				return writer.WriteLineAsync( me );
			}
			catch ( Exception exception ) {
				exception.Log();

				return Task.FromException( exception );
			}
		}
	}
}