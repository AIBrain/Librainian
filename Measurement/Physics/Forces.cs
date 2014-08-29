namespace Librainian.Measurement.Physics {
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Fundamental_interaction"/>
    [Flags]
    public enum Forces {

        /// <summary>
        ///
        /// </summary>
        /// <seealso cref="http://wikipedia.org/wiki/Gravitation"/>
        Gravitation = 0x1,

        /// <summary>
        /// http: //wikipedia.org/wiki/Electromagnetic_force
        /// </summary>
        ElectromagneticForce = 0x2,

        /// <summary>
        /// http: //wikipedia.org/wiki/Strong_interaction
        /// </summary>
        StrongInteraction = 0x4,

        /// <summary>
        /// http: //wikipedia.org/wiki/Weak_interaction
        /// </summary>
        WeakInteraction = 0x8
    }
}