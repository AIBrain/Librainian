namespace Librainian.Internet {

    using System;
    using System.Runtime.Serialization;
    using Annotations;

    [DataContract( IsReference = true )]
    public class CaptchaSite {

        [DataMember]
        public CaptchaStatus CaptchaStatus {
            get;
            set;
        }

        /// <summary>
        ///     URI of the captcha.
        /// </summary>
        [CanBeNull]
        [DataMember]
        public Uri Location {
            get;
            set;
        }

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

        /// <summary>
        ///     When this website was added.
        /// </summary>
        [DataMember]
        public DateTime WhenAdded {
            get;
            set;
        }
    }
}