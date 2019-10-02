// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Status.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "Status.cs" was last formatted by Protiguous on 2019/10/02 at 9:46 AM.

namespace Librainian {

    using System;
    using System.ComponentModel;
    using Extensions;
    using JetBrains.Annotations;
    using Parsing;

    /// <summary>
    ///     Returns:
    ///     <para>-3 = <see cref="Exception" /></para>
    ///     <para>-2 = <see cref="Error" /></para>
    ///     <para>
    ///         -1 = <see cref="Failure" />, <see cref="No" />, <see cref="NoGo" />, <see cref="Stop" />, <see cref="Halt" />
    ///         , <see cref="Negative" />.
    ///     </para>
    ///     <para>0 = <see cref="Unknown" /></para>
    ///     <para>
    ///         1 = <see cref="Success" />, <see cref="Go" />, <see cref="Yes" />, <see cref="Proceed" />,
    ///         <see cref="Continue" />, <see cref="Advance" />, <see cref="Positive" />.
    ///     </para>
    /// </summary>
    [Flags]
    public enum Status : SByte {

        [Description( Symbols.Exception )]
        Exception = Error - 1,

        [Description( Symbols.Error )]
        Error = Failure - 1,

        [Description( Symbols.Fail )]
        Failure = -1,

        [Description( Symbols.Fail )]
        No = Failure,

        [Description( Symbols.Fail )]
        NoGo = Failure,

        [Description( Symbols.Fail )]
        Stop = Failure,

        [Description( Symbols.Fail )]
        Halt = Failure,

        [Description( Symbols.Fail )]
        Negative = Failure,

        [Description( Symbols.Unknown )]
        Unknown = 0,

        [Description( Symbols.CheckMark )]
        Success = 1,

        [Description( Symbols.CheckMark )]
        Go = Success,

        [Description( Symbols.CheckMark )]
        Yes = Success,

        [Description( Symbols.CheckMark )]
        Proceed = Success,

        [Description( Symbols.CheckMark )]
        Continue = Success,

        [Description( Symbols.CheckMark )]
        Advance = Success,

        [Description( Symbols.CheckMark )]
        Positive = Success

    }

    public static class StatusExtensions {

        public static Boolean Failed( this Status status ) => status <= Status.Failure;

        public static Boolean IsBad( this Status status ) => status <= Status.Failure;

        public static Boolean IsGood( this Status status ) => status >= Status.Success;

        public static Boolean IsUnknown( this Status status ) => status == Status.Unknown || status != Status.Success && status != Status.Failure;

        public static Boolean Succeeded( this Status status ) => status >= Status.Success;

        [NotNull]
        public static String Symbol( this Status status ) => status.GetDescription() ?? Symbols.Null;

    }

}