// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Address.cs",
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
// "Librainian/Librainian/Address.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using Parsing;

    /// <summary>
    /// </summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    public class Address {

        private String _address;
        private Hash _pubKeyHash;
        private Hash _scriptHash;
        private Byte? _type;
        public const Byte Pubkey = 0xFE;

        public const Byte Pubkeyhash = 0x00;

        public const Byte Script = 0xFF;

        public const Byte Scripthash = 0x05;

        public static ThreadLocal<SHA256> SHA256 { get; } = new ThreadLocal<SHA256>( () => new SHA256Managed() );

        public Hash EitherHash {
            get {
                if ( this._pubKeyHash is null && this._scriptHash is null ) { this.CalcHash(); }

                if ( this._pubKeyHash != null ) { return this._pubKeyHash; }

                return this._scriptHash;
            }
        }

        public Hash PubKeyHash {
            get {
                if ( this._pubKeyHash is null && this.CalcHash() != Pubkeyhash ) { throw new InvalidOperationException( "Address is not a public key hash." ); }

                return this._pubKeyHash;
            }
        }

        public Hash ScriptHash {
            get {
                if ( this._pubKeyHash is null && this.CalcHash() != Scripthash ) { throw new InvalidOperationException( "Address is not a script hash." ); }

                return this._scriptHash;
            }
        }

        public Byte Type {
            get {
                if ( this._type is null ) { this.CalcHash(); }

                if ( this._type is null ) { throw new InvalidOperationException(); }

                return this._type.Value;
            }
        }

        public Address( Byte[] data, Byte version = Pubkey ) {
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

        public Address( String address ) => this._address = address;

        private void CalcBase58() {
            if ( this._pubKeyHash != null ) { this._address = Base58CheckString.FromByteArray( this._pubKeyHash, Pubkeyhash ); }
            else if ( this._scriptHash != null ) { this._address = Base58CheckString.FromByteArray( this._scriptHash, Scripthash ); }
            else { throw new InvalidOperationException( "Address is not a public key or script hash!" ); }
        }

        private Byte CalcHash() {
            var hash = Base58CheckString.ToByteArray( this.ToString(), out var version );

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

        public override Boolean Equals( Object obj ) {
            if ( !( obj is Address ) ) { return false; }

            if ( this.EitherHash is null || ( ( Address )obj ).EitherHash is null ) { return false; }

            return this.EitherHash.HashBytes.SequenceEqual( ( ( Address )obj ).EitherHash.HashBytes );
        }

        public override Int32 GetHashCode() => this.EitherHash.GetHashCode();

        public override String ToString() {
            if ( this._address is null ) { this.CalcBase58(); }

            return this._address;
        }
    }
}