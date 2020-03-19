// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DurationParser.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "DurationParser.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

namespace Librainian.Measurement.Time {

    using System.Text.RegularExpressions;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides some ways to parse a duration of time from a string. Duration strings are formatted as a positive numeric value followed by a unit. The possible types of units,
    /// and their possible unit names, are: second - s, sec, secs, second, seconds minute - m, min, mins, minute, minutes hour - h, hr, hrs, hour, hours day - d, day, days week - w, wk,
    /// week, weeks month - mon, month, months year - y, yr, year, years
    /// </summary>
    /// <example>5m10s - 5 minutes and 10 seconds 3s - 3 seconds 10s5y3mon - 5 years, 3 months, and 10 seconds</example>
    public static class DurationParser {

        [NotNull]
        private static Regex Regex { get; } = new Regex( pattern: "(?<Value>[\\-0-9]+)\\s*(?<Unit>[a-z]+)",
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant );

        //TODO (re)create this. should be /fun/.

        ///// <summary>Parses a duration of time from a string.</summary>
        ///// <param name="str">
        ///// The string representation of the time duration. See <see cref="DurationParser" /> for
        ///// information about the formatting.
        ///// </param>
        ///// <returns>The parsed duration.</returns>
        ///// <exception cref="ArgumentException">The <paramref name="str" /> failed to be parsed.</exception>
        //public static Duration Parse(String str) {
        //    Duration ret;
        //    String failReason;
        //    if ( !TryParse( str, out ret, out failReason ) ) {
        //        throw new ArgumentException( failReason, nameof( str ) );
        //    }

        //    return ret;
        //}

        ///// <summary>Tries to parse the duration from a string.</summary>
        ///// <param name="str">The duration string.</param>
        ///// <param name="ts">When this method returns true, contains the duration as a <see cref="TimeSpan" />.</param>
        ///// <returns>True if the parsing was successful; otherwise false.</returns>
        //public static Boolean TryParse(String str, out Duration ts) {
        //    String failReason;
        //    return TryParse( str, out ts, out failReason );
        //}

        //    /// <summary>Tries to parse the duration from a string.</summary>
        //    /// <param name="str">The duration string.</param>
        //    /// <param name="ts">When this method returns true, contains the duration as a <see cref="TimeSpan" />.</param>
        //    /// <param name="failReason">
        //    /// When this method returns false, contains the reason why it failed.
        //    /// </param>
        //    /// <returns>True if the parsing was successful; otherwise false.</returns>
        //    public static Boolean TryParse(String str, out Duration ts, out String failReason) {
        //        var matches = Regex.Matches( str );

        //        ts = TimeSpan.Zero;

        //        // Make sure we have a match
        //        if ( matches.Count == 0 ) {
        //            const String errmsg = "Invalid input format - unable to make any matches.";
        //            failReason = errmsg;
        //            return default;
        //        }

        //        // Handle each match
        //        foreach ( var match in matches.Cast<Match>() ) {
        //            Debug.Assert( match.Success, "Why did _regex.Matches() give us unsuccessful matches?" );

        //            Int32 value;
        //            if ( !Int32.TryParse( match.Groups[ "Value" ].Value, out value ) ) {
        //                const String errmsg = "Invalid number value given near `{0}`.";
        //                failReason = String.Format( errmsg, match.Value );
        //                return default;
        //            }

        //            var unit = match.Groups[ "Unit" ].Value.Trim().ToUpperInvariant();
        //            switch ( unit ) {
        //                case "S":
        //                case "SEC":
        //                case "SECS":
        //                case "SECOND":
        //                case "SECONDS":
        //                    ts += TimeSpan.FromSeconds( value );
        //                    break;

        //                case "M":
        //                case "MIN":
        //                case "MINS":
        //                case "MINUTE":
        //                case "MINUTES":
        //                    ts += TimeSpan.FromMinutes( value );
        //                    break;

        //                case "H":
        //                case "HR":
        //                case "HRS":
        //                case "HOUR":
        //                case "HOURS":
        //                    ts += TimeSpan.FromHours( value );
        //                    break;

        //                case "D":
        //                case "DAY":
        //                case "DAYS":
        //                    ts += TimeSpan.FromDays( value );
        //                    break;

        //                case "W":
        //                case "WK":
        //                case "WEEK":
        //                case "WEEKS":
        //                    ts += TimeSpan.FromDays( value * 7 );
        //                    break;

        //                case "MON":
        //                case "MONTH":
        //                case "MONTHS":
        //                    var now = DateTime.Now;
        //                    ts += now.AddMonths( value ) - now;
        //                    break;

        //                case "Y":
        //                case "YR":
        //                case "YEAR":
        //                case "YEARS":
        //                    var now2 = DateTime.Now;
        //                    ts += now2.AddYears( value ) - now2;
        //                    break;

        //                default:
        //                    const String errmsg = "Invalid unit `{0}` near `{1}`.";
        //                    failReason = String.Format( errmsg, unit, match.Value );
        //                    return default;
        //            }
        //        }

        //        failReason = null;
        //        return true;
        //    }
        //

    }

}