namespace Librainian.Internet {
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Runtime.Serialization;

    [DataContract]
    public class CaptchaSite {
        /// <summary>
        ///     URI of the captcha.
        /// </summary>
        [DataMember]
        public Uri Location {
            get;
            set;
        }

        [DataMember]
        public CaptchaStatus CaptchaStatus {
            get;
            set;
        }

        [DataMember]
        public ConcurrentDictionary<DateTime, CaptchaStatus> CaptchaHistory = new ConcurrentDictionary<DateTime, CaptchaStatus>();

        /// <summary>
        ///     When this website was added.
        /// </summary>
        [DataMember]
        public DateTime WhenAddedToQueue {
            get;
            set;
        }

        /// <summary>
        ///     When the last webrequest was started.
        /// </summary>
        [DataMember]
        public DateTime WhenRequestStarted {
            get;
            set;
        }

        /// <summary>
        ///     When the last webrequest got a  response.
        /// </summary>
        [DataMember]
        public DateTime WhenResponseCame {
            get;
            set;
        }

        /// <summary>
        ///     A count of the requests to scrape this website.
        /// </summary>
        [DataMember]
        public UInt64 RequestCount {
            get;
            set;
        }

        /// <summary>
        ///     A count of the responses for this url.
        /// </summary>
        [DataMember]
        public UInt64 ResponseCount {
            get;
            set;
        }

        /// <summary>
        ///     The <see cref="HttpWebRequest" />.
        /// </summary>
        public HttpWebRequest Request {
            get;
            set;
        }

    }
}