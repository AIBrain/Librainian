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
// "Librainian/Customer.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.Financial.Customers {

    using System;
    using Containers.Accounts;
    using Containers.Wallets;
    using Magic;
    using Newtonsoft.Json;

    [JsonObject]
    public class Customer : ABetterClassDispose,ICustomer {

        public Customer( Guid customerID ) {
            this.Wallet = new Wallet( customerID );
            this.CheckingAccount = new CheckingAccount( customerID );
            this.SavingsAccount = new SavingsAccount( customerID );
        }

        [JsonProperty]
        public CheckingAccount CheckingAccount {
            get;
        }

        [JsonProperty]
        public SavingsAccount SavingsAccount {
            get;
        }

        [JsonProperty]
        public Wallet Wallet {
            get;
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this.Wallet.Dispose();
            this.CheckingAccount.Dispose();
            this.SavingsAccount.Dispose();
        }

    }
}