// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/RegistryServer.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.Win32;

    /// <summary>
    /// Provider immutable projections from the registry of the machine, as well as events for
    /// status and errors via a singeton wrapper on the .NET Registry singleton. Here we are only
    /// exposing the HKLM area subkey but you can see it is easily extensible
    /// </summary>
    public class RegistryServer : IEqualityComparer<RegistryKey> /*, IInitializable*/, IEnumerable<RegistryKey> {

        // IInitializable is from Castle.Core Contractually saying we need a call on our
        // Initialize() method before we can be given out as a service to others

        private static readonly RegistryServer Instance;
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

        public static RegistryServer Hklm {
            get {
                if ( !Instance._isInitialized ) {
                    throw new InvalidOperationException( "Please initialize the backing store first" );
                }

                return Instance;
            }
        }

        public static event PopulateProgressDelegate PopulateProgress {
            add {
                Instance._populateEventOk += value;
            }

            // ReSharper disable DelegateSubtraction
            remove {
                Instance._populateEventOk -= value;
            }

            // ReSharper restore DelegateSubtraction
        }

        //void IInitializable.Initialize() { Initialize(); }
        public static event PopulateProgressDelegateError PopulateProgressItemError {
            add {
                Instance._populateError += value;
            }

            // ReSharper disable DelegateSubtraction
            remove {
                Instance._populateError -= value;
            }

            // ReSharper restore DelegateSubtraction
        }

        static RegistryServer() {
            Instance = new RegistryServer();
        }

        private RegistryServer() {
        }

        public static void Initialize() => Initialize( Registry.LocalMachine );

        /// <summary>
        /// If either contains a null, the result is false (actually it is null be we do not have
        /// that option. It is 'unknown and indeterminant'. An emptry String however is treated as
        /// 'known to be empty' where null is 'could be anything we have no idea'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals(RegistryKey x, RegistryKey y) => ( x.Name != null ) && ( y.Name != null ) && ( x.Name == y.Name );

        public IEnumerator<RegistryKey> GetEnumerator() {
            if ( !this._isInitialized ) {
                throw new InvalidOperationException( "Please initialize the backing store first" );
            }

            return this._allKeys.GetEnumerator();
        }

        /// <summary>
        /// For null names here we will calculate a funky random number as null != null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode(RegistryKey obj) => obj.Name?.GetHashCode() ?? RuntimeHelpers.GetHashCode( new Object() );

        private static IEnumerable<RegistryKey> GetAllSubkeys(RegistryKey startkeyIn, String nodeKey) {
            Instance.InvokePopulateProgress();

            if ( startkeyIn == null ) {
                yield break;
            }

            RegistryKey subItemRoot;

            if ( !TryOpenSubKey( startkeyIn, nodeKey, out subItemRoot ) ) {
                yield break;
            }
            yield return subItemRoot;

            foreach ( var sub in subItemRoot.GetSubKeyNames().SelectMany( s => GetAllSubkeys( subItemRoot, s ) ) ) {
                yield return sub;
            }
        }

        private static void Initialize(RegistryKey registryStartKey) {
            if ( Instance._isInitialized ) {
                return;
            }
            Instance._eventArgStatus = new PopulateProgressEventArgs();

            Instance._allKeys = GetAllSubkeys( registryStartKey, "" ).ToHashSet( Instance );

            Instance._isInitialized = true;
        }

        private static void InvokePopulateProgressItemError(PopulateProgressEventArgs args) {
            var populateProgressDelegateError = Instance._populateError;
            if ( populateProgressDelegateError != null ) {
                populateProgressDelegateError( Instance, args );
            }
        }

        private static Boolean TryOpenSubKey(RegistryKey startFrom, String name, out RegistryKey itemOut) {
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

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void InvokePopulateProgress() {
            var populateProgressDelegate = Instance._populateEventOk;
            if ( populateProgressDelegate == null ) {
                return;
            }
            this._eventArgStatus.ItemCount = Interlocked.Increment( ref _iCounter );
            populateProgressDelegate( this, this._eventArgStatus );
        }
    }
}