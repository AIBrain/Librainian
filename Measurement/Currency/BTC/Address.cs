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
// "Librainian/Address.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using Annotations;
    using Parsing;

    /// <summary>
    /// </summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    [UsedImplicitly]
    public class Address {
        public const Byte PUBKEYHASH = 0x00;
        public const Byte SCRIPTHASH = 0x05;
        public const Byte PUBKEY = 0xFE;
        public const Byte SCRIPT = 0xFF;
        private static readonly ThreadLocal< SHA256 > SHA256 = new ThreadLocal< SHA256 >( () => new SHA256Managed() );

        private String _address;
        private Hash _pubKeyHash;
        private Hash _scriptHash;
        private Byte? _type;

        public Address( Byte[] data, Byte version = PUBKEY ) {
            RIPEMD160 ripemd160 = new RIPEMD160Managed();
            switch ( version ) {
                case PUBKEY:
                    this._pubKeyHash = ripemd160.ComputeHash( SHA256.Value.ComputeHash( data ) );
                    version = PUBKEYHASH;
                    break;
                case SCRIPT:
                    this._scriptHash = ripemd160.ComputeHash( SHA256.Value.ComputeHash( data ) );
                    version = SCRIPTHASH;
                    break;
                case PUBKEYHASH:
                    this._pubKeyHash = data;
                    break;
                case SCRIPTHASH:
                    this._scriptHash = data;
                    break;
            }
            this._type = version;
        }

        public Address( String address ) {
            this._address = address;
        }

        public Hash PubKeyHash {
            get {
                if ( this._pubKeyHash == null && this.CalcHash() != PUBKEYHASH ) {
                    throw new InvalidOperationException( "Address is not a public key hash." );
                }
                return this._pubKeyHash;
            }
        }

        public Hash ScriptHash {
            get {
                if ( this._pubKeyHash == null && this.CalcHash() != SCRIPTHASH ) {
                    throw new InvalidOperationException( "Address is not a script hash." );
                }
                return this._scriptHash;
            }
        }

        public Hash EitherHash {
            get {
                if ( this._pubKeyHash == null && this._scriptHash == null ) {
                    this.CalcHash();
                }
                if ( this._pubKeyHash != null ) {
                    return this._pubKeyHash;
                }
                if ( this._scriptHash != null ) {
                    return this._scriptHash;
                }
                return null;
            }
        }

        public Byte Type {
            get {
                if ( this._type == null ) {
                    this.CalcHash();
                }
                if ( this._type == null ) {
                    throw new InvalidOperationException();
                }
                return this._type.Value;
            }
        }

        private Byte CalcHash() {
            Byte version;
            var hash = Base58CheckString.ToByteArray( this.ToString(), out version );
            switch ( version ) {
                case PUBKEYHASH:
                    this._pubKeyHash = hash;
                    break;
                case SCRIPTHASH:
                    this._scriptHash = hash;
                    break;
            }
            this._type = version;
            return version;
        }

        private void CalcBase58() {
            if ( this._pubKeyHash != null ) {
                this._address = Base58CheckString.FromByteArray( this._pubKeyHash, PUBKEYHASH );
            }
            else if ( this._scriptHash != null ) {
                this._address = Base58CheckString.FromByteArray( this._scriptHash, SCRIPTHASH );
            }
            else {
                throw new InvalidOperationException( "Address is not a public key or script hash!" );
            }
        }

        public override Boolean Equals( object obj ) {
            if ( !( obj is Address ) ) {
                return false;
            }
            if ( this.EitherHash == null || ( ( Address ) obj ).EitherHash == null ) {
                return false;
            }
            return this.EitherHash.HashBytes.SequenceEqual( ( ( Address ) obj ).EitherHash.HashBytes );
        }

        public override int GetHashCode() {
            return this.EitherHash.GetHashCode(); //TODO possible bug here.
        }

        public override String ToString() {
            if ( this._address == null ) {
                this.CalcBase58();
            }
            return this._address;
        }
    }
}
