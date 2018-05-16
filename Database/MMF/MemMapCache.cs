// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "MemMapCache.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/MemMapCache.cs" was last cleaned by Protiguous on 2018/05/15 at 10:39 PM.

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

        private BinaryFormatter _formatter;

        private NetworkStream _networkStream;

        private TcpClient _tcpClient;

        public MemMapCache() {
            this.Encoding = Encoding.ASCII;
            this.ChunkSize = 1024 * 1024 * 30;

            this.Server = "127.0.0.1"; //limited to local
            this.Port = 57742;

            this.CacheHitAlwaysMiss = false;

            this._keyExpirations = new Dictionary<String, DateTime>();
        }

        public static Int32 MaxKeyLength => 4096 - 32;

        public Boolean CacheHitAlwaysMiss { get; }

        public Int64 ChunkSize { get; }

        public Encoding Encoding { get; }

        public Boolean IsConnected => this._tcpClient.Connected;

        public Int32 Port { get; }

        public String Server { get; }

        public void Connect() {
            this._tcpClient = new TcpClient();
            this._tcpClient.Connect( hostname: this.Server, port: this.Port );
            this._networkStream = this._tcpClient.GetStream();
            this._formatter = new BinaryFormatter();
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this._tcpClient?.Dispose();

        //32 bytes for datetime String... it's an overkill i know
        public T Get( String key ) {
            if ( !this.IsConnected ) { return default; }

            if ( this.CacheHitAlwaysMiss ) { return default; }

            try {
                using ( var memoryMappedFile = MemoryMappedFile.OpenExisting( mapName: key ) ) {
                    if ( this._keyExpirations.ContainsKey( key ) ) {
                        if ( DateTime.UtcNow >= this._keyExpirations[key] ) {
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
                if ( this._keyExpirations.ContainsKey( key ) ) { this._keyExpirations.Remove( key ); }

                return default;
            }
        }

        //ideal for Unit Testing of classes that depend upon this Library.
        public void Set( String key, T obj ) => this.Set( key, obj, size: this.ChunkSize, expire: DateTime.MaxValue );

        public void Set( String key, T obj, Int64 size, DateTime expire ) {
            try {
                if ( String.IsNullOrEmpty( key ) ) { throw new Exception( "The key can't be null or empty." ); }

                if ( key.Length >= MaxKeyLength ) { throw new Exception( "The key has exceeded the maximum length." ); }

                if ( !this.IsConnected ) { return; }

                expire = expire.ToUniversalTime();

                if ( !this._keyExpirations.ContainsKey( key ) ) { this._keyExpirations.Add( key, expire ); }
                else { this._keyExpirations[key] = expire; }

                var mmf = MemoryMappedFile.CreateOrOpen( mapName: key, capacity: size );
                var vs = mmf.CreateViewStream();
                this._formatter.Serialize( serializationStream: vs, graph: obj );

                var cmd = "{0}{1}{2}";
                cmd = String.Format( format: cmd, arg0: key, arg1: Delim, arg2: expire.ToString( format: "s" ) );

                var buf = this.Encoding.GetBytes( s: cmd );
                this._networkStream.Write( buffer: buf, offset: 0, size: buf.Length );
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

        public void Set( String key, T obj, DateTime expire ) => this.Set( key, obj, size: this.ChunkSize, expire: expire );

        public void Set( String key, T obj, TimeSpan expire ) {
            var expireDt = DateTime.Now.Add( expire );
            this.Set( key, obj, size: this.ChunkSize, expire: expireDt );
        }

        public void Set( String key, T obj, Int64 size ) => this.Set( key, obj, size: size, expire: DateTime.MaxValue );

        public T TryGetThenSet( String key, Func<T> cacheMiss ) => this.TryGetThenSet( key, expire: DateTime.MaxValue, cacheMiss: cacheMiss );

        public T TryGetThenSet( String key, DateTime expire, Func<T> cacheMiss ) {
            var obj = this.Get( key );

            if ( obj != null ) { return obj; }

            obj = cacheMiss.Invoke();
            this.Set( key, obj, expire: expire );

            return obj;
        }

        public T TryGetThenSet( String key, TimeSpan expire, Func<T> cacheMiss ) {
            var expireDt = DateTime.Now.Add( expire );

            return this.TryGetThenSet( key, expire: expireDt, cacheMiss: cacheMiss );
        }

        public T TryGetThenSet( String key, Int64 size, TimeSpan expire, Func<T> cacheMiss ) {
            var expireDt = DateTime.Now.Add( expire );

            return this.TryGetThenSet( key, size: size, expire: expireDt, cacheMiss: cacheMiss );
        }

        public T TryGetThenSet( String key, Int64 size, DateTime expire, Func<T> cacheMiss ) {
            var obj = this.Get( key );

            if ( obj == null ) {
                obj = cacheMiss.Invoke();
                this.Set( key, obj, size: size, expire: expire );
            }

            return obj;
        }
    }
}