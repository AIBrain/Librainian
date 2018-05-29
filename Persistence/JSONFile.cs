// Copyright 2017 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/JSONFile.cs" was last cleaned by Rick on 2017/01/17 at 6:20 PM

namespace Librainian.Persistence {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using FileSystem;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	///     Persist a document to and from a JSON formatted text document.
	/// </summary>
	[ JsonObject ]
	public class JSONFile {

		/// <summary>
		/// </summary>
		/// <param name="document"></param>
		public JSONFile( Document document ) : this() {
			this.Document = document;
			if ( !this.Document.Folder.Exists() ) {
				this.Document.Folder.Create();
			}
			this.Read().Wait();
		}

		public JSONFile() { }

		public IEnumerable< String > AllKeys => this.Sections.SelectMany( section => this.Data[ section ].Keys );

		/// <summary>
		/// </summary>
		[ JsonProperty ]
		[ CanBeNull ]
		public Document Document { get; set; }

		public IEnumerable< String > Sections => this.Data.Keys;

		[ JsonProperty ]
		[ NotNull ]
		private ConcurrentDictionary< String, ConcurrentDictionary< String, String > > Data { [ DebuggerStepThrough ] get; } = new ConcurrentDictionary< String, ConcurrentDictionary< String, String > >();

		public IReadOnlyDictionary< String, String > this[ [ CanBeNull ] String section ] {
			[ DebuggerStepThrough ]
			[ CanBeNull ]
			get {
				if ( String.IsNullOrEmpty( section ) ) {
					return null;
				}
				if ( !this.Data.ContainsKey( section ) ) {
					return null;
				}
				return this.Data.TryGetValue( section, out var result ) ? result : null;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="section"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public String this[ [ CanBeNull ] String section, [ CanBeNull ] String key ] {
			//[DebuggerStepThrough]
			[ CanBeNull ]
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


				return this.Data[ section ].TryGetValue( key, out var value ) ? value : null;
			}

			//[DebuggerStepThrough]
			set {
				if ( String.IsNullOrEmpty( section ) ) {
					return;
				}
				if ( String.IsNullOrEmpty( key ) ) {
					return;
				}
				this.Add( section, new KeyValuePair< String, String >( key, value ) );
			}
		}

		/// <summary>
		///     <para>Add in all of the sections, and key-value-pairs from the <see cref="INIFile" />.</para>
		///     <para>Performs a file save.</para>
		/// </summary>
		/// <param name="iniFile"></param>
		/// <returns></returns>
		public Boolean Add( INIFile iniFile ) {
			if ( null == iniFile ) {
				return false;
			}

			Parallel.ForEach( iniFile.Sections.AsParallel(), ThreadingExtensions.CPUIntensive, section => {
																								   var dictionary = iniFile[ section ];
																								   if ( null != dictionary ) {
																									   Parallel.ForEach( dictionary.AsParallel(), ThreadingExtensions.CPUIntensive, pair => { this.Add( section, pair ); } );
																								   }
																							   } );

			return true;
		}

		/// <summary>
		///     Removes all data from all sections.
		/// </summary>
		/// <returns></returns>
		public Boolean Clear() {
			Parallel.ForEach( this.Data.Keys, section => { this.TryRemove( section ); } );
			return !this.Data.Keys.Any();
		}

		public Task< Boolean > Read( CancellationToken cancellationToken = default ) {
			var document = this.Document;

			return Task.Run( () => {
								 if ( document == null ) {
									 return false;
								 }
								 if ( !document.Exists() ) {
									 return false;
								 }

								 try {
									 var data = document.LoadJSON< ConcurrentDictionary< String, ConcurrentDictionary< String, String > > >();
									 if ( data == null ) {
										 return false;
									 }

									 var result = Parallel.ForEach( data.Keys.AsParallel(), ThreadingExtensions.CPUIntensive, section => { Parallel.ForEach( data[ section ].Keys.AsParallel(), ThreadingExtensions.CPUIntensive, key => { this.Add( section, new KeyValuePair< String, String >( key, data[ section ][ key ] ) ); } ); } );
									 return result.IsCompleted;
								 }
								 catch ( JsonException exception ) {
									 exception.More();
								 }
								 catch ( IOException exception ) {
									 //file in use by another app
									 exception.More();
								 }
								 catch ( OutOfMemoryException exception ) {
									 //file is huge
									 exception.More();
								 }

								 return false;
							 }, cancellationToken );
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override String ToString() => $"{this.Sections.Count()} sections, {this.AllKeys.Count()} keys";

		[ DebuggerStepThrough ]
		public Boolean TryRemove( String section ) {
			if ( section == null ) {
				throw new ArgumentNullException( nameof( section ) );
			}
			return this.Data.TryRemove( section, out var dict );
		}

		[ DebuggerStepThrough ]
		public Boolean TryRemove( String section, String key ) {
			if ( section == null ) {
				throw new ArgumentNullException( nameof( section ) );
			}
			if ( !this.Data.ContainsKey( section ) ) {
				return false;
			}
			return this.Data[ section ].TryRemove( key, out var value );
		}

		/// <summary>
		///     Saves the <see cref="Data" /> to the <see cref="Document" />.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[ NotNull ]
		public Task< Boolean > Write( CancellationToken cancellationToken = default ) {
			var document = this.Document;

			return Task.Run( () => {
								 if ( document == null ) {
									 return false;
								 }

								 if ( document.Exists() ) {
									 document.Delete();
								 }

								 return this.Data.Save( document, true, Formatting.Indented );
							 }, cancellationToken );
		}

		/// <summary>
		///     (Trims whitespaces from section and key)
		/// </summary>
		/// <param name="section"></param>
		/// <param name="pair"></param>
		/// <returns></returns>
		private Boolean Add( String section, KeyValuePair< String, String > pair ) {
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
				this.Data.TryAdd( section, new ConcurrentDictionary< String, String >() );
			}
			try {
				this.Data[ section ][ pair.Key.Trim() ] = pair.Value;
				return true;
			}
			catch ( KeyNotFoundException exception ) {
				retries--;
				if ( retries.Any() ) {
					goto TryAgain;
				}
				exception.More();
			}
			return false;
		}

	}

}
