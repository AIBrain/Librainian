namespace Librainian.Gaming.Directional {
    using System;
    using System.Runtime.Serialization;

    public enum Directions {
        North = 0,
        East = 90,
        South = 180,
        West = 270,
    }

    [DataContract(IsReference = true)]
    public class Compass {
        public Single Maximum = 360;

    }
}
