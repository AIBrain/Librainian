// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "RegistryServer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "RegistryServer.cs" was last formatted by Protiguous on 2020/03/16 at 3:04 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    /// Provider immutable projections from the registry of the machine, as well as events for status and errors via a singeton wrapper on the .NET Registry singleton. Here we
    /// are only exposing the HKLM area subkey but you can see it is easily extensible
    /// </summary>
    public class RegistryServer : IEqualityComparer<RegistryKey> /*, IInitializable*/, IEnumerable<RegistryKey> {

        [NotNull]
        private static readonly RegistryServer Instance;

        // IInitializable is from Castle.Core Contractually saying we need a call on our
        // Initialize() method before we can be given out as a service to others
        private static Int32 _iCounter;

        private HashSet<RegistryKey> _allKeys;

        private PopulateProgressEventArgs _eventArgStatus;

        private Boolean _isInitialized;

        private PopulateProgressDelegateError _populateError;

        private PopulateProgressDelegate _populateEventOk;

        public static Int64 Count {
            get {
                if ( !Instance._isInitialized ) {
                    throw new InvalidOperationException( "Please initialize the backing store first" );
                }

                return Instance._allKeys.Count;
            }
        }

        [NotNull]
        public static RegistryServer Hklm {
            get {
                if ( !Instance._isInitialized ) {
                    throw new InvalidOperationException( "Please initialize the backing store first" );
                }

                return Instance;
            }
        }

        private RegistryServer() { }

        static RegistryServer() => Instance = new RegistryServer();

        public static event PopulateProgressDelegate PopulateProgress {
            add => Instance._populateEventOk += value;

            // ReSharper disable DelegateSubtraction
            remove => Instance._populateEventOk -= value;

            // ReSharper restore DelegateSubtraction
        }

        //void IInitializable.Initialize() { Initialize(); }
        public static event PopulateProgressDelegateError PopulateProgressItemError {
            add => Instance._populateError += value;

            // ReSharper disable DelegateSubtraction
            remove => Instance._populateError -= value;

            // ReSharper restore DelegateSubtraction
        }

        [ItemCanBeNull]
        private static IEnumerable<RegistryKey> GetAllSubkeys( [NotNull] RegistryKey startkeyIn, [NotNull] String nodeKey ) {
            if ( startkeyIn == null ) {
                throw new ArgumentNullException( nameof( startkeyIn ) );
            }

            if ( nodeKey == null ) {
                throw new ArgumentNullException( nameof( nodeKey ) );
            }

            Instance.InvokePopulateProgress();

            if ( !TryOpenSubKey( startkeyIn, nodeKey, out var subItemRoot ) ) {
                yield break;
            }

            yield return subItemRoot;

            if ( subItemRoot != null ) {
                foreach ( var sub in subItemRoot.GetSubKeyNames().SelectMany( s => GetAllSubkeys( subItemRoot, s ) ) ) {
                    yield return sub;
                }
            }
        }

        private static void Initialize( [NotNull] RegistryKey registryStartKey ) {
            if ( registryStartKey == null ) {
                throw new ArgumentNullException( nameof( registryStartKey ) );
            }

            if ( Instance._isInitialized ) {
                return;
            }

            Instance._eventArgStatus = new PopulateProgressEventArgs();

            Instance._allKeys = GetAllSubkeys( registryStartKey, "" ).ToHashSet( Instance );

            Instance._isInitialized = true;
        }

        private static void InvokePopulateProgressItemError( [CanBeNull] PopulateProgressEventArgs args ) => Instance._populateError.Invoke( Instance, args );

        private static Boolean TryOpenSubKey( [NotNull] RegistryKey startFrom, [NotNull] String name, [CanBeNull] out RegistryKey itemOut ) {
            if ( startFrom == null ) {
                throw new ArgumentNullException( nameof( startFrom ) );
            }

            if ( String.IsNullOrWhiteSpace( name ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
            }

            var bIsOk = false;
            itemOut = null;

            try {
                itemOut = startFrom.OpenSubKey( name, RegistryKeyPermissionCheck.ReadSubTree );

                if ( itemOut != null ) {
                    bIsOk = true;
                }
            }
            catch ( Exception ex ) {
                InvokePopulateProgressItemError( new PopulateProgressEventArgs( -1, ex.Message + Environment.NewLine + "Key=" + startFrom.Name + " failed trying " + name ) );
            }

            return bIsOk;
        }

        private void InvokePopulateProgress() {
            var populateProgressDelegate = Instance._populateEventOk;

            this._eventArgStatus.ItemCount = Interlocked.Increment( ref _iCounter );
            populateProgressDelegate( this, this._eventArgStatus );
        }

        public static void Initialize() => Initialize( Registry.LocalMachine );

        /// <summary>
        /// If either contains a null, the result is false (actually it is null be we do not have that option. It is 'unknown and indeterminant'. An emptry String however is treated
        /// as 'known to be empty' where null is 'could be anything we have no idea'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals( RegistryKey x, RegistryKey y ) {
            if ( x is null ) {
                throw new ArgumentNullException( nameof( x ) );
            }

            if ( y is null ) {
                throw new ArgumentNullException( nameof( y ) );
            }

            return x.Name != null && y.Name != null && x.Name == y.Name;
        }

        public IEnumerator<RegistryKey> GetEnumerator() {
            if ( !this._isInitialized ) {
                throw new InvalidOperationException( "Please initialize the backing store first" );
            }

            return this._allKeys.GetEnumerator();
        }

        /// <summary>For null names here we will calculate a funky random number as null != null</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode( [CanBeNull] RegistryKey obj ) => obj?.Name?.GetHashCode() ?? 0;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}