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
// "Librainian/EFGExtensions.cs" was last cleaned by Rick on 2014/11/17 at 4:00 PM

#endregion License & Information

namespace Librainian.Graphics {
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Imaging;
    using IO;
    using Moving;

    public static class GraphicsExtensions {

        public static Stream EFVToStream() {
            var ms = new MemoryStream( );
        }

        public static async Task<bool> TrySave( this EFV efv, Document document, CancellationToken token ) {
            return await Task.Run( () => {
                //TODO recalc the checksums
                //write out to file
                // ReSharper disable once ConvertToLambdaExpression
                                       var bob = new BinaryFormatter();
                bob.Serialize(
                return false;
            }, token );
        }

        public static async Task<bool> TrySave( this EFG efg, Document document, CancellationToken token ) {
            return await Task.Run( () => {
                //TODO recalc the checksums
                //write out to file
                                       // ReSharper disable once ConvertToLambdaExpression
                return false;
            }, token );
        }

        public static async Task<EFG> TryLoad( this Document document, CancellationToken token ) {
            return await Task.Run( () => {
                //TODO recalc the checksums
                //load file, checking checksums along the way.. (skip frames/lines with bad checksums?)
                                       // ReSharper disable once ConvertToLambdaExpression
                return new EFG();
            }, token );
        }
    }
}