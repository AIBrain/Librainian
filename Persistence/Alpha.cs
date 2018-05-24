// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Alpha.cs" belongs to Rick@AIBrain.org and
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
// "Librainian/Librainian/Alpha.cs" was last formatted by Protiguous on 2018/05/21 at 11:18 PM.

namespace Librainian.Persistence {

    using System;

    /// <summary>
    ///     The last data storage class your program should ever need.
    /// </summary>
    public static class Alpha {

        /*

        //failure is not an option. No exceptions visible to user. If there is storage( ram, disk, ssd, flash, lan, inet) of any sort, let this class Store & Retrieve the data.

        //screw security.. leave encrypt before storage and decrypt after retrieval up to the program/user

        //get list of disks on local computer

        //check isolatedstorage for any pointers to other known storage locations

        //check AppDataa for any pointers to other storage locations

        //use static PersistentDictionary access to store and retrieve. make it is using json serializer.

        //get list of available lan storage

        //redis? memcache?

        //async Initialize()

        //async Store(NameKeyValue)
        //sync Store(NameKeyValue)

        //async Retrieve(NameKey)
        */

        /// <summary>
        ///     Data([Name]Key=Value)
        /// </summary>
        public struct D {

            /// <summary>
            ///     The *pointer* to the value (<see cref="V" />).
            /// </summary>
            public String K { get; set; }

            /// <summary>
            ///     The namespace the key belongs in.
            /// </summary>
            public String N { get; set; }

            /// <summary>
            ///     The value. serialized to &amp; from json string&lt;-&gt;object.
            /// </summary>
            public String V { get; set; } //obj serialized to json

        }

    }

}