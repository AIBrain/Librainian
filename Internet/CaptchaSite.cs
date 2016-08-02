// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CaptchaSite.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class CaptchaSite {

        [JsonProperty]
        public CaptchaStatus CaptchaStatus {
            get; set;
        }

        /// <summary>URI of the captcha.</summary>
        [CanBeNull]
        [JsonProperty]
        public Uri Location {
            get; set;
        }

        /// <summary>A count of the requests to scrape this captcha.</summary>
        [JsonProperty]
        public UInt64 RequestCount {
            get; set;
        }

        /// <summary>A count of the responses for this url.</summary>
        [JsonProperty]
        public UInt64 ResponseCount {
            get; set;
        }

        /// <summary>When this website was added.</summary>
        [JsonProperty]
        public DateTime WhenAdded {
            get; set;
        }
    }
}