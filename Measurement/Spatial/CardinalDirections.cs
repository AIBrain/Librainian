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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CardinalDirections.cs" was last cleaned by Rick on 2014/09/29 at 1:41 PM

#endregion License & Information

namespace Librainian.Measurement.Spatial {

    public enum CardinalDirections {
        North = 0,
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

        /// <summary>
        /// 360
        /// </summary>
        FullNorth = 360,
    }
}