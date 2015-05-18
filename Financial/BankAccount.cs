namespace Librainian.Financial {
    using System;
    using Measurement.Currency.USD;

    public abstract class BankAccount  : Wallet, IBankAccount {
        protected BankAccount( Guid id ) : base( id ) { }
    }
}
