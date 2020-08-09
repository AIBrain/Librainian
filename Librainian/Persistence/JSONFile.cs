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
	[JsonObject]
	public class JSONFile {

		/// <summary></summary>
		/// <param name="document"></param>
		public JSONFile( [CanBeNull] Document document ) : this() {
			this.Document = document;

			this.Document.ContainingingFolder().Create();

			this.Read().Wait();
		}

		public JSONFile() { }

		[NotNull]
		public IEnumerable<String> AllKeys => this.Sections.SelectMany( section => this.Data[section]?.Keys );

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
					return null;
				}

				if ( this.Data.ContainsKey( section ) ) {
					if ( this.Data.TryGetValue( section, out var result ) ) {
						return result;
					}
				}

				return null;
			}
		}

		/// <summary></summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		public String this[ [CanBeNull] String section, [CanBeNull] String key ] {
			[DebuggerStepThrough]
			[CanBeNull]
			get {
				if ( String.IsNullOrEmpty( section ) ) {
					return null;
				}

				if ( String.IsNullOrEmpty( key ) ) {
					return null;
				}

				if ( !this.Data.ContainsKey( section ) ) {
					return null;
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
		public Boolean TryRemove( String section, [CanBeNull] String key ) {
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