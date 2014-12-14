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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Captcha.cs" was last cleaned by Rick on 2014/09/13 at 5:48 PM

#endregion License & Information

namespace Librainian.Internet {

    using System;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;
    using Annotations;

    [DataContract( IsReference = true )]
    public class Captcha {
        [ NotNull ]
        [ DataMember ]
        public ConcurrentDictionary< DateTime, CaptchaStatus > StatusHistory { get; } = new ConcurrentDictionary< DateTime, CaptchaStatus >();

        [DataMember]
        private CaptchaStatus _status;

        [CanBeNull]
        [DataMember]
        public String ChallengeElementID {
            get;
            set;
        }

        //[CanBeNull]
        //[DataMember]
        //public Image Image {
        //    get;
        //    set;
        //}

        [CanBeNull]
        [DataMember]
        public Uri ImageUri {
            get;
            set;
        }


        [CanBeNull]
        [DataMember]
        public String ResponseElementID {
            get;
            set;
        }

        public CaptchaStatus Status {
            get {
                return this._status;
            }

            set {
                if ( !Equals( this._status, value ) ) {
                    this.StatusHistory.TryAdd( DateTime.Now, value );
                }
                this._status = value;
            }
        }

        [CanBeNull]
        [DataMember]
        public String FormID {
            get;
            set;
        }

        [CanBeNull]
        [DataMember]
        public String SubmitID {
            get;
            set;
        }

        [CanBeNull]
        [DataMember]
        public Uri Uri {
            get;
            set;
        }
    }
}