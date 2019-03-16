// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Credentials.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "Credentials.cs" was last formatted by Protiguous on 2019/02/02 at 3:25 PM.

namespace Librainian.Internet {

    using System;
    using Exceptions;
    using JetBrains.Annotations;
    using Parsing;

    /// <summary>
    ///     Simple container for a <see cref="UserName" /> and <see cref="Password" />.
    /// </summary>
    public class Credentials {

        [CanBeNull]
        public String Name {
            get => this.UserName;
            set => this.UserName = value;
        }

        [CanBeNull]
        public String Pass {
            get => this.Password;
            set => this.Password = value;
        }

        [CanBeNull]
        public String Password { get; set; }

        [CanBeNull]
        public String PassWord {
            get => this.Password;
            set => this.Password = value;
        }

        [CanBeNull]
        public String User {
            get => this.UserName;
            set => this.UserName = value;
        }

        [CanBeNull]
        public String Userid {
            get => this.UserName;
            set => this.UserName = value;
        }

        [CanBeNull]
        public String UserId {
            get => this.UserName;
            set => this.UserName = value;
        }

        [CanBeNull]
        public String UserID {
            get => this.UserName;
            set => this.UserName = value;
        }

        [CanBeNull]
        public String UserName { get; set; }

        public Credentials() { }

        public Credentials( [NotNull] String username, [NotNull] String password ) {
            if ( String.IsNullOrWhiteSpace( username ) ) {
                throw new MissingTextException( nameof( username ) );
            }

            if ( String.IsNullOrEmpty( password ) ) {
                throw new MissingTextException( nameof( password ) );
            }

            this.UserName = username.EndsWith( "=" ) ? username.FromBase64() : username;
            this.Password = password.EndsWith( "=" ) ? password.FromBase64() : password;
        }

        public override String ToString() => $"{this.UserName ?? Symbols.Null} : {this.Password ?? Symbols.Null}";

    }

}