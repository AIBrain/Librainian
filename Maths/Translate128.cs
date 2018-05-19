// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any
// source code used or derived from our binaries, libraries, projects, or solutions.
//
// This source code, "Translate128.cs", belongs to Rick@AIBrain.org
// and Protiguous@Protiguous.com unless otherwise specified or
// the original license has been overwritten by this automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects
// still retain their original license and our thanks goes to those Authors.
//
// Donations, royalties from any software that uses any of our code,
// and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Translate128.cs" was last formatted by Protiguous on 2018/05/17 at 7:37 PM.

namespace Librainian.Maths {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Struct for easily converting <see cref="Guid" /> to <see cref="FourBytes" />, <see cref="EightBytes" />, and
    ///     <see cref="Translate64" />.
    /// </summary>
    [StructLayout( layoutKind: LayoutKind.Explicit )]
    public struct Translate128 {

        [FieldOffset( offset: 0 )]
        public Translate64 Lower;

        [FieldOffset( offset: 0 )]
        public Guid Guid;

        /// <summary>
        ///     Just the first four bytes.
        /// </summary>
        [FieldOffset( offset: 0 )]
        public FourBytes FourBytes;

        [FieldOffset( offset: 0 )]
        public EightBytes EightBytesLow;

        [FieldOffset( offset: sizeof( UInt64 ) )]
        public Translate64 Higher;

        [FieldOffset( offset: sizeof( UInt64 ) )]
        public EightBytes EightBytesHigh;

        public Translate128( Guid guid ) {
            this.FourBytes = default;
            this.EightBytesLow = default;
            this.EightBytesHigh = default;
            this.Lower = default;
            this.Higher = default;
            this.Guid = guid;
        }

        public Translate128( Translate64 lower, Translate64 higher ) : this( Guid.Empty ) {
            this.Lower = lower;
            this.Higher = higher;
        }

        public Translate128( EightBytes lower, EightBytes higher ) : this( Guid.Empty ) {
            this.EightBytesLow = lower;
            this.EightBytesHigh = higher;
        }
    }
}