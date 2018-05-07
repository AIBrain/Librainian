// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Era.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    /// <summary>
    ///     <para>Represents an Era for a HistoricalDate.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Anno_Domini" />
    public enum Era {

        /// <summary>
        ///     <para>Before Christ (BC or B.C.)</para>
        /// </summary>
        Bc = 0,

        /// <summary>
        ///     Before common era.
        /// </summary>
        Bce = Bc,

        /// <summary>
        ///     <para>Anno Domini (AD or A.D.)</para>
        ///     <para>"In the year of our Lord"</para>
        /// </summary>
        Ad
    }
}