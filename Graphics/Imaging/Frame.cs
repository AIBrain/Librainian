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
// "Librainian/Frame.cs" was last cleaned by Rick on 2014/11/17 at 3:34 PM

#endregion License & Information

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class Frame {

        /// <summary>
        ///     Checksum of the page (guard against corruption).
        /// </summary>
        /// <remarks>Should include the <see cref="Count" /> and <see cref="Delay" /> to prevent buffer overflows and timeouts.</remarks>
        [DataMember]
        [FieldOffset( 0 )]
        public UInt64 Checksum;

        /// <summary>
        ///     How many lines should be in this page?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) )]
        public UInt64 Count;

        /// <summary>
        ///     How many milliseconds to display this page?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 2 )]
        public UInt64 Delay;

        /// <summary>
        ///     An array of <see cref="Line" />.
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 3 )]
        public Line[] Lines;
    }
}