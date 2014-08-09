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
// "Librainian/WhenRange.cs" was last cleaned by Rick on 2014/08/09 at 2:16 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents a <see cref="UniversalDateTime" /> range with minimum and maximum values.
    /// </summary>
    [DataContract( IsReference = true )]
    [Serializable]
    public struct WhenRange {
        /// <summary>
        ///     Length of the range (difference between maximum and minimum values).
        /// </summary>
        [DataMember] [OptionalField] public readonly Span Length;

        /// <summary>
        ///     Maximum value
        /// </summary>
        [DataMember] [OptionalField] public readonly UniversalDateTime Max;

        /// <summary>
        ///     Minimum value
        /// </summary>
        [DataMember] [OptionalField] public readonly UniversalDateTime Min;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WhenRange" /> class
        /// </summary>
        /// <param name="min"> Minimum value of the range </param>
        /// <param name="max"> Maximum value of the range </param>
        public WhenRange( UniversalDateTime min, UniversalDateTime max ) {
            if ( min < max ) {
                this.Min = min;
                this.Max = max;
            }
            else {
                this.Min = max;
                this.Max = min;
            }
            var δ = this.Max.Value - this.Min.Value;
            this.Length = new Span( planckTimes: δ );
        }

        ///// <summary>
        /////     Check if the specified range is inside this range
        ///// </summary>
        ///// <param name="range"> Range to check </param>
        ///// <returns>
        /////     <b>True</b> if the specified range is inside this range or <b>false</b> otherwise.
        ///// </returns>
        //public Boolean IsInside( WhenRange range ) {
        //    return this.IsInside( range.Min ) && this.IsInside( range.Max );
        //}

        ///// <summary>
        /////     Check if the specified value is inside this range
        ///// </summary>
        ///// <param name="x"> Value to check </param>
        ///// <returns>
        /////     <b>True</b> if the specified value is inside this range or <b>false</b> otherwise.
        ///// </returns>
        //public Boolean IsInside( When x ) {
        //    if ( this.Min > x ) {
        //        return false;
        //    }
        //    return x <= this.Max;
        //}

        ///// <summary>
        /////     Check if the specified range overlaps with this range
        ///// </summary>
        ///// <param name="range"> Range to check for overlapping </param>
        ///// <returns>
        /////     <b>True</b> if the specified range overlaps with this range or <b>false</b> otherwise.
        ///// </returns>
        //public Boolean IsOverlapping( WhenRange range ) {
        //    return this.IsInside( range.Min ) || this.IsInside( range.Max );
        //}

        //public IEnumerable<Milliseconds> Milliseconds() {
        //    return this.Min.TotalMilliseconds.To( this.Max.TotalMilliseconds )
        //               .Select( millsecond => Time.Milliseconds.One );
        //}

        //public IEnumerable<Minutes> Minutes() {
        //    return this.Min.TotalMinutes.To( this.Max.TotalMinutes )
        //               .Select( minutes => Time.Minutes.One );
        //}

        //public TimeSpan Random() {
        //    return Randem.NextTimeSpan( this.Min, this.Max );
        //}

        //public IEnumerable<Seconds> Seconds() {
        //    return this.Min.TotalSeconds.To( this.Max.TotalSeconds )
        //               .Select( second => Time.Seconds.One );
        //}
    }
}
