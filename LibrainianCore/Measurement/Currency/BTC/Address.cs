// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Address.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Address.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using JetBrains.Annotations;
    using Parsing;

    /// <summary></summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    public class Address {

        [CanBeNull]
        private String _address;

        [CanBeNull]
        private Hash _pubKeyHash;

        private Hash _scriptHash;

        private Byte? _type;

        public static ThreadLocal<SHA256> SHA256 { get; } = new ThreadLocal<SHA256>( valueFactory: () => new SHA256Managed() );

        [NotNull]
        public Hash EitherHash {
            get {
                if ( this._pubKeyHash is null && this._scriptHash is null ) {
                    this.CalcHash();
                }

                if ( this._pubKeyHash != null ) {
                    return this._pubKeyHash;
                }

                return this._scriptHash;
            }
        }

        [CanBeNull]
        public Hash PubKeyHash {
            get {
                if ( this._pubKeyHash is null && this.CalcHash() != Pubkeyhash ) {
                    throw new InvalidOperationException( message: "Address is not a public key hash." );
                }

                return this._pubKeyHash;
            }
        }

        [CanBeNull]
        public Hash ScriptHash {
            get {
                if ( this._pubKeyHash is null && this.CalcHash() != Scripthash ) {
                    throw new InvalidOperationException( message: "Address is not a script hash." );
                }

                return this._scriptHash;
            }
        }

        public Byte Type {
            get {
                if ( this._type is null ) {
                    this.CalcHash();
                }

                if ( this._type is null ) {
                    throw new InvalidOperationException();
                }

                return this._type.Value;
            }
        }

        public const Byte Pubkey = 0xFE;

        public const Byte Pubkeyhash = 0x00;

        public const Byte Script = 0xFF;

        public const Byte Scripthash = 0x05;

        public Address( [CanBeNull] String address ) => this._address = address;

        private void CalcBase58() {
            if ( this._pubKeyHash != null ) {
                this._address = Base58CheckString.FromByteArray( b: this._pubKeyHash, version: Pubkeyhash );
            }
            else if ( this._scriptHash != null ) {
                this._address = Base58CheckString.FromByteArray( b: this._scriptHash, version: Scripthash );
            }
            else {
                throw new InvalidOperationException( message: "Address is not a public key or script hash!" );
            }
        }

        private Byte CalcHash() {
            var hash = Base58CheckString.ToByteArray( s: this.ToString(), version: out var version );

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
            if ( !( obj is Address ) ) {
                return default;
            }

            return this.EitherHash.HashBytes.SequenceEqual( second: ( ( Address ) obj ).EitherHash.HashBytes );
        }

        public override Int32 GetHashCode() => this.EitherHash.GetHashCode();

        [NotNull]
        public override String ToString() {
            if ( this._address is null ) {
                this.CalcBase58();
            }

            return this._address ?? throw new InvalidOperationException();
        }

    }

}