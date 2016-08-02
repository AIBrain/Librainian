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
// "Librainian/Epochs.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;

    public static class Epochs {

        /// <summary></summary>
        /// <seealso cref="http://wikipedia.org/wiki/Timeline_of_the_Big_Bang" />
        public static readonly WhenRange Before1PlanckTime = new WhenRange( UniversalDateTime.TheBeginning, UniversalDateTime.One );

        /// <summary>1927</summary>
        public static readonly DateTime BigBangModelFormulated = new DateTime( year: 1927, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc );

        /// <summary>January 1st, 1970, zero seconds.</summary>
        public static readonly DateTime Unix = new DateTime( year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc );
    }
}