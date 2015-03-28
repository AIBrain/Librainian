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
// "Librainian 2015/InternetExtensions.cs" was last cleaned by aibra_000 on 2015/03/28 at 5:02 AM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    public static class InternetExtensions {
        /// <summary>
        /// Return the machine's hostname
        /// </summary>
        public static String GetHostName() => Dns.GetHostName();

        /// <summary>
        ///     Convert a string to network bytes
        /// </summary>
        public static IEnumerable< byte > ToNetworkBytes( this String data ) {
            var bytes = Encoding.UTF8.GetBytes( data );

            var len = IPAddress.HostToNetworkOrder( ( short ) bytes.Length );

            return BitConverter.GetBytes( len ).Concat( bytes );
        }

        /// <summary>
        ///     Convert network bytes to a string
        /// </summary>
        public static string FromNetworkBytes( this IEnumerable< Byte > data ) {
            var listData = data as IList< byte > ?? data.ToList();

            var len = IPAddress.NetworkToHostOrder( BitConverter.ToInt16( listData.Take( 2 ).ToArray(), 0 ) );
            if ( listData.Count() < 2 + len ) {
                throw new ArgumentException( "Too few bytes in packet" );
            }

            return Encoding.UTF8.GetString( listData.Skip( 2 ).Take( len ).ToArray() );
        }
    }
}
