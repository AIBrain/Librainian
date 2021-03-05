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
// File "MultiKeyDictionary.cs" last formatted on 2020-08-14 at 8:31 PM.

#nullable enable

namespace Librainian.Collections {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;

	/// <summary>Multi-Key Dictionary Class</summary>
	/// <typeparam name="TK">Primary Key Type</typeparam>
	/// <typeparam name="TL">Sub Key Type</typeparam>
	/// <typeparam name="TV">Value Type</typeparam>
	/// <remarks>A relational database would be faster for large datasets..</remarks>
	public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> 
		where TK : notnull
		 {

		internal ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping { get; } = new();

		internal ConcurrentDictionary<TL, TK> SubDictionary { get; } = new();

		[CanBeNull]
		public TV this[ [NotNull] TL subKey ] {
			get {
				if ( this.TryGetValue( subKey, out var item ) ) {
					return item;
				}

				return default( TV );
				//throw new KeyNotFoundException( $"sub key not found: {subKey}" ); //TODO I hate throwing exceptions in indexers..
			}
		}

		[CanBeNull]
		public new TV this[ [NotNull] TK primaryKey ] {
			get {
				if ( this.TryGetValue( primaryKey, out var item ) ) {
					return item;
				}

				return default( TV );
				//throw new KeyNotFoundException( $"primary key not found: {primaryKey}" );
			}
		}

		public void Add( [NotNull] TK primaryKey, [CanBeNull] TV val ) => this.TryAdd( primaryKey, val );

		public void Add( [NotNull] TK primaryKey, [NotNull] TL subKey, [CanBeNull] TV val ) {
			this.TryAdd( primaryKey, val );

			this.Associate( subKey, primaryKey );
		}

		public void Associate( [NotNull] TL subKey, [NotNull] TK primaryKey ) {
			if ( !base.ContainsKey( primaryKey ) ) {
				throw new KeyNotFoundException( $"The primary dictionary does not contain the key '{primaryKey}'" );
			}

			if ( this.SubDictionary.ContainsKey( subKey ) ) {
				this.SubDictionary[subKey] = primaryKey;
				this.PrimaryToSubkeyMapping[primaryKey] = subKey;
			}
			else {
				this.SubDictionary.TryAdd( subKey, primaryKey );
				this.PrimaryToSubkeyMapping.TryAdd( primaryKey, subKey );
			}
		}

		[NotNull]
		public TK[] ClonePrimaryKeys() => this.Keys.ToArray();

		[NotNull]
		public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

		[NotNull]
		public TV[] CloneValues() => this.Values.ToArray();

		public Boolean ContainsKey( [NotNull] TL subKey ) => this.TryGetValue( subKey, out _ );

		public new Boolean ContainsKey( [NotNull] TK primaryKey ) => this.TryGetValue( primaryKey, out _ );

		public void Remove( [NotNull] TK primaryKey ) {
			if ( Equals( primaryKey, default( Object? ) ) ) {
				throw new InvalidOperationException( nameof( primaryKey ) );
			}

			this.SubDictionary.TryRemove( this.PrimaryToSubkeyMapping[primaryKey], out _ );

			this.PrimaryToSubkeyMapping.TryRemove( primaryKey, out _ );

			this.TryRemove( primaryKey, out _ );
		}

		public void Remove( [NotNull] TL subKey ) {
			this.TryRemove( this.SubDictionary[subKey], out _ );
			this.PrimaryToSubkeyMapping.TryRemove( this.SubDictionary[subKey], out _ );
			this.SubDictionary.TryRemove( subKey, out _ );
		}

		public Boolean TryGetValue( [NotNull] TL subKey, [CanBeNull] out TV val ) {
			val = default( TV );

			return this.SubDictionary.TryGetValue( subKey, out var ep ) && this.TryGetValue( ep, out val );
		}

		public new Boolean TryGetValue( [NotNull] TK primaryKey, [CanBeNull] out TV val ) => base.TryGetValue( primaryKey, out val );

	}

}