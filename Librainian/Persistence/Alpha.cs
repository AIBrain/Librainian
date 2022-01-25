// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Alpha.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

#nullable enable

namespace Librainian.Persistence;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using FileSystem;
using Logging;
using Parsing;

/// <summary>The *last* data storage class your program will ever need. Haha, I wish.</summary>
public static class Alpha {

	public interface IResourceSource {

		Task<TimeTracker> DiscoveryTask { get; set; }
	}

	/// <summary>Pull the value out of the either.</summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public static Boolean TryGet( String? key, out String? value ) {
		value = null;

		return false;
	}

	/*

//failure is not an option. NO EXCEPTIONS VISIBLE TO USER. If there is storage( ram, disk, ssd, flash, lan, inet) of any sort, let this class Store & Retrieve the data.

		//if the Key cannot be found in any storage locations, simply return default. don't throw any exceptions. I'm sick and tired of things throwing.

//screw security.. put encryption before storage and decryption after retrieval up to the user

//get list of disks on local computer

//check isolatedstorage for any pointers to other known storage locations

//check AppDataa for any pointers to other storage locations

//use static PersistentDictionary access to store and retrieve. make it "using json serializer".

//get list of available lan storage

//redis? memcache?

//async Initialize()

//async Store(NameKeyValue)
//sync Store(NameKeyValue)

//async Retrieve(NameKey)
*/

	public static class Storage {

		static Storage() {
			if ( !RootPath.Info.Exists ) {
				RootPath.Info.Create();
				RootPath.Info.Refresh();
				if ( !RootPath.Info.Exists ) {
					throw new DirectoryNotFoundException( RootPath.FullPath );
				}
			}

			Root = new PersistTable<String, String>( RootPath );
		}

		private static TimeTracker InitializeTimeTracker { get; } = new();

		private static Task? LocalDiscoveryTask { get; set; }

		private static TimeTracker LocalDiscoveryTimeTracker { get; } = new();

		private static Task? RemoteDiscoveryTask { get; set; }

		private static TimeTracker RemoteResourceDiscoveryTimeTracker { get; } = new();

		public static CancellationToken LocalDiscoveryCancellationToken { get; set; }

		public static CancellationToken RemoteDiscoveryCancellationToken { get; set; }

		/// <summary>The <see cref="Root" /> folder for pointing to storage locations?</summary>
		public static PersistTable<String, String> Root { get; }

		/// <summary>Where the main indexes will be stored.</summary>
		public static Folder RootPath { get; } = new( Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ),
			Path.Combine( nameof( Storage ), nameof( Root ) ) ) );

		private static Boolean DiscoverLocalResources() {
			try {

				//make this a task
				LocalDiscoveryTimeTracker.Started = DateTime.UtcNow;

				//var computer = new Computer();

				//var drives = new DeviceClass(

				//search drives for free space
				//report back
			}
			catch ( Exception exception ) {
				exception.Log();
			}
			finally {
				LocalDiscoveryTimeTracker.Finished = DateTime.UtcNow;
			}

			return false;
		}

		private static Boolean DiscoverRemoteResources() {
			try {

				//make this a task
				RemoteResourceDiscoveryTimeTracker.Started = DateTime.UtcNow;

				//search network/internet? for storage locations
				//report back
			}
			catch ( Exception exception ) {
				exception.Log();
			}
			finally {
				RemoteResourceDiscoveryTimeTracker.Finished = DateTime.UtcNow;
			}

			return false;
		}

		public static String BuildKey<T>( params T[] keys ) {
			if ( keys is null ) {
				throw new ArgumentEmptyException( nameof( keys ) );
			}

			return keys.ToStrings( Symbols.TriplePipes );
		}

		public static TaskStatus? GetLocalDiscoveryStatus() => LocalDiscoveryTask?.Status;

		public static TaskStatus? GetRemoteDiscoveryStatus() => RemoteDiscoveryTask?.Status;

		/// <summary>
		///     <para>Starts the local and remote discovery tasks.</para>
		/// </summary>
		/// <remarks>Is this coded in the correct way for starting Tasks?</remarks>
		public static async Task Initialize( CancellationToken? localToken = null, CancellationToken? remoteToken = null ) {
			try {
				InitializeTimeTracker.Started = DateTime.UtcNow;
				LocalDiscoveryTask = Task.Run( DiscoverLocalResources, localToken ?? LocalDiscoveryCancellationToken );
				RemoteDiscoveryTask = Task.Run( DiscoverRemoteResources, remoteToken ?? RemoteDiscoveryCancellationToken );
				await Task.WhenAll( LocalDiscoveryTask, RemoteDiscoveryTask ).ConfigureAwait( false );
			}
			catch ( Exception exception ) {
				exception.Log();
			}
			finally {
				InitializeTimeTracker.Finished = DateTime.UtcNow;
			}
		}
	}
}