namespace Librainian.Persistence {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Collections;
	using ComputerSystems.FileSystem;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Magic;
	using Maths;
	using Measurement.Time;
	using Microsoft.Database.Isam.Config;
	using Microsoft.Isam.Esent.Collections.Generic;
	using Microsoft.Isam.Esent.Interop.Windows81;
	using Newtonsoft.Json;
	using OperatingSystem.Compression;
	using Parsing;

	/// <summary>
	///     <para>
	///         Allows the <see cref="PersistentDictionary{String,String}" /> class to persist a <see cref="KeyValuePair{TKey,TValue}"/> of base64 compressed strings.
	///     </para>
	/// </summary>
	/// <seealso cref="http://managedesent.codeplex.com/wikipage?title=PersistentDictionaryDocumentation" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject]
	public sealed class StringKVPTable : ABetterClassDispose, IDictionary<String, String> {

		[JsonProperty]
		[NotNull]
		private PersistentDictionary<String, String> Dictionary { get; }

		// ReSharper disable once NotNullMemberIsNotInitialized
		private StringKVPTable() => throw new NotImplementedException();

		/// <summary>
		///     Return true if we can read/write in the <see cref="Folder" /> .
		/// </summary>
		/// <returns></returns>
		private Boolean TestForReadWriteAccess() {
			try {
				if ( this.Folder.TryGetTempDocument( document: out var document ) ) {
					var text = Randem.NextString( 64, lowers: true, uppers: true, numbers: true, symbols: true );
					document.AppendText( text: text );
					document.TryDeleting( tryFor: Seconds.Five );

					return true;
				}
			}
			catch { }

			return false;
		}

		/// <summary>
		///     No path given?
		/// </summary>
		[NotNull]
		public Folder Folder { get; }

		public Int32 Count => this.Dictionary.Count;

		public Boolean IsReadOnly => this.Dictionary.IsReadOnly;

		public ICollection<String> Keys => this.Dictionary.Keys;

		public ICollection<String> Values => this.Dictionary.Values.Select( selector: value => value.FromCompressedBase64() ) as ICollection<String> ?? new Collection<String>();

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( Environment.SpecialFolder specialFolder, [NotNull] String tableName ) : this( folder: new Folder( specialFolder: specialFolder, applicationName: null, subFolder: tableName ) ) { }

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( Environment.SpecialFolder specialFolder, String subFolder, [NotNull] String tableName ) : this( folder: new Folder( specialFolder, subFolder, tableName ) ) { }

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( [NotNull] Folder folder, [NotNull] String tableName ) : this( fullpath: Path.Combine( path1: folder.FullName, path2: tableName ) ) { }

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( [NotNull] Folder folder, [NotNull] String subFolder, [NotNull] String tableName ) : this( fullpath: Path.Combine( folder.FullName, subFolder, tableName ) ) { }

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( [CanBeNull] Folder folder, Boolean testForReadWriteAccess = false ) {
			try {
				this.Folder = folder ?? throw new ArgumentNullException( nameof( folder ) );

				if ( !this.Folder.Create() ) {
					throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
				}

				var customConfig = new DatabaseConfig {
					CreatePathIfNotExist = true,
					EnableShrinkDatabase = ShrinkDatabaseGrbit.On,
					DefragmentSequentialBTrees = true
				};

				this.Dictionary = new PersistentDictionary<String, String>( directory: this.Folder.FullName, customConfig: customConfig );

				if ( testForReadWriteAccess && !this.TestForReadWriteAccess() ) {
					throw new IOException( $"Read/write permissions denied in folder {this.Folder.FullName}." );
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}
		}

		// ReSharper disable once NotNullMemberIsNotInitialized
		public StringKVPTable( [NotNull] String fullpath ) : this( folder: new Folder( fullPath: fullpath ) ) { }

		/// <summary>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[CanBeNull]
		public String this[[NotNull] String key] {
			[CanBeNull]
			get {
				if ( key == null ) {
					throw new ArgumentNullException( paramName: nameof( key ) );
				}

				return this.Dictionary.TryGetValue( key, out var storedValue ) ? storedValue.FromCompressedBase64() : default;
			}

			set {
				if ( key == null ) {
					throw new ArgumentNullException( paramName: nameof( key ) );
				}

				if ( String.IsNullOrEmpty( value ) ) {
					this.Dictionary.Remove( key );

					return;
				}

				this.Dictionary[key] = value.ToCompressedBase64();
			}
		}

		[CanBeNull]
		public String this[[NotNull] params String[] keys] {

			[CanBeNull]
			get {
				if ( keys == null ) {
					throw new ArgumentNullException( paramName: nameof( keys ) );
				}

				var key = this.BuildKey( keys );

				return this.Dictionary.TryGetValue( key, out var storedValue ) ? storedValue.FromCompressedBase64() : default;
			}

			set {
				if ( keys == null ) {
					throw new ArgumentNullException( paramName: nameof( keys ) );
				}

				var key = this.BuildKey( keys );

				if ( String.IsNullOrEmpty( value ) ) {
					this.Dictionary.Remove( key );

					return;
				}

				this.Dictionary[key] = value.ToCompressedBase64();
			}
		}

		public void Add( String key, String value ) => this[key] = value;

		public void Add( KeyValuePair<String, String> item ) => this[item.Key] = item.Value;

		public String BuildKey( [NotNull] params String[] objs ) {
			if ( objs == null ) {
				throw new ArgumentNullException( paramName: nameof( objs ) );
			}

			if ( objs.All( String.IsNullOrEmpty ) ) {
				throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( objs ) );
			}

			return objs.Where( s => !String.IsNullOrEmpty( s ) ).ToStrings( separator: "⦀" );
		}

		public void Clear() => this.Dictionary.Clear();

		public Boolean Contains( KeyValuePair<String, String> item ) {
			var value = item.Value.ToJSON().ToCompressedBase64();
			var asItem = new KeyValuePair<String, String>( item.Key, value );

			return this.Dictionary.Contains( asItem );
		}

		public Boolean ContainsKey( String key ) => this.Dictionary.ContainsKey( key );

		public void CopyTo( KeyValuePair<String, String>[] array, Int32 arrayIndex ) => throw new NotImplementedException(); //this.Dictionary.CopyTo( array, arrayIndex ); ??

		/// <summary>
		///     Dispose any disposable managed fields or properties.
		/// </summary>
		public override void DisposeManaged() {
			Trace.Write( $"Disposing of {nameof( this.Dictionary )}..." );

			using ( this.Dictionary ) { }

			Trace.WriteLine( "done." );
			base.DisposeManaged();
		}

		/// <summary>
		/// Force all changes to be written to disk.
		/// </summary>
		public void Flush() => this.Dictionary.Flush();

		public IEnumerator<KeyValuePair<String, String>> GetEnumerator() => this.Items().GetEnumerator();

		public void Initialize() {
			Logging.Enter();
			this.Dictionary.Database.Should().NotBeNull();

			if ( this.Dictionary.Database.ToString().IsNullOrWhiteSpace() ) {
				throw new DirectoryNotFoundException( $"Unable to find or create the folder `{this.Folder.FullName}`." );
			}

			Logging.Exit();
		}

		/// <summary>
		///     All <see cref="KeyValuePair{TKey,TValue }" /> , with the <see cref="String" /> deserialized.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		public IEnumerable<KeyValuePair<String, String>> Items() => this.Dictionary.Select( selector: pair => new KeyValuePair<String, String>( pair.Key, pair.Value.FromCompressedBase64() ) );

		/// <summary>
		///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" /> .
		/// </summary>
		/// <returns>
		///     true if the element is successfully removed; otherwise, false. This method also returns false if
		///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" /> .
		/// </returns>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is
		///     read-only.
		/// </exception>
		public Boolean Remove( String key ) => this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );

		/// <summary>
		///     Removes the first occurrence of a specific object from the
		///     <see cref="T:System.Collections.Generic.ICollection`1" /> .
		/// </summary>
		/// <returns>
		///     true if <paramref name="item" /> was successfully removed from the
		///     <see cref="T:System.Collections.Generic.ICollection`1" /> ; otherwise, false. This method also returns false if
		///     <paramref name="item" /> is not
		///     found in the original <see cref="T:System.Collections.Generic.ICollection`1" /> .
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" /> .</param>
		/// <exception cref="T:System.NotSupportedException">
		///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is
		///     read-only.
		/// </exception>
		public Boolean Remove( KeyValuePair<String, String> item ) {
			var value = item.Value.ToJSON().ToCompressedBase64();
			var asItem = new KeyValuePair<String, String>( item.Key, value );

			return this.Dictionary.Remove( item: asItem );
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override String ToString() => $"{this.Count} items";

		public void TryAdd( [NotNull] String key, String value ) {
			if ( key == null ) {
				throw new ArgumentNullException( paramName: nameof( key ) );
			}

			if ( !this.Dictionary.ContainsKey( key ) ) {
				this[key] = value;
			}
		}

		/// <summary>
		///     Gets the value associated with the specified key.
		/// </summary>
		/// <returns>
		///     true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
		///     element with the specified key; otherwise, false.
		/// </returns>
		/// <param name="key">  The key whose value to get.</param>
		/// <param name="value">
		///     When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
		///     value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		public Boolean TryGetValue( [NotNull] String key, out String value ) {
			if ( key == null ) {
				throw new ArgumentNullException( paramName: nameof( key ) );
			}

			value = default;

			if ( this.Dictionary.TryGetValue( key, out var storedValue ) ) {
				value = storedValue.FromCompressedBase64();
				return true;
			}

			return false;
		}

		public Boolean TryRemove( [NotNull] String key ) {
			if ( key == null ) {
				throw new ArgumentNullException( paramName: nameof( key ) );
			}

			return this.Dictionary.ContainsKey( key ) && this.Dictionary.Remove( key );
		}

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public void Save() {
			this.Flush();	//should be all that's needed..
		}

	}
}