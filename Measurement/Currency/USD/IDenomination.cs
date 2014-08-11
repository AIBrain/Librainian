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

namespace Librainian.Measurement.Currency.USD {
    using System;
    using Annotations;

    public interface IDenomination {
        Decimal FaceValue { get; }

        [UsedImplicitly]
        String Formatted { get; }
    }

    /// <summary>
    /// </summary>
    /// <see cref="http://www.treasury.gov/resource-center/faqs/Currency/Pages/denominations.aspx" />
    /// <see cref="http://wikipedia.org/wiki/Banknote" />
    public interface IBankNote : IDenomination { }

    /// <summary>
    /// </summary>
    /// <see cref="http://www.treasury.gov/resource-center/faqs/Currency/Pages/denominations.aspx" />
    /// <see cref="http://wikipedia.org/wiki/Coin" />
    public interface ICoin : IDenomination { }

    namespace Denominations {
        using System.Diagnostics;

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Dime : ICoin {
            public Decimal FaceValue { get { return 0.10M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Fifty : IBankNote {
            public Decimal FaceValue { get { return 50.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Five : IBankNote {
            public Decimal FaceValue { get { return 5.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Hundred : IBankNote {
            public Decimal FaceValue { get { return 100.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Nickel : ICoin {
            public Decimal FaceValue { get { return 0.05M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public class One : IBankNote {
            public Decimal FaceValue { get { return 1.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Penny : ICoin {
            public Decimal FaceValue { get { return 0.01M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public sealed class Quarter : ICoin {
            public Decimal FaceValue { get { return 0.25M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public class Ten : IBankNote {
            public Decimal FaceValue { get { return 10.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public class Twenty : IBankNote {
            public Decimal FaceValue { get { return 20.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }

        /* This bill exists, but it is so rarely found and therefore not calculated.
        [DebuggerDisplay( "{Formatted,nq}" )]
        [UsedImplicitly]
        public class Two : IPaperBill {
            public Decimal FaceValue { get { return 2.00M; } }
            public String Formatted { get { return String.Format( "{0:C}", this.FaceValue ); } }
        }
         * */
    }
}
