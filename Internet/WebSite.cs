// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WebSite.cs",
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
// "Librainian/Librainian/WebSite.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Internet {

    using System;
    using System.Net;
    using Newtonsoft.Json;

    [JsonObject]
    public class WebSite {

        [JsonProperty]
        public String Document { get; set; }

        /// <summary>URI of the website.</summary>
        [JsonProperty]
        public Uri Location { get; set; }

        /// <summary>The <see cref="HttpWebRequest" />.</summary>
        public HttpWebRequest Request { get; set; }

        /// <summary>A count of the requests to scrape this website.</summary>
        [JsonProperty]
        public UInt64 RequestCount { get; set; }

        /// <summary>A count of the responses for this url.</summary>
        [JsonProperty]
        public UInt64 ResponseCount { get; set; }

        /// <summary>When this website was added.</summary>
        [JsonProperty]
        public DateTime WhenAddedToQueue { get; set; }

        /// <summary>When the last webrequest was started.</summary>
        [JsonProperty]
        public DateTime WhenRequestStarted { get; set; }

        /// <summary>When the last webrequest got a response.</summary>
        [JsonProperty]
        public DateTime WhenResponseCame { get; set; }
    }
}