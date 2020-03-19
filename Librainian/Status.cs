// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Status.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Status.cs" was last formatted by Protiguous on 2020/03/18 at 10:31 AM.

namespace Librainian {

    using System;
    using System.ComponentModel;
    using Extensions;
    using JetBrains.Annotations;
    using Parsing;

    public enum Status {

        [Description( Symbols.SkullAndCrossbones )]
        Fatal = Exception - 1,

        [Description( Symbols.Exception )]
        Exception = Error - 1,

        [Description( Symbols.Error )]
        Error = Stop - 1,

        [Description( Symbols.Fail )]
        NoGo = Halt,

        [Description( Symbols.FailBig )]
        Stop = Halt,

        [Description( Symbols.Fail )]
        Halt = Skip - 1,

        [Description( Symbols.Fail )]
        Skip = Timeout - 1,

        [Description( Symbols.Fail )]
        Timeout = Failure - 1,

        [Description( Symbols.Fail )]
        Negative = Failure,

        [Description( Symbols.Fail )]
        Bad = Failure,

        [Description( Symbols.Fail )]
        No = Failure - 1,

        [Description( Symbols.Fail )]
        Failure = -1,

        [Description( Symbols.Unknown )]
        Unknown = 0,

        [Description( Symbols.WhiteStar )]
        Success = 1,

        [Description( Symbols.CheckMark )]
        Go = Success,

        [Description( Symbols.CheckMark )]
        Good = Success,

        [Description( Symbols.CheckMark )]
        Yes = Success + 1,

        [Description( Symbols.CheckMark )]
        Proceed = Success,

        [Description( Symbols.CheckMark )]
        Continue = Success,

        [Description( Symbols.CheckMark )]
        Advance = Success,

        [Description( Symbols.CheckMark )]
        Positive = Success + 1

    }

    public static class StatusExtensions {

        static StatusExtensions() {
            if ( Status.Good.IsBad() ) {
                throw new InvalidOperationException( "The universe messed up." );
            }

            if ( Status.Failure.IsGood() ) {
                throw new InvalidOperationException( "The universe messed up." );
            }

            if ( !Status.Unknown.IsUnknown() ) {
                throw new InvalidOperationException( "The universe messed up." );
            }
        }

        public static Boolean Failed( this Status status ) => status <= Status.Failure;

        public static Boolean IsBad( this Status status ) => status <= Status.Failure;

        public static Boolean IsGood( this Status status ) => status >= Status.Success;

        public static Boolean IsUnknown( this Status status ) => ( status == Status.Unknown ) || ( !status.IsBad() && !status.IsGood() );

        public static Boolean Succeeded( this Status status ) => status >= Status.Success;

        [NotNull]
        public static String Symbol( this Status status ) => status.GetDescription() ?? Symbols.Null;

    }

}