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
// "Librainian2/Randem2.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading;
    using Annotations;

    public class Randem2 {
        [NotNull] public static readonly ThreadLocal< SHA256Managed > Crypts = new ThreadLocal< SHA256Managed >( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        /// <summary>
        ///     <para>Provide to each thread its own <see cref="Random" /> with a random seed.</para>
        /// </summary>
        [NotNull] public static readonly ThreadLocal< Random > ThreadSafeRandom = new ThreadLocal< Random >( valueFactory: () => {
                                                                                                                               var hash = Crypts.Value.ComputeHash( BitConverter.GetBytes( Interlocked.Increment( ref seed ) ).Concat( BitConverter.GetBytes( Guid.NewGuid().GetHashCode() ) ).ToArray() );
                                                                                                                               var iArray = new short[hash.Length/sizeof ( short )];
                                                                                                                               Buffer.BlockCopy( hash, 0, iArray, 0, hash.Length );

                                                                                                                               var aggregate = iArray.Aggregate( 0, ( current, i ) => current + Math.Abs( i ) );
                                                                                                                               return new Random( aggregate );
                                                                                                                           }, trackAllValues: false );

        private static int seed;

        static Randem2() {
            seed = Environment.TickCount;
        }
    }
}
