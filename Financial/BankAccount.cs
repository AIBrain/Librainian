namespace Librainian.Financial {
    using System;

    public abstract class BankAccount  : Wallet, IBankAccount {
        protected BankAccount( Guid id ) : base( id ) { }
    }
}
