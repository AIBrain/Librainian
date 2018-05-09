// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/IDenomination.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Diagnostics;

    namespace Denominations {

        [DebuggerDisplay( "{" + nameof( Formatted ) + ",nq}" )]
        public struct Btc : ICoin {

            public Decimal FaceValue => 1.00M;

            public String Formatted => $"฿{this.FaceValue:f8}";
        }

        [DebuggerDisplay( "{" + nameof( Formatted ) + ",nq}" )]
        public struct MBtc : ICoin {

            public Decimal FaceValue => 0.001M;

            public String Formatted => $"฿{this.FaceValue:f8}";
        }

        [DebuggerDisplay( "{" + nameof( Formatted ) + ",nq}" )]
        public struct Satoshi : ICoin {

            public Decimal FaceValue => 0.00000001M;

            public String Formatted => $"฿{this.FaceValue:f8}";
        }

        [DebuggerDisplay( "{" + nameof( Formatted ) + ",nq}" )]
        public struct ΜBtc : ICoin {

            public Decimal FaceValue => 0.000001M;

            public String Formatted => $"฿{this.FaceValue:f8}";
        }
    }

    public interface IDenomination {

        Decimal FaceValue {
            get;
        }

        String Formatted {
            get;
        }
    }
}