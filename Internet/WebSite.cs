#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/WebSite.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    [DataContract]
    public class WebSite {
        /// <summary>
        ///     URI of the website.
        /// </summary>
        [DataMember]
        public Uri Location { get; set; }

        [DataMember]
        public String Document { get; set; }

        /// <summary>
        ///     When this website was added.
        /// </summary>
        [DataMember]
        public DateTime WhenAddedToQueue { get; set; }

        /// <summary>
        ///     When the webrequest was started.
        /// </summary>
        [DataMember]
        public DateTime WhenRequestStarted { get; set; }

        /// <summary>
        ///     When a webrequest was responded to.
        /// </summary>
        [DataMember]
        public DateTime WhenResponseCame { get; set; }

        /// <summary>
        ///     A count of the number of requests to scrape this website.
        /// </summary>
        [DataMember]
        public UInt64 RequestCount { get; set; }

        /// <summary>
        ///     A count of the number of responses for this url.
        /// </summary>
        [DataMember]
        public UInt64 ResponseCount { get; set; }

        /// <summary>
        ///     The <see cref="HttpWebRequest" />.
        /// </summary>
        public HttpWebRequest Request { get; set; }

        //public Action<WebSite> ResponseAction { get; set; }
    }
}
