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
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using Annotations;

    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class Frame : IEquatable<Frame> {

        public static readonly String DefaultHeader = "EFGFrame";

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 0 )]
        public UInt64 Identity;

        /// <summary>
        ///     Checksum of the page (guard against corruption).
        /// </summary>
        /// <remarks>Should include the <see cref="Count" /> and <see cref="Delay" /> to prevent buffer overflows and timeouts.</remarks>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 1 )]
        public UInt64 Checksum;

        /// <summary>
        ///     How many lines should be in this frame?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 2 )]
        public UInt64 LineCount;

        /// <summary>
        ///     How many milliseconds to display this frame?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 3 )]
        public UInt64 Delay;

        /// <summary>
        ///     An array of <see cref="Line" />.
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 4 )]
        public Line[] Lines;

        /// <summary>
        /// static comparision
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        public Boolean Equal( Frame left, Frame right ) {
            if ( left == null || right == null ) {
                return false;
            }

            if ( left.Checksum != right.Checksum ) {
                return false;
            }
            if ( left.LineCount != right.LineCount ) {
                return false;
            }
            if ( left.Lines.LongLength != right.Lines.LongLength ) {
                return false;
            }
            return left.Lines.SequenceEqual( right.Lines );
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals( Frame other ) => Equal( this, other );
    }
}