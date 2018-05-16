// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Captcha.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Captcha.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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