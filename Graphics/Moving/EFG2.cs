namespace Librainian.Graphics.Imaging {
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Experimental Framed Graphics
    /// </summary>
    /// <remarks>
    /// Just for fun & learning.
    /// Prefer compression over [decoding/display] speed (assuming local cpu will be 'faster' than network transfer speed).
    /// Compressions must be lossless.
    /// Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.
    /// Checksums are used on each line of each page to guard against (detect but not fix) corruption.
    /// </remarks>
    [DataContract]
    [Serializable]
    //[StructLayout( LayoutKind.Explicit )]
    public class EFG2 {

        /// <summary>
        /// Human readable file header.
        /// </summary>
        public static readonly String Header = "EFG1";      //TODO
        public static readonly String Extension = ".efg";   //TODO

        /// <summary>
        /// Checksum of all pages
        /// </summary>
        [DataMember]
        public UInt64 Checksum {
            get;
            set;
        }

        /// <summary>
        /// EXIF metadatas
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<String, String> Exifs = new ConcurrentDictionary<String, String>();

        [DataMember]
        public ConcurrentDictionary<UInt64, Frame> Frames = new ConcurrentDictionary<UInt64, Frame>();

        public EFG2() {
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