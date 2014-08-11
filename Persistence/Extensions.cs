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
// "Librainian/Extensions.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Persistence {
    using System;
    using System.Collections.Concurrent;
    using Collections;

    public static class Extensions {
        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Properties have to have both the Getter and the Setter.<br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection< T >( this IProducerConsumerCollection< T > collection, String fileName ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( "fileName" );
            }

            return collection.Saver( fileName: fileName );
        }

        /// <summary>
        ///     Persist an object to an IsolatedStorageFile.<br />
        ///     Mark class with [DataContract( Namespace = "http://aibrain.org" )]<br />
        ///     Mark fields with [DataMember, OptionalField] to serialize (both public and private).<br />
        ///     Properties have to have both the Getter and the Setter.<br />
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="fileName"></param>
        /// <returns>Returns True if the object was saved.</returns>
        [Obsolete( "Not in use yet." )]
        public static Boolean SaveCollection< T >( this ThreadSafeList< T > collection, String fileName ) where T : class {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            if ( String.IsNullOrWhiteSpace( fileName ) ) {
                throw new ArgumentNullException( "fileName" );
            }

            return collection.Saver( fileName: fileName );
        }

        //TODO

        ///// <summary>
        /////   Attempts to Add() the specified filename into the collection.
        ///// </summary>
        ///// <typeparam name = "T"></typeparam>
        ///// <param name = "collection"></param>
        ///// <param name = "fileName"></param>
        ///// <returns></returns>
        //public static Boolean LoadCollection< T >( this IProducerConsumerCollection< T > collection, String fileName ) {
        //    if ( collection == null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }
        //    if ( fileName == null ) {
        //        throw new ArgumentNullException( "fileName" );
        //    }
        //    IProducerConsumerCollection< T > temp;
        //    if ( Storage.Load( out temp, fileName ) ) {
        //        if ( null != temp ) {
        //            var result = Parallel.ForEach( temp, collection.Add );
        //            return result.IsCompleted;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        /////   Attempts to Add() the specified filename into the collection.
        ///// </summary>
        ///// <typeparam name = "T"></typeparam>
        ///// <param name = "collection"></param>
        ///// <param name = "fileName"></param>
        ///// <returns></returns>
        //public static Boolean LoadCollection<T>( this ConcurrentSet<T> collection, String fileName ) where T : class {
        //    if ( collection == null ) {
        //        throw new ArgumentNullException( "collection" );
        //    }
        //    if ( fileName == null ) {
        //        throw new ArgumentNullException( "fileName" );
        //    }
        //    ConcurrentSet<T> temp;
        //    if ( Storage.Load( out temp, fileName ) ) {
        //        if ( null != temp ) {
        //            collection.AddRange( temp );
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
