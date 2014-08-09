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
// "Librainian/AMorPM.cs" was last cleaned by Rick on 2014/08/09 at 2:15 PM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    /// <summary>
    ///     from the Latin: ante meridiem meaning "before midday", or post meridiem "after midday"
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/12-hour_clock" />
    public enum AMorPM {
        /// <summary>
        ///     from the Latin ante meridiem, meaning "before midday"
        /// </summary>
        AM,

        /// <summary>
        ///     from the Latin post meridiem, meaning "after midday"
        /// </summary>
        PM
    }
}
