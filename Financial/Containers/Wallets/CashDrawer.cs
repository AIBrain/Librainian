// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CashDrawer.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Containers.Wallets {

    using System;
    using System.Linq;
    using Currency.BankNotes;
    using Currency.Coins;

    public class CashDrawer : Wallet {

        public CashDrawer( Guid id ) : base( id ) {
        }

        public Boolean Fund( params IBankNote[] bankNotes ) {
            if ( null == bankNotes ) {
                throw new ArgumentNullException( nameof( bankNotes ) );
            }

            return 0 == bankNotes.LongCount( bankNote => !this.Deposit( bankNote, 1 ) );
        }

        public Boolean Fund( params ICoin[] coins ) {
            if ( null == coins ) {
                throw new ArgumentNullException( nameof( coins ) );
            }

            return 0 == coins.LongCount( coin => this.Deposit( coin, 1 ) != 1 );
        }

        public Decimal RunningTotal() => this.Total();

    }
}