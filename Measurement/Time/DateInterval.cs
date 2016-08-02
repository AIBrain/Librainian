// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/DateInterval.cs" was last cleaned by Rick on 2016/07/26 at 3:29 PM

namespace Librainian.Measurement.Time {

    using System;

    /// <summary>
    ///     Used when calculating the difference between two <see cref="DateTime" /> instances
    ///     with the <see cref="DateSpan" /> class.
    /// </summary>
    /// <remarks>
    ///     Adapted from <see cref="http://github.com/danielcrenna/vault/blob/master/dates/src/Dates/DateInterval.cs" />
    /// </remarks>
    public enum DateInterval {

        /// <summary>
        ///     Years
        /// </summary>
        Years,

        /// <summary>
        ///     Months
        /// </summary>
        Months,

        /// <summary>
        ///     Weeks
        /// </summary>
        Weeks,

        /// <summary>
        ///     Days
        /// </summary>
        Days,

        /// <summary>
        ///     Hours
        /// </summary>
        Hours,

        /// <summary>
        ///     Minutes
        /// </summary>
        Minutes,

        /// <summary>
        ///     Seconds
        /// </summary>
        Seconds
    }
}