// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Ini.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Ini.cs" was last formatted by Protiguous on 2018/07/13 at 1:36 AM.

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ComputerSystem.FileSystem;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	///     Persist <see cref="Section" /> to and from a JSON formatted text <see cref="Document" />.
	/// </summary>
	[JsonObject]
	public class Ini : IEquatable<Ini> {

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		///     <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise,
		///     <see langword="false" />.
		/// </returns>
		public Boolean Equals( Ini other ) => Equals( this, other );

		/// <summary>
		///     [Header] <see cref="Environment.NewLine" /> Key=Value <see cref="Environment.NewLine" />
		/// </summary>
		[JsonProperty]
		[NotNull]
		private ConcurrentDictionary<String, Section> Data { get; } = new ConcurrentDictionary<String, Section>();

		//private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, ReferenceLoopHandling = ReferenceLoopHandling.Serialize };
		/// <summary>
		/// </summary>
		[JsonProperty]
		[CanBeNull]
		public Document Document { get; set; }

		[JsonProperty]
		public Guid ID { get; }

		[CanBeNull]
		public IReadOnlyList<Section> Sections => this.Data.Values as IReadOnlyList<Section>;

		/// <summary>
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key">    </param>
		/// <returns></returns>
		[CanBeNull]
		public String this[ [CanBeNull] String section, String key ] {
			[DebuggerStepThrough]
			[CanBeNull]
			get {
				if ( section is null ) { return null; }

				if ( key is null ) { return null; }

				return this.Data[ section ]?[ key ];
			}

			[DebuggerStepThrough]
			set {
				if ( section is null ) { return; }

				if ( key is null ) { return; }

				if ( this.Data.TryGetValue( section, out var result ) ) { result[ key ] = value; }
				else {

					//is this threadsafe? another thread could pop in between the TryGetValue() above, and the Add() here.
					this.Add( section, key, value );
				}
			}
		}

		public Ini( Guid id ) => this.ID = id;

		public Ini() => this.ID = Guid.NewGuid();

		/// <summary>
		///     Persist a document to and from a JSON formatted text document.
		/// </summary>
		/// <param name="document">          </param>
		/// <param name="cancellationSource"></param>
		public Ini( [NotNull] Document document, CancellationTokenSource cancellationSource = null ) {
			this.Document = document ?? throw new ArgumentNullException( nameof( document ) );

			if ( !this.Document.Folder.Exists() ) { this.Document.Folder.Create(); }

			if ( cancellationSource is null ) { cancellationSource = new CancellationTokenSource( delay: Seconds.Ten ); }

			this.Reload().Wait( cancellationToken: cancellationSource.Token );
		}

		/// <summary>
		///     Add the <paramref name="value" /> to the <see cref="Section" /> under the key
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key">    </param>
		/// <param name="value">  </param>
		/// <returns></returns>
		private Boolean Add( [NotNull] String section, [NotNull] String key, String value ) {
			if ( section is null ) { throw new ArgumentNullException( nameof( section ) ); }

			if ( key is null ) { throw new ArgumentNullException( nameof( key ) ); }

			if ( this.Data.TryGetValue( section, out var result ) ) { result[ key ] = value; }
			else {
				this.Data[ section ] = new Section {
					[ key ] = value
				};
			}

			return true;
		}

		/// <summary>
		///     static comparison.
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( Ini left, Ini right ) {
			if ( ReferenceEquals( left, right ) ) { return true; }

			if ( ReferenceEquals( left, default ) || ReferenceEquals( default, right ) ) { return false; }

			return left.ID.Equals( right.ID );
		}

		/// <summary>
		///     Returns a value that indicates whether two <see cref="T:Librainian.Persistence.Ini" /> objects have different
		///     values.
		/// </summary>
		/// <param name="left"> The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
		public static Boolean operator !=( [CanBeNull] Ini left, [CanBeNull] Ini right ) => !Equals( left, right );

		/// <summary>
		///     Returns a value that indicates whether the values of two <see cref="T:Librainian.Persistence.Ini" /> objects are
		///     equal.
		/// </summary>
		/// <param name="left"> The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		/// <returns>
		///     true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise,
		///     false.
		/// </returns>
		public static Boolean operator ==( [CanBeNull] Ini left, [CanBeNull] Ini right ) => Equals( left, right );

		/// <summary>
		///     <para>Add in all of the sections, and key-value-pairs from the <see cref="IniFile" />.</para>
		///     <para>Performs a file save.</para>
		/// </summary>
		/// <param name="iniFile"></param>
		/// <returns></returns>
		public Boolean Add( [CanBeNull] IniFile iniFile ) {
			if ( null == iniFile ) { return false; }

			Parallel.ForEach( source: iniFile.Sections.AsParallel(), body: section => {
				var dictionary = iniFile[ section: section ];

				if ( dictionary != null ) { Parallel.ForEach( source: dictionary.AsParallel(), body: pair => this.Add( section: section, pair.Key, pair.Value ) ); }
			} );

			return true;
		}

		/// <summary>
		///     Removes all data from all sections.
		/// </summary>
		/// <returns></returns>
		public Boolean Clear() {
			Parallel.ForEach( source: this.Data.Keys, body: section => this.TryRemove( section: section ) );

			return !this.Data.Keys.Any();
		}

		/// <summary>
		///     Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///     <see langword="true" /> if the specified object is equal to the current object; otherwise,
		///     <see langword="false" />.
		/// </returns>
		public override Boolean Equals( Object obj ) => Equals( this, obj as Ini );

		/// <summary>
		///     Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override Int32 GetHashCode() => this.Data.GetHashCode();

		public async Task<Boolean> Reload( CancellationToken token = default ) {
			var document = this.Document;

			return await Task.Run( function: () => {

				if ( document?.Exists() != true ) { return false; }

				try {
					var data = document.LoadJSON<ConcurrentDictionary<String, Section>>();

					if ( data is null ) { return false; }

					var result = Parallel.ForEach( source: data.Keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive,
						body: section => Parallel.ForEach( source: data[ section ].Keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive,
							body: key => this.Add( section: section, key, data[ section ][ key ] ) ) );

					return result.IsCompleted;
				}
				catch ( JsonException exception ) { exception.More(); }
				catch ( IOException exception ) {

					//file in use by another app
					exception.More();
				}
				catch ( OutOfMemoryException exception ) {

					//file is huge. too huge!
					exception.More();
				}

				return false;
			}, cancellationToken: token );
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.Sections.Count()} sections, {this.Data.Keys.Count} keys";

		[DebuggerStepThrough]
		public Boolean TryRemove( [NotNull] String section ) {
			if ( section is null ) { throw new ArgumentNullException( nameof( section ) ); }

			return this.Data.TryRemove( section, out _ );
		}

		[DebuggerStepThrough]
		public Boolean TryRemove( [NotNull] String section, String key ) {
			if ( section is null ) { throw new ArgumentNullException( nameof( section ) ); }

			if ( !this.Data.ContainsKey( section ) ) { return false; }

			if ( !this.Data[ section ].Keys.Contains( key ) ) { return false; }

			this.Data[ section ][ key ] = null;

			return true;
		}

		/// <summary>
		///     Saves the <see cref="Data" /> to the <see cref="Document" />.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public async Task<Boolean> Write( CancellationToken cancellationToken = default ) {
			var document = this.Document;

			return await Task.Run( function: () => {
				if ( document is null ) { return false; }

				if ( document.Exists() ) { document.Delete(); }

				return this.Data.TrySave( document: document, overwrite: true, formatting: Formatting.Indented );
			}, cancellationToken: cancellationToken );
		}
	}
}