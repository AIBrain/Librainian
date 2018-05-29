// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Alpha.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/Alpha.cs" was last formatted by Protiguous on 2018/05/28 at 5:09 AM.

namespace Librainian.Persistence {

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using ComputerSystems;
    using ComputerSystems.Devices;
    using ComputerSystems.FileSystem;
    using JetBrains.Annotations;
    using Threading;

    /// <summary>
    ///     The last data storage class your program should ever need.
    /// </summary>
    public static class Alpha {

        /// <summary>
        ///     Pull the value out of the either.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean TryGet( String key, out String value ) {

            //get index for key
            //pull value from that index.
            //simple!
        }

        /*

//failure is not an option. No exceptions visible to user. If there is storage( ram, disk, ssd, flash, lan, inet) of any sort, let this class Store & Retrieve the data.

//screw security.. leave encrypt before storage and decrypt after retrieval up to the program/user

//get list of disks on local computer

//check isolatedstorage for any pointers to other known storage locations

//check AppDataa for any pointers to other storage locations

//use static PersistentDictionary access to store and retrieve. make it is using json serializer.

//get list of available lan storage

//redis? memcache?

//async Initialize()

//async Store(NameKeyValue)
//sync Store(NameKeyValue)

//async Retrieve(NameKey)
*/

        public static class Storage {

            private static TrackTime InitializeTime { get; } = new TrackTime();

            private static CancellationTokenSource LocalDiscoveryCancellation { get; } = new CancellationTokenSource();

            private static Task LocalDiscoveryTask { get; set; }

            private static TrackTime LocalResourceDiscoveryTime { get; } = new TrackTime();

            private static CancellationTokenSource RemoteDiscoveryCancellation { get; } = new CancellationTokenSource();

            private static Task RemoteDiscoveryTask { get; set; }

            private static TrackTime RemoteResourceDiscoveryTime { get; } = new TrackTime();

            private static async Task<Boolean> DiscoverLocalResources() {
                try {
                    LocalResourceDiscoveryTime.Started = DateTime.UtcNow;

                    var computer = new Computer();

                    //var drives = new DeviceClass(

                    //search drives for free space
                    //report back
                }
                catch ( Exception exception ) { exception.More(); }
                finally { LocalResourceDiscoveryTime.Finished = DateTime.UtcNow; }

                return false;
            }

            private static async Task<Boolean> DiscoverRemoteResources() {
                try {
                    RemoteResourceDiscoveryTime.Started = DateTime.UtcNow;

                    //search network/internet? for storage locations
                    //report back
                }
                catch ( Exception exception ) { exception.More(); }
                finally { RemoteResourceDiscoveryTime.Finished = DateTime.UtcNow; }

                return false;
            }

            /// <summary>
            ///     The <see cref="Root" /> folder for pointing to storage locations?
            /// </summary>
            public static PersistTable<String, I> Root { get; }

            /// <summary>
            ///     Where the main indexes will be stored.
            /// </summary>
            public static Folder RootPath { get; } = new Folder( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), Path.Combine( nameof( Storage ), nameof( Root ) ) ) );

            static Storage() {

                if ( !RootPath.Exists() ) {
                    RootPath.Create();

                    if ( !RootPath.Exists() ) { throw new DirectoryNotFoundException( RootPath.FullName ); }
                }

                Root = new PersistTable<String, I>( RootPath );
            }

            public static String BuildKey<T>( [NotNull] params T[] keys ) {
                if ( keys == null ) { throw new ArgumentNullException( paramName: nameof( keys ) ); }

                return keys.ToStrings( "⦀" );
            }

            public static TaskStatus GetLocalDiscoveryStatus() => LocalDiscoveryTask.Status;

            public static TaskStatus GetRemoteDiscoveryStatus() => RemoteDiscoveryTask.Status;

            /// <summary>
            ///     <para>Starts the local and remote discovery tasks.</para>
            /// </summary>
            /// <returns></returns>
            public static async Task Initialize() {
                try {
                    InitializeTime.Started = DateTime.UtcNow;
                    LocalDiscoveryTask = Task.Run( DiscoverLocalResources ).WithCancellation( LocalDiscoveryCancellation.Token );
                    RemoteDiscoveryTask = Task.Run( DiscoverRemoteResources ).WithCancellation( RemoteDiscoveryCancellation.Token );
                    await Task.Run( () => Parallel.Invoke( async () => await LocalDiscoveryTask.NoUI(), async () => await RemoteDiscoveryTask.NoUI() ) ).NoUI();
                }
                catch ( Exception exception ) { exception.More(); }
                finally { InitializeTime.Finished = DateTime.UtcNow; }
            }

            public static void RequestCancelLocalDiscovery() => LocalDiscoveryCancellation.Cancel( false );

            public static void RequestCancelRemoteDiscovery() => RemoteDiscoveryCancellation.Cancel( false );

            public class TrackTime {

                /// <summary>
                ///     Null? Hasn't finished yet.
                /// </summary>
                public DateTime? Finished { get; set; }

                /// <summary>
                ///     Null? Hasn't been started yet.
                /// </summary>
                public DateTime? Started { get; set; }
            }
        }
    }
}