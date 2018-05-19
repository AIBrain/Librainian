// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any
// source code used or derived from our binaries, libraries, projects, or solutions.
//
// This source code, "EightBytes.cs", belongs to Rick@AIBrain.org
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
// "Librainian/Librainian/EightBytes.cs" was last formatted by Protiguous on 2018/05/17 at 7:36 PM.

namespace Librainian.Maths {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     8 byte struct.
    /// </summary>
    [StructLayout( layoutKind: LayoutKind.Sequential )]
    public struct EightBytes {

        public Byte A;

        public Byte B;

        public Byte C;

        public Byte D;

        public Byte E;

        public Byte F;

        public Byte G;

        public Byte H;
    }
}