#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/IDenomination.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using Annotations;

    public interface IDenomination {
        [UsedImplicitly]
       Decimal FaceValue { get; }

        [UsedImplicitly]
        String Formatted { get; }
    }

    namespace Denominations {
        using System.Diagnostics;

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public struct BTC : ICoin {
            public  Decimal FaceValue { get { return 1.00M; } }
            public String Formatted { get { return String.Format( "฿{0:f8}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public struct mBTC : ICoin {
            public  Decimal FaceValue { get { return 0.001M; } }
            public String Formatted { get { return String.Format( "฿{0:f8}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public struct μBTC : ICoin {
            public  Decimal FaceValue { get { return 0.000001M; } }
            public String Formatted { get { return String.Format( "฿{0:f8}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public struct Satoshi : ICoin {
            public  Decimal FaceValue { get { return 0.00000001M; } }
            public String Formatted { get { return String.Format( "฿{0:f8}", this.FaceValue ); } }
        }
    }
}
