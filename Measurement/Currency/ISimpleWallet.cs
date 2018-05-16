// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ISimpleWallet.cs",
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
// "Librainian/Librainian/ISimpleWallet.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Currency {

    using System;
    using System.Windows.Forms;
    using JetBrains.Annotations;

    public interface ISimpleWallet {

        Decimal Balance { get; }

        [CanBeNull]
        Label LabelToFlashOnChanges { get; set; }

        [CanBeNull]
        Action<Decimal> OnAfterDeposit { get; set; }

        [CanBeNull]
        Action<Decimal> OnAfterWithdraw { get; set; }

        [CanBeNull]
        Action<Decimal> OnAnyUpdate { get; set; }

        [CanBeNull]
        Action<Decimal> OnBeforeDeposit { get; set; }

        [CanBeNull]
        Action<Decimal> OnBeforeWithdraw { get; set; }

        /// <summary>Add any (+-)amount directly to the balance.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryAdd( Decimal amount, Boolean sanitize = true );

        Boolean TryAdd( [NotNull] SimpleWallet wallet, Boolean sanitize = true );

        /// <summary>Attempt to deposit amoount (larger than zero) to the <see cref="SimpleWallet.Balance" />.</summary>
        /// <param name="amount"></param>
        /// <param name="sanitize"></param>
        /// <returns></returns>
        Boolean TryDeposit( Decimal amount, Boolean sanitize = true );

        Boolean TryTransfer( Decimal amount, ref SimpleWallet intoWallet, Boolean sanitize = true );

        Boolean TryUpdateBalance( Decimal amount, Boolean sanitize = true );

        void TryUpdateBalance( SimpleWallet simpleWallet );

        Boolean TryWithdraw( Decimal amount, Boolean sanitize = true );

        Boolean TryWithdraw( [NotNull] SimpleWallet wallet );
    }
}