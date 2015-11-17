// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Address.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using Parsing;

    /// <summary></summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    public class Address {
        public const Byte Pubkey = 0xFE;
        public const Byte Pubkeyhash = 0x00;
        public const Byte Script = 0xFF;
        public const Byte Scripthash = 0x05;
        private static readonly ThreadLocal<SHA256> SHA256 = new ThreadLocal<SHA256>( () => new SHA256Managed() );
        private String _address;
        private Hash _pubKeyHash;
        private Hash _scriptHash;
        private Byte? _type;

        public Hash EitherHash {
            get {
                if ( ( this._pubKeyHash == null ) && ( this._scriptHash == null ) ) {
                    this.CalcHash();
                }
                if ( this._pubKeyHash != null ) {
                    return this._pubKeyHash;
                }
                return this._scriptHash;
            }
        }

        public Hash PubKeyHash {
            get {
                if ( ( this._pubKeyHash == null ) && ( this.CalcHash() != Pubkeyhash ) ) {
                    throw new InvalidOperationException( "Address is not a public key hash." );
                }
                return this._pubKeyHash;
            }
        }

        public Hash ScriptHash {
            get {
                if ( ( this._pubKeyHash == null ) && ( this.CalcHash() != Scripthash ) ) {
                    throw new InvalidOperationException( "Address is not a script hash." );
                }
                return this._scriptHash;
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

        public Address(Byte[] data, Byte version = Pubkey) {
            RIPEMD160 ripemd160 = new RIPEMD160Managed();
            switch ( version ) {
                case Pubkey:
                    this._pubKeyHash = ripemd160.ComputeHash( SHA256.Value.ComputeHash( data ) );
                    version = Pubkeyhash;
                    break;

                case Script:
                    this._scriptHash = ripemd160.ComputeHash( SHA256.Value.ComputeHash( data ) );
                    version = Scripthash;
                    break;

                case Pubkeyhash:
                    this._pubKeyHash = data;
                    break;

                case Scripthash:
                    this._scriptHash = data;
                    break;
            }
            this._type = version;
        }

        public Address(String address) {
            this._address = address;
        }

        public override Boolean Equals(Object obj) {
            if ( !( obj is Address ) ) {
                return false;
            }
            if ( ( this.EitherHash == null ) || ( ( ( Address )obj ).EitherHash == null ) ) {
                return false;
            }
            return this.EitherHash.HashBytes.SequenceEqual( ( ( Address )obj ).EitherHash.HashBytes );
        }

        public override Int32 GetHashCode() => this.EitherHash.GetHashCode();

        public override String ToString() {
            if ( this._address == null ) {
                this.CalcBase58();
            }
            return this._address;
        }

        private void CalcBase58() {
            if ( this._pubKeyHash != null ) {
                this._address = Base58CheckString.FromByteArray( this._pubKeyHash, Pubkeyhash );
            }
            else if ( this._scriptHash != null ) {
                this._address = Base58CheckString.FromByteArray( this._scriptHash, Scripthash );
            }
            else {
                throw new InvalidOperationException( "Address is not a public key or script hash!" );
            }
        }

        private Byte CalcHash() {
            Byte version;
            var hash = Base58CheckString.ToByteArray( this.ToString(), out version );
            switch ( version ) {
                case Pubkeyhash:
                    this._pubKeyHash = hash;
                    break;

                case Scripthash:
                    this._scriptHash = hash;
                    break;
            }
            this._type = version;
            return version;
        }
    }
}