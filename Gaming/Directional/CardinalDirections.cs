namespace Librainian.Gaming.Directional {
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
        FullNorth = 360,
    }
}