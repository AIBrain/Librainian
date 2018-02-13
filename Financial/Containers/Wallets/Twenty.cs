namespace Librainian.Financial.Wallets.Librainian.Financial.Coins {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class Twenty : IBankNote {

        public Decimal FaceValue => 20.00M;

        public override String ToString() => $"{this.FaceValue:C}";

    }

}