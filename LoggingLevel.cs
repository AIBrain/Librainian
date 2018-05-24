// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "LoggingLevel.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/LoggingLevel.cs" was last formatted by Protiguous on 2018/05/22 at 3:51 PM.

namespace Librainian {

    using System;

    public enum LoggingLevel : Byte {

        /// <summary>
        ///     The root of all extra information.
        /// </summary>
        Trace = 0,

        /// <summary>
        ///     Just debugging information please.
        /// </summary>
        Debug,

        /// <summary>
        ///     I need more info than <see cref="Debug" />.
        /// </summary>
        Info,

        /// <summary>
        ///     Okay, I didn't need that much!
        /// </summary>
        Verbose,

        /// <summary>
        ///     A simple warning happened. Fix it and continue.
        /// </summary>
        Warning,

        /// <summary>
        ///     Requires user input to fix, and then continue.
        /// </summary>
        Error,

        /// <summary>
        ///     It happens.
        /// </summary>
        Exception,

        /// <summary>
        ///     An exception we can recover from.
        /// </summary>
        Critical,

        /// <summary>
        ///     The program must stop soon, before any damage is done.
        /// </summary>
        Fatal
    }
}