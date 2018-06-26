namespace Librainian.Financial.Wallets.Librainian.Financial.Coins {

    using System;
    using System.Diagnostics;

    [DebuggerDisplay( "{ToString(),nq}" )]
    public sealed class Quarter : ICoin {

        public Decimal FaceValue => 0.25M;

        public override String ToString() => $"{this.FaceValue:C}";

    }

}