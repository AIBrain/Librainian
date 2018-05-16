// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "CardinalDirection.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/CardinalDirection.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Spatial {

    using System;

    public enum CardinalDirection : UInt32 {

        /// <summary>0 (is equal to 360)</summary>
        North = 0,

        /// <summary>22</summary>
        NorthNorthEast = ( North + NorthEast ) / 2,

        NorthEast = ( North + East ) / 2,

        EastNorthEast = ( East + NorthEast ) / 2,

        East = 90,

        EastSouthEast = ( East + SouthEast ) / 2,

        SouthEast = ( South + East ) / 2,

        SouthSouthEast = ( South + SouthEast ) / 2,

        South = 180,

        SouthSouthWest = ( South + SouthWest ) / 2,

        SouthWest = ( South + West ) / 2,

        WestSouthWest = ( West + SouthWest ) / 2,

        West = 270,

        WestNorthWest = ( West + NorthWest ) / 2,

        NorthWest = ( FullNorth + West ) / 2,

        NorthNorthWest = ( FullNorth + NorthWest ) / 2,

        /// <summary>360</summary>
        FullNorth = 360
    }
}