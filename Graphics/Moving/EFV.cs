namespace Librainian.Graphics.Imaging {
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Experimental Full Video
    /// </summary>
    /// <remarks>
    /// Just for fun & learning.
    /// Prefer compression over [decoding/display] speed (assuming local cpu will be 'faster' than network transfer speed).
    /// Compressions must be lossless.
    /// Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.
    /// Checksums are used on each <see cref="Pixelyx"/> to guard against (detect but not fix) corruption.
    /// </remarks>
    [DataContract]
    [Serializable]
    public class EFV {

        /// <summary>
        /// Human readable file header.
        /// </summary>
        public static readonly String Header = "EFV1";      //TODO
        public static readonly String Extension = ".efv";   //TODO

        /// <summary>
        /// Checksum of all pages
        /// </summary>
        [DataMember]
        public UInt64 Checksum {
            get;
            set;
        }

        
        [DataMember]
        public ConcurrentDictionary<UInt64, Pixelyx> Pixels = new ConcurrentDictionary<UInt64, Pixelyx>();

        public EFV() {
            this.Checksum = UInt64.MaxValue;    //an unlikely hash
        }

        public async Task<Boolean> TryAdd( UInt64 index, Frame frame, CancellationToken token ) {
            return await Task.Run( () => {
                                       // ReSharper disable once ConvertToLambdaExpression
                                       return false;   //TODO add frame
                                   }, token );
        }


    }
}