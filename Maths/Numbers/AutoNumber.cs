// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AutoNumber.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/AutoNumber.cs" was last formatted by Protiguous on 2018/05/24 at 7:21 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Threading;
    using Newtonsoft.Json;

    /// <summary>
    ///     An automatically incrementing Identity class. ( <see cref="Identity" /> is
    ///     <see cref="ulong" /> )
    /// </summary>
    [JsonObject]
    public sealed class AutoNumber {

        [JsonProperty]
        private Int64 _identity;

        /// <summary>The current value of the AutoNumber</summary>
        public UInt64 Identity => ( UInt64 )Interlocked.Read( ref this._identity );

        /// <summary>Initialize the Identity with the specified seed value.</summary>
        /// <param name="seed"></param>
        public AutoNumber( UInt64 seed = UInt64.MinValue ) => this.Reseed( seed );

        public void Ensure( UInt64 atLeast ) {
            if ( this.Identity < atLeast ) {

                //TODO make this an atomic operation
                this.Reseed( atLeast );
            }
        }

        /// <summary>Returns the incremented Identity</summary>
        /// <returns></returns>
        public UInt64 Next() => ( UInt64 )Interlocked.Increment( ref this._identity );

        /// <summary>Resets the Identity to the specified seed value</summary>
        /// <param name="newIdentity"></param>
        public void Reseed( UInt64 newIdentity ) => Interlocked.Exchange( ref this._identity, ( Int64 )newIdentity );

        public override String ToString() => $"{this.Identity}";
    }
}