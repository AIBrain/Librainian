// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "TranslateGuidUInt64.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "TranslateGuidUInt64.cs" was last formatted by Protiguous on 2020/03/16 at 4:38 PM.

namespace Librainian.Converters {

    using System;
    using System.Runtime.InteropServices;

    [StructLayout( layoutKind: LayoutKind.Explicit, Pack = 0 )]
    public struct TranslateGuidUInt64 {

        [FieldOffset( offset: 0 )] // bytes 0..15 == 16 bytes
        public Guid guid;

        [FieldOffset( offset: 0 )]
        public readonly UInt64 Low;

        [FieldOffset( offset: sizeof( UInt64 ) )]
        public readonly UInt64 High;

        public TranslateGuidUInt64( UInt64 high, UInt64 low ) {
            this.guid = Guid.Empty;
            this.Low = low;
            this.High = high;
        }
    }
}