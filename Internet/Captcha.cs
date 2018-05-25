// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Captcha.cs" belongs to Rick@AIBrain.org and
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Captcha.cs" was last formatted by Protiguous on 2018/05/24 at 7:14 PM.

namespace Librainian.Internet {

    using System;
    using System.Collections.Concurrent;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public sealed class Captcha {

        [JsonProperty]
        private CaptchaStatus _status;

        [CanBeNull]
        [JsonProperty]
        public String ChallengeElementID { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String FormID { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Uri ImageUri { get; set; }

        [CanBeNull]
        [JsonProperty]
        public String ResponseElementID { get; set; }

        public CaptchaStatus Status {
            get => this._status;

            set {
                if ( !Equals( this._status, value ) ) { this.StatusHistory.TryAdd( DateTime.Now, value ); }

                this._status = value;
            }
        }

        [NotNull]
        [JsonProperty]
        public ConcurrentDictionary<DateTime, CaptchaStatus> StatusHistory { get; } = new ConcurrentDictionary<DateTime, CaptchaStatus>();

        [CanBeNull]
        [JsonProperty]
        public String SubmitID { get; set; }

        [CanBeNull]
        [JsonProperty]
        public Uri Uri { get; set; }
    }
}