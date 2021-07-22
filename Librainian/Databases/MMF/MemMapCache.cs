// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "MemMapCache.cs" last touched on 2021-03-07 at 10:01 AM by Protiguous.

#nullable enable

namespace Librainian.Databases.MMF {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO.MemoryMappedFiles;
	using System.Net.Sockets;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using Exceptions;
	using Logging;
	using Utilities;
	using Utilities.Disposables;

	[Obsolete( "Unfinished attempt at caching." )]
	public class MemMapCache<T> : ABetterClassDispose {

		private const String Delim = "[!@#]";

		private NetworkStream? _networkStream;

		private BinaryFormatter _formatter { get; } = new();

		private Dictionary<String, DateTime> _keyExpirations { get; } = new();

		private TcpClient _tcpClient { get; } = new();

		public static Int32 MaxKeyLength => 4096 - 32;

		public Int64 ChunkSize => 1024 * 1024 * 30;

		public Encoding Encoding { get; } = Encoding.Unicode;

		public Boolean IsConnected => this._tcpClient.Connected;

		public Int32 Port => 57742;

		public String Server => "127.0.0.1";

		public void Connect() {
			this._tcpClient.Connect( this.Server, this.Port );
			this._networkStream = this._tcpClient.GetStream();
		}

		/// <summary>Dispose any disposable members.</summary>
		public override void DisposeManaged() {
			using ( this._tcpClient ) { }
		}

		public T? Get( String key ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			if ( !this.IsConnected ) {
				return default( T );
			}

			try {
				if ( this._keyExpirations.ContainsKey( key ) ) {
					if ( DateTime.UtcNow >= this._keyExpirations[ key ] ) {
						this._keyExpirations.Remove( key );

						return default( T );
					}
				}

#pragma warning disable CA1416 // Validate platform compatibility
				using var memoryMappedFile = MemoryMappedFile.OpenExisting( key );
#pragma warning restore CA1416 // Validate platform compatibility

				using var viewStream = memoryMappedFile.CreateViewStream( 0, 0 ); //TODO

				var o = this._formatter.Deserialize( viewStream );

				return o is T o1 ? o1 : default( T );
			}
			catch ( SerializationException ) {

				//throw;
				return default( T );
			}
			catch ( Exception ) {
				if ( this._keyExpirations.ContainsKey( key ) ) {
					this._keyExpirations.Remove( key );
				}

				return default( T );
			}
		}

		//ideal for Unit Testing of classes that depend upon this Library.
		public void Set( String key, [DisallowNull] T obj ) {
			if ( obj is null ) {
				throw new ArgumentEmptyException( nameof( obj ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			this.Set( key, obj, this.ChunkSize, DateTime.MaxValue );
		}

		public void Set( String? key, [DisallowNull] T obj, Int64 size, DateTime expire ) {
			if ( String.IsNullOrEmpty( key ) ) {
				throw new Exception( "The key can't be null or empty." );
			}

			if ( key.Length >= MaxKeyLength ) {
				throw new Exception( "The key has exceeded the maximum length." );
			}

			if ( obj is null ) {
				throw new ArgumentEmptyException( nameof( obj ) );
			}

			if ( !this.IsConnected ) {
				return;
			}

			try {
				expire = expire.ToUniversalTime();

				if ( !this._keyExpirations.ContainsKey( key ) ) {
					this._keyExpirations.Add( key, expire );
				}
				else {
					this._keyExpirations[ key ] = expire;
				}

#pragma warning disable CA1416 // Validate platform compatibility
				using ( var mmf = MemoryMappedFile.CreateOrOpen( key, size ) ) {
#pragma warning restore CA1416 // Validate platform compatibility
					var vs = mmf.CreateViewStream();
					this._formatter.Serialize( vs, obj );
				}

				var cmd = $"{key}{Delim}{expire:s}";

				var buf = this.Encoding.GetBytes( cmd );

				var networkStream = this._networkStream;

				if ( networkStream != null ) {
					networkStream.Write( buf, 0, buf.Length );
					networkStream.Flush();
				}
			}
			catch ( NotSupportedException exception ) {

				//Console.WriteLine( "{0} is too small for {1}.", size, key );
				exception.Log();
			}
			catch ( Exception exception ) {

				//Console.WriteLine( "MemMapCache: Set Failed.\n\t" + ex.Message );
				exception.Log();
			}
		}

		public void Set( String key, [DisallowNull] T obj, DateTime expire ) {
			if ( obj is null ) {
				throw new ArgumentEmptyException( nameof( obj ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			this.Set( key, obj, this.ChunkSize, expire );
		}

		public void Set( String key, [DisallowNull] T obj, TimeSpan expire ) {
			if ( obj is null ) {
				throw new ArgumentEmptyException( nameof( obj ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			var expireDt = DateTime.Now.Add( expire );
			this.Set( key, obj, this.ChunkSize, expireDt );
		}

		public void Set( String key, [DisallowNull] T obj, Int64 size ) {
			if ( obj is null ) {
				throw new ArgumentEmptyException( nameof( obj ) );
			}

			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			this.Set( key, obj, size, DateTime.MaxValue );
		}

		public T? TryGetThenSet( String key, Func<T>? cacheMiss ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			return this.TryGetThenSet( key, DateTime.MaxValue, cacheMiss );
		}

		public T? TryGetThenSet( String key, DateTime expire, Func<T>? cacheMiss ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			var obj = this.Get( key );

			if ( obj is not null ) {
				return obj;
			}

			if ( cacheMiss is null ) {
				return obj;
			}

			obj = cacheMiss.Invoke();

			if ( obj is not null ) {
				this.Set( key, obj, expire );
			}

			return obj;
		}

		public T? TryGetThenSet( String key, TimeSpan expire, Func<T>? cacheMiss ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			var expireDt = DateTime.Now.Add( expire );

			return this.TryGetThenSet( key, expireDt, cacheMiss );
		}

		public T? TryGetThenSet( String key, Int64 size, TimeSpan expire, Func<T>? cacheMiss ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( key ) );
			}

			var expireDt = DateTime.Now.Add( expire );

			return this.TryGetThenSet( key, size, expireDt, cacheMiss );
		}

		public T? TryGetThenSet( String key, Int64 size, DateTime expire, Func<T>? cacheMiss ) {
			if ( String.IsNullOrWhiteSpace( key ) ) {
				throw new ArgumentEmptyException( nameof( key ) );
			}

			var obj = this.Get( key );

			if ( obj is not null ) {
				return obj;
			}

			if ( cacheMiss is null ) {
				return obj;
			}

			obj = cacheMiss.Invoke();

			if ( obj is not null ) {
				this.Set( key, obj, size, expire );
			}

			return obj;
		}
	}
}