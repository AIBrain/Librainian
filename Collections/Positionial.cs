// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Positionial.cs" was last cleaned by Protiguous on 2018/05/12 at 1:19 AM

namespace Librainian.Collections {

    using System;
    using System.Runtime.Serialization;

    public enum Positionial : Byte {

        [EnumMember( Value = nameof( Lowest ) )]
        Lowest,

        /// <summary>
        /// Randomly between <see cref="Lowest"/> and <see cref="Middle"/>.
        /// </summary>
        [EnumMember( Value = nameof( Lowish ) )]
        Lowish,

        //[EnumMember( Value = nameof( Lower ) )]
        //Lower,

        //[EnumMember( Value = nameof( Low ) )]
        //Low,

        /// <summary>
        /// smack dab in the middle.
        /// </summary>
        [EnumMember( Value = nameof( Middle ) )]
        Middle,

        ///// <summary>Halfway between high and avg</summary>
        //[EnumMember( Value = nameof( High ) )]
        //High,

        ///// <summary>Slightly higher than high.</summary>
        //[EnumMember( Value = nameof( Higher ) )]
        //Higher,

        /// <summary>
        /// Randomly between <see cref="Middle"/> and <see cref="Highest"/>.
        /// </summary>
        [EnumMember( Value = nameof( Highish ) )]
        Highish,

        /// <summary>
        /// Highest priority.
        /// </summary>
        [EnumMember( Value = nameof( Highest ) )]
        Highest
    }
}