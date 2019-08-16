// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "RegistryServer.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "RegistryServer.cs" was last formatted by Protiguous on 2019/08/08 at 7:20 AM.

namespace Librainian.Extensions {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using JetBrains.Annotations;
    using Microsoft.Win32;

    /// <summary>
    ///     Provider immutable projections from the registry of the machine, as well as events for
    ///     status and errors via a singeton wrapper on the .NET Registry singleton. Here we are only
    ///     exposing the HKLM area subkey but you can see it is easily extensible
    /// </summary>
    public class RegistryServer : IEqualityComparer<RegistryKey> /*, IInitializable*/, IEnumerable<RegistryKey> {

        public IEnumerator<RegistryKey> GetEnumerator() {
            if ( !this._isInitialized ) {
                throw new InvalidOperationException( "Please initialize the backing store first" );
            }

            return this._allKeys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     If either contains a null, the result is false (actually it is null be we do not have
        ///     that option. It is 'unknown and indeterminant'. An emptry String however is treated as
        ///     'known to be empty' where null is 'could be anything we have no idea'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals( RegistryKey x, RegistryKey y ) => x.Name != null && y.Name != null && x.Name == y.Name;

        /// <summary>
        ///     For null names here we will calculate a funky random number as null != null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode( RegistryKey obj ) => obj.Name?.GetHashCode() ?? RuntimeHelpers.GetHashCode( new Object() );

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

        // IInitializable is from Castle.Core Contractually saying we need a call on our
        // Initialize() method before we can be given out as a service to others

        private static readonly RegistryServer Instance;

        private static Int32 _iCounter;

        static RegistryServer() => Instance = new RegistryServer();

        private RegistryServer() { }

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

        private static IEnumerable<RegistryKey> GetAllSubkeys( [CanBeNull] RegistryKey startkeyIn, String nodeKey ) {
            Instance.InvokePopulateProgress();

            if ( startkeyIn == null ) {
                yield break;
            }

            if ( !TryOpenSubKey( startkeyIn, nodeKey, out var subItemRoot ) ) {
                yield break;
            }

            yield return subItemRoot;

            foreach ( var sub in subItemRoot.GetSubKeyNames().SelectMany( s => GetAllSubkeys( subItemRoot, s ) ) ) {
                yield return sub;
            }
        }

        private static void Initialize( RegistryKey registryStartKey ) {
            if ( Instance._isInitialized ) {
                return;
            }

            Instance._eventArgStatus = new PopulateProgressEventArgs();

            Instance._allKeys = GetAllSubkeys( registryStartKey, "" ).ToHashSet( Instance );

            Instance._isInitialized = true;
        }

        private static void InvokePopulateProgressItemError( PopulateProgressEventArgs args ) => Instance._populateError?.Invoke( Instance, args );

        private static Boolean TryOpenSubKey( [NotNull] RegistryKey startFrom, String name, [CanBeNull] out RegistryKey itemOut ) {
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

            if ( populateProgressDelegate == null ) {
                return;
            }

            this._eventArgStatus.ItemCount = Interlocked.Increment( ref _iCounter );
            populateProgressDelegate( this, this._eventArgStatus );
        }

        public static void Initialize() => Initialize( Registry.LocalMachine );
    }
}