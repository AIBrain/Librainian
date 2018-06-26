namespace Librainian.Financial.Wallets.Librainian.Financial.Coins {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class Hundred : IBankNote {

        public Decimal FaceValue => 100.00M;

        public override String ToString() => $"{this.FaceValue:C}";

    }

}