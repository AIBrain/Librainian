// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/IDenomination.cs" was last cleaned by Rick on 2015/06/12 at 2:54 PM

namespace Librainian.Financial {

    using System;

    namespace Denominations.USD {

        using System.Diagnostics;

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Dime : ICoin {

            public Decimal FaceValue => 0.10M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Fifty : IBankNote {

            public Decimal FaceValue => 50.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Five : IBankNote {

            public Decimal FaceValue => 5.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Hundred : IBankNote {

            public Decimal FaceValue => 100.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Nickel : ICoin {

            public Decimal FaceValue => 0.05M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class One : IBankNote {

            public Decimal FaceValue => 1.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Penny : ICoin {

            public Decimal FaceValue => 0.01M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Quarter : ICoin {

            public Decimal FaceValue => 0.25M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Ten : IBankNote {

            public Decimal FaceValue => 10.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        public sealed class Twenty : IBankNote {

            public Decimal FaceValue => 20.00M;

            public String Formatted => $"{this.FaceValue:C}";
        }

        ///// <summary>
        ///// This bill exists, but it is rarely found and therefore not calculated.
        ///// </summary>
        //[DebuggerDisplay( "{Formatted,nq}" )]
        //
        //public class Two : IBankNote {
        //    public  Decimal FaceValue => 2.00M;
        //    public String Formatted => $"{this.FaceValue:C}";
        //}
    }

    /// <summary></summary>
    /// <see cref="http://www.treasury.gov/resource-center/faqs/Currency/Pages/denominations.aspx" />
    /// <see cref="http://wikipedia.org/wiki/Banknote" />
    public interface IBankNote : IDenomination {
    }

    /// <summary></summary>
    /// <see cref="http://www.treasury.gov/resource-center/faqs/Currency/Pages/denominations.aspx" />
    /// <see cref="http://wikipedia.org/wiki/Coin" />
    public interface ICoin : IDenomination {
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