// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Alpha.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Alpha.cs" was last formatted by Protiguous on 2018/07/13 at 1:35 AM.

namespace Librainian.Persistence {

	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using Collections;
	using ComputerSystem;
	using ComputerSystem.FileSystem;
	using JetBrains.Annotations;
	using Logging;

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
		public static Boolean TryGet( String key, [CanBeNull] out String value ) {
			value = null;

			return false;
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

			private static Boolean DiscoverLocalResources() {
				try {

					//make this a task
					LocalResourceDiscoveryTime.Started = DateTime.UtcNow;

					var computer = new Computer();

					//var drives = new DeviceClass(

					//search drives for free space
					//report back
				}
				catch ( Exception exception ) { exception.Log(); }
				finally { LocalResourceDiscoveryTime.Finished = DateTime.UtcNow; }

				return false;
			}

			private static Boolean DiscoverRemoteResources() {
				try {

					//make this a task
					RemoteResourceDiscoveryTime.Started = DateTime.UtcNow;

					//search network/internet? for storage locations
					//report back
				}
				catch ( Exception exception ) { exception.Log(); }
				finally { RemoteResourceDiscoveryTime.Finished = DateTime.UtcNow; }

				return false;
			}

			public static String BuildKey<T>( [NotNull] params T[] keys ) {
				if ( keys == null ) { throw new ArgumentNullException( paramName: nameof( keys ) ); }

				return keys.ToStrings( Parsing.ParsingExtensions.TriplePipes );
			}

			public static TaskStatus GetLocalDiscoveryStatus() => LocalDiscoveryTask.Status;

			public static TaskStatus GetRemoteDiscoveryStatus() => RemoteDiscoveryTask.Status;

			/// <summary>
			///     <para>Starts the local and remote discovery tasks.</para>
			/// </summary>
			/// <returns></returns>
			/// <remarks>Is this coded in the correct way for starting Tasks?</remarks>
			public static async Task Initialize() {
				try {
					InitializeTime.Started = DateTime.UtcNow;
					LocalDiscoveryTask = Task.Run( DiscoverLocalResources, LocalDiscoveryCancellation.Token );
					RemoteDiscoveryTask = Task.Run( DiscoverRemoteResources, RemoteDiscoveryCancellation.Token );
					await Task.WhenAll( LocalDiscoveryTask, RemoteDiscoveryTask ).ConfigureAwait( false );
				}
				catch ( Exception exception ) { exception.Log(); }
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