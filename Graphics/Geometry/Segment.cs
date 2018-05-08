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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Segment.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Geometry {

    using System;
    using System.Diagnostics;
    using DDD;
    using Extensions;
    using Newtonsoft.Json;

    [Immutable]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( MemberSerialization.Fields )]
    public class Segment {
        public static Segment Empty;
        public readonly CoordinateF P1;
        public readonly CoordinateF P2;

        public Segment( CoordinateF p1, CoordinateF p2 ) {
            this.P1 = p1;
            this.P2 = p2;
        }

        public Single X1() => this.P1.X;

        public Single X2() => this.P2.X;

        public Single Y1() => this.P1.Y;

        public Single Y2() => this.P2.Y;

        public Single Z1() => this.P1.Z;

        public Single Z2() => this.P2.Z;
    }
}