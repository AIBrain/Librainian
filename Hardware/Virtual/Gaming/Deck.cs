﻿#region License & Information
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
// "Librainian/Deck.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Hardware.Virtual.Gaming {
    using System;
    using Annotations;
    using Collections;

    public class Deck {
        /// <summary>
        ///     list of cards contained in this deck.
        /// </summary>
        [NotNull] private readonly ParallelList< Card > _cards = new ParallelList< Card >();

        public void AddCard( [NotNull] Card card ) {
            if ( card == null ) {
                throw new ArgumentNullException( "card" );
            }
            this._cards.Add( card );
        }
    }
}