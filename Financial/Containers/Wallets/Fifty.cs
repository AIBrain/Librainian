namespace Librainian.Financial.Wallets.Librainian.Financial.Coins {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class Fifty : IBankNote {

        public Decimal FaceValue => 50.00M;

        public override String ToString() => $"{this.FaceValue:C}";

    }

}