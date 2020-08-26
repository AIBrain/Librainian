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
// File "JSONFile.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Maths;
	using Newtonsoft.Json;
	using OperatingSystem.FileSystem;

	/// <summary>Persist a document to and from a JSON formatted text document.</summary>
	/// TODO Needs testing.
	[JsonObject]
	public class JSONFile {

		/// <summary></summary>
		/// <param name="document"></param>
		public JSONFile( [CanBeNull] Document document ) : this() {
			this.Document = document;

			this.Document?.ContainingingFolder().Create();

			this.Read().Wait();
		}

		public JSONFile() { }

		[NotNull]
		public IEnumerable<String?>? AllKeys =>
			this.Sections.SelectMany( section => {
				var keys = this.Data[section]?.Keys;

				return keys!;
			} );

		/// <summary></summary>
		[JsonProperty]
		[CanBeNull]
		public Document? Document { get; set; }

		[NotNull]
		public IEnumerable<String> Sections => this.Data.Keys;

		[JsonProperty]
		[NotNull]
		private ConcurrentDictionary<String, ConcurrentDictionary<String, String>> Data {
			[DebuggerStepThrough]
			get;
		} = new ConcurrentDictionary<String, ConcurrentDictionary<String, String>>();

		[CanBeNull]
		public IReadOnlyDictionary<String?, String?> this[ [CanBeNull] String? section ] {
			[DebuggerStepThrough]
			[CanBeNull]
			get {
				if ( String.IsNullOrEmpty( section ) ) {
					return default;
				}

				if ( this.Data.ContainsKey( section ) ) {
					if ( this.Data.TryGetValue( section, out var result ) ) {
						return result;
					}
				}

				return default;
			}
		}

		/// <summary></summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		public String this[ [CanBeNull] String? section, [CanBeNull] String? key ] {
			[DebuggerStepThrough]
			[CanBeNull]
			get {
				if ( String.IsNullOrEmpty( section ) ) {
					return default;
				}

				if ( String.IsNullOrEmpty( key ) ) {
					return default;
				}

				if ( !this.Data.ContainsKey( section ) ) {
					return default;
				}

				return this.Data[section].TryGetValue( key, out var value ) ? value : null;
			}

			[DebuggerStepThrough]
			set {
				if ( String.IsNullOrEmpty( section ) ) {
					return;
				}

				if ( String.IsNullOrEmpty( key ) ) {
					return;
				}

				this.Add( section, new KeyValuePair<String, String>( key, value ) );
			}
		}

		/// <summary>Removes all data from all sections.</summary>
		/// <returns></returns>
		public Boolean Clear() {
			Parallel.ForEach( this.Data.Keys, section => this.TryRemove( section ) );

			return !this.Data.Keys.Any();
		}

		[NotNull]
		public Task<Boolean> Read( CancellationToken cancellationToken = default ) {
			var document = this.Document;

			return Task.Run( () => {
				if ( !document.Exists() ) {
					return false;
				}

				try {
					var data = document.LoadJSON<ConcurrentDictionary<String, ConcurrentDictionary<String, String>>>();

					if ( data == null ) {
						return false;
					}

					var result = Parallel.ForEach( data.Keys.AsParallel(),
												   section => Parallel.ForEach( data[section].Keys.AsParallel().AsUnordered(),
																				key => this.Add( section, new KeyValuePair<String, String>( key, data[section][key] ) ) ) );

					return result.IsCompleted;
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

				return false;
			}, cancellationToken );
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		[NotNull]
		public override String ToString() => $"{this.Sections.Count()} sections, {this.AllKeys.Count()} keys";

		[DebuggerStepThrough]
		public Boolean TryRemove( String section ) {
			if ( section == null ) {
				throw new ArgumentNullException( nameof( section ) );
			}

			return this.Data.TryRemove( section, out var dict );
		}

		[DebuggerStepThrough]
		public Boolean TryRemove( String section, [CanBeNull] String? key ) {
			if ( section == null ) {
				throw new ArgumentNullException( nameof( section ) );
			}

			if ( !this.Data.ContainsKey( section ) ) {
				return false;
			}

			return this.Data[section].TryRemove( key, out var value );
		}

		/// <summary>Saves the <see cref="Data" /> to the <see cref="Document" />.</summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[NotNull]
		public Task<Boolean> Write( CancellationToken cancellationToken = default ) {
			var document = this.Document;

			return Task.Run( () => {
				if ( document.Exists() ) {
					document.Delete();
				}

				return this.Data.TrySave( document, true, Formatting.Indented );
			}, cancellationToken );
		}

		/// <summary>(Trims whitespaces from section and key)</summary>
		/// <param name="section"></param>
		/// <param name="pair"></param>
		/// <returns></returns>
		public Boolean Add( String section, KeyValuePair<String, String> pair ) {
			if ( String.IsNullOrWhiteSpace( section ) ) {
				throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
			}

			section = section.Trim();

			if ( String.IsNullOrWhiteSpace( section ) ) {
				throw new ArgumentException( "Argument is null or whitespace", nameof( section ) );
			}

			var retries = 10;
			TryAgain:

			if ( !this.Data.ContainsKey( section ) ) {
				this.Data.TryAdd( section, new ConcurrentDictionary<String, String>() );
			}

			try {
				this.Data[section][pair.Key.Trim()] = pair.Value;

				return true;
			}
			catch ( KeyNotFoundException exception ) {
				retries--;

				if ( retries.Any() ) {
					goto TryAgain;
				}

				exception.Log();
			}

			return false;
		}

	}

}