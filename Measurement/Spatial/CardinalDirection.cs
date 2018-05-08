// Copyright 2018 Protiguous.
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
// "Librainian/CardinalDirection.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Spatial {

    public enum CardinalDirection : System.UInt32 {

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