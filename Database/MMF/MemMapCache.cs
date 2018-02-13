// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/MemMapCache.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Database.MMF {

    using System;
    using System.Collections.Generic;
    using System.IO.MemoryMappedFiles;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using Magic;

    public class MemMapCache<T> : ABetterClassDispose {
        private const String Delim = "[!@#]";
        private readonly Dictionary<String, DateTime> _keyExpirations;

        //this is necessary because the lib still will hold refs to expired MMFs

        private BinaryFormatter _formatter;
        private NetworkStream _networkStream;
        private TcpClient _tcpClient;

		/// <summary>
		/// Dispose any disposable members.
		/// </summary>
		protected override void DisposeManaged() => this._tcpClient?.Dispose();

		public MemMapCache() {
            this.Encoding = Encoding.ASCII;
            this.ChunkSize = 1024 * 1024 * 30;

            this.Server = "127.0.0.1"; //limited to local
            this.Port = 57742;

            this.CacheHitAlwaysMiss = false;

            this._keyExpirations = new Dictionary<String, DateTime>();
        }

        public Boolean CacheHitAlwaysMiss {
            get;
        }

        public Int64 ChunkSize {
            get;
        }

        public Encoding Encoding {
            get;
        }

        public Boolean IsConnected => this._tcpClient.Connected;

        public static Int32 MaxKeyLength => 4096 - 32;

        public Int32 Port {
            get;
        }

        public String Server {
            get;
        }

        //ideal for Unit Testing of classes that depend upon this Library.

        //32 bytes for datetime String... it's an overkill i know

        public void Connect() {
            this._tcpClient = new TcpClient();
            this._tcpClient.Connect( this.Server, this.Port );
            this._networkStream = this._tcpClient.GetStream();
            this._formatter = new BinaryFormatter();
        }

        public T Get( String key ) {
            if ( !this.IsConnected ) {
                return default;
            }

            if ( this.CacheHitAlwaysMiss ) {
                return default;
            }

            try {
                using ( var memoryMappedFile = MemoryMappedFile.OpenExisting( key ) ) {
                    if ( this._keyExpirations.ContainsKey( key ) ) {
                        if ( DateTime.UtcNow >= this._keyExpirations[ key ] ) {
                            this._keyExpirations.Remove( key );
                            return default;
                        }
                    }

                    var viewStream = memoryMappedFile.CreateViewStream( offset: 0, size: 0 );

                    var o = this._formatter.Deserialize( serializationStream: viewStream );
                    return ( T )o;
                }
            }
            catch ( SerializationException ) {

                //throw;
                return default;
            }
            catch ( Exception ) {
                if ( this._keyExpirations.ContainsKey( key ) ) {
                    this._keyExpirations.Remove( key );
                }

                return default;
            }
        }

        public void Set( String key, T obj ) => this.Set( key, obj, this.ChunkSize, DateTime.MaxValue );

        public void Set( String key, T obj, Int64 size, DateTime expire ) {
            try {
                if ( String.IsNullOrEmpty( key ) ) {
                    throw new Exception( "The key can't be null or empty." );
                }

                if ( key.Length >= MaxKeyLength ) {
                    throw new Exception( "The key has exceeded the maximum length." );
                }

                if ( !this.IsConnected ) {
                    return;
                }

                expire = expire.ToUniversalTime();

                if ( !this._keyExpirations.ContainsKey( key ) ) {
                    this._keyExpirations.Add( key, expire );
                }
                else {
                    this._keyExpirations[ key ] = expire;
                }

                var mmf = MemoryMappedFile.CreateOrOpen( key, size );
                var vs = mmf.CreateViewStream();
                this._formatter.Serialize( vs, obj );

                var cmd = "{0}{1}{2}";
                cmd = String.Format( cmd, key, Delim, expire.ToString( "s" ) );

                var buf = this.Encoding.GetBytes( cmd );
                this._networkStream.Write( buf, 0, buf.Length );
                this._networkStream.Flush();
            }
            catch ( NotSupportedException exception ) {

                //Console.WriteLine( "{0} is too small for {1}.", size, key );
                exception.More();
            }
            catch ( Exception exception ) {

                //Console.WriteLine( "MemMapCache: Set Failed.\n\t" + ex.Message );
                exception.More();
            }
        }

        public void Set( String key, T obj, DateTime expire ) => this.Set( key, obj, this.ChunkSize, expire );

        public void Set( String key, T obj, TimeSpan expire ) {
            var expireDt = DateTime.Now.Add( expire );
            this.Set( key, obj, this.ChunkSize, expireDt );
        }

        public void Set( String key, T obj, Int64 size ) => this.Set( key, obj, size, DateTime.MaxValue );

        public T TryGetThenSet( String key, Func<T> cacheMiss ) => this.TryGetThenSet( key, DateTime.MaxValue, cacheMiss );

        public T TryGetThenSet( String key, DateTime expire, Func<T> cacheMiss ) {
            var obj = this.Get( key );
            if ( obj != null ) {
                return obj;
            }
            obj = cacheMiss.Invoke();
            this.Set( key, obj, expire );

            return obj;
        }

        public T TryGetThenSet( String key, TimeSpan expire, Func<T> cacheMiss ) {
            var expireDt = DateTime.Now.Add( expire );
            return this.TryGetThenSet( key, expireDt, cacheMiss );
        }

        public T TryGetThenSet( String key, Int64 size, TimeSpan expire, Func<T> cacheMiss ) {
            var expireDt = DateTime.Now.Add( expire );
            return this.TryGetThenSet( key, size, expireDt, cacheMiss );
        }

        public T TryGetThenSet( String key, Int64 size, DateTime expire, Func<T> cacheMiss ) {
            var obj = this.Get( key );
            if ( obj == null ) {
                obj = cacheMiss.Invoke();
                this.Set( key, obj, size, expire );
            }

            return obj;
        }



    }
}