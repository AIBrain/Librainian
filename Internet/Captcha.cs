// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Captcha.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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
        public String ChallengeElementID {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public String FormID {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public Uri ImageUri {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public String ResponseElementID {
            get; set;
        }

        public CaptchaStatus Status {
            get => this._status;

	        set {
                if ( !Equals( this._status, value ) ) {
                    this.StatusHistory.TryAdd( DateTime.Now, value );
                }
                this._status = value;
            }
        }

        [NotNull]
        [JsonProperty]
        public ConcurrentDictionary<DateTime, CaptchaStatus> StatusHistory { get; } = new ConcurrentDictionary<DateTime, CaptchaStatus>();

        [CanBeNull]
        [JsonProperty]
        public String SubmitID {
            get; set;
        }

        [CanBeNull]
        [JsonProperty]
        public Uri Uri {
            get; set;
        }
    }
}