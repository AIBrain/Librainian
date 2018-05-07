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
// "Librainian/WebSite.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject]
    public class WebSite {

        [JsonProperty]
        public String Document {
            get; set;
        }

        /// <summary>URI of the website.</summary>
        [JsonProperty]
        public Uri Location {
            get; set;
        }

        /// <summary>The <see cref="HttpWebRequest" />.</summary>
        public HttpWebRequest Request {
            get; set;
        }

        /// <summary>A count of the requests to scrape this website.</summary>
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
        public DateTime WhenAddedToQueue {
            get; set;
        }

        /// <summary>When the last webrequest was started.</summary>
        [JsonProperty]
        public DateTime WhenRequestStarted {
            get; set;
        }

        /// <summary>When the last webrequest got a response.</summary>
        [JsonProperty]
        public DateTime WhenResponseCame {
            get; set;
        }
    }
}