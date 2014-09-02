#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/MemMapCache.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Database.MMF {
    using System;
    using System.Collections.Generic;
    using System.IO.MemoryMappedFiles;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    public class MemMapCache< T > {
        private const String DELIM = "[!@#]";

        private readonly Dictionary< String, DateTime > _keyExpirations;

        //this is necessary because the lib still will hold refs to expired MMFs

        private BinaryFormatter _formatter;
        private NetworkStream _networkStream;
        private TcpClient _tcpClient;

        public MemMapCache() {
            this.Encoding = Encoding.ASCII;
            this.ChunkSize = 1024*1024*30; //10MB

            this.Server = "127.0.0.1"; //limited to local
            this.Port = 57742;

            this.CacheHitAlwaysMiss = false;

            this._keyExpirations = new Dictionary< String, DateTime >();
        }

        public static int MaxKeyLength { get { return 4096 - 32; } }

        public Boolean CacheHitAlwaysMiss { get; set; }

        public long ChunkSize { get; set; }

        public Encoding Encoding { get; set; }

        public Boolean IsConnected { get { return this._tcpClient.Connected; } }

        public int Port { get; set; }

        public String Server { get; protected set; }

        //ideal for Unit Testing of classes that depend upon this library.

        //32 bytes for datetime String... it's an overkill i know

        public void Connect() {
            this._tcpClient = new TcpClient();
            this._tcpClient.Connect( this.Server, this.Port );
            this._networkStream = this._tcpClient.GetStream();
            this._formatter = new BinaryFormatter();
        }

        public T Get( String key ) {
            if ( !this.IsConnected ) {
                return default( T );
            }

            if ( this.CacheHitAlwaysMiss ) {
                return default( T );
            }

            try {
                using ( var memoryMappedFile = MemoryMappedFile.OpenExisting( key ) ) {
                    if ( this._keyExpirations.ContainsKey( key ) ) {
                        if ( DateTime.UtcNow >= this._keyExpirations[ key ] ) {
                            memoryMappedFile.Dispose();
                            this._keyExpirations.Remove( key );
                            return default( T );
                        }
                    }

                    var viewStream = memoryMappedFile.CreateViewStream( offset: 0, size: 0 );

                    var o = this._formatter.Deserialize( serializationStream: viewStream );
                    return ( T ) o;
                }
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

        public void Set( String key, T obj ) {
            this.Set( key, obj, this.ChunkSize, DateTime.MaxValue );
        }

        public void Set( String key, T obj, long size, DateTime expire ) {
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
                cmd = String.Format( cmd, key, DELIM, expire.ToString( "s" ) );

                var buf = this.Encoding.GetBytes( cmd );
                this._networkStream.Write( buf, 0, buf.Length );
                this._networkStream.Flush();
            }
            catch ( NotSupportedException exception ) {
                //Console.WriteLine( "{0} is too small for {1}.", size, key );
                exception.Error();
            }
            catch ( Exception exception ) {
                //Console.WriteLine( "MemMapCache: Set Failed.\n\t" + ex.Message );
                exception.Error();
            }
        }

        public void Set( String key, T obj, DateTime expire ) {
            this.Set( key, obj, this.ChunkSize, expire );
        }

        public void Set( String key, T obj, TimeSpan expire ) {
            var expireDT = DateTime.Now.Add( expire );
            this.Set( key, obj, this.ChunkSize, expireDT );
        }

        public void Set( String key, T obj, long size ) {
            this.Set( key, obj, size, DateTime.MaxValue );
        }

        public T TryGetThenSet( String key, Func< T > cacheMiss ) {
            return this.TryGetThenSet( key, DateTime.MaxValue, cacheMiss );
        }

        public T TryGetThenSet( String key, DateTime expire, Func< T > cacheMiss ) {
            var obj = this.Get( key );
            if ( obj != null ) {
                return obj;
            }
            obj = cacheMiss.Invoke();
            this.Set( key, obj, expire );

            return obj;
        }

        public T TryGetThenSet( String key, TimeSpan expire, Func< T > cacheMiss ) {
            var expireDT = DateTime.Now.Add( expire );
            return this.TryGetThenSet( key, expireDT, cacheMiss );
        }

        public T TryGetThenSet( String key, long size, TimeSpan expire, Func< T > cacheMiss ) {
            var expireDT = DateTime.Now.Add( expire );
            return this.TryGetThenSet( key, size, expireDT, cacheMiss );
        }

        public T TryGetThenSet( String key, long size, DateTime expire, Func< T > cacheMiss ) {
            var obj = this.Get( key );
            if ( obj == null ) {
                obj = cacheMiss.Invoke();
                this.Set( key, obj, size, expire );
            }

            return obj;
        }
    }
}
