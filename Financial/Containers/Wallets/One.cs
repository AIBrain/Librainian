namespace Librainian.Financial.Wallets.Librainian.Financial.Coins {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class One : IBankNote {

        public Decimal FaceValue => 1.00M;

        public override String ToString() => $"{this.FaceValue:C}";

    }

}