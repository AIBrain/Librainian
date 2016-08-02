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
// "Librainian/AutoNumber.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

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

        /// <summary>Initialize the Identity with the specified seed value.</summary>
        /// <param name="seed"></param>
        public AutoNumber( UInt64 seed = UInt64.MinValue ) {
            this.Reseed( seed );
        }

        /// <summary>The current value of the AutoNumber</summary>
        public UInt64 Identity => ( UInt64 )Interlocked.Read( ref this._identity );

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