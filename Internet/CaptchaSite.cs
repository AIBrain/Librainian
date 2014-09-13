namespace Librainian.Internet {
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;
    using Annotations;

    [DataContract(IsReference = true)]
    public class CaptchaSite {
        /// <summary>
        ///     URI of the captcha.
        /// </summary>
        [ CanBeNull ]
        [ DataMember ]
        public Uri Location {
            get;
            set;
        }

        [DataMember]
        public CaptchaStatus CaptchaStatus {
            get;
            set;
        }

        [ NotNull ] [ DataMember ] public ConcurrentDictionary<DateTime, CaptchaStatus> CaptchaHistory = new ConcurrentDictionary<DateTime, CaptchaStatus>();

        /// <summary>
        ///     When this website was added.
        /// </summary>
        [DataMember]
        public DateTime WhenAdded {
            get;
            set;
        }

        ///// <summary>
        /////     When the last webrequest was started.
        ///// </summary>
        //[DataMember]
        //public DateTime WhenRequestStarted {
        //    get;
        //    set;
        //}

        ///// <summary>
        /////     When the last webrequest got a  response.
        ///// </summary>
        //[DataMember]
        //public DateTime WhenResponseCame {
        //    get;
        //    set;
        //}

        /// <summary>
        ///     A count of the requests to scrape this captcha.
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

       

    }
}