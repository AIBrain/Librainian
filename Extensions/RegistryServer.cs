#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/RegistryServer.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

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

        private static int iCounter;

        private HashSet<RegistryKey> _allKeys;

        private PopulateProgressEventArgs _eventArgStatus;

        private Boolean _isInitialized;

        private PopulateProgressDelegateError _populateError;

        private PopulateProgressDelegate _populateEventOk;

        static RegistryServer() {
            Instance = new RegistryServer();
        }

        private RegistryServer() {
        }

        public static event PopulateProgressDelegate PopulateProgress {
            add { Instance._populateEventOk += value; }

            // ReSharper disable DelegateSubtraction
            remove { Instance._populateEventOk -= value; }

            // ReSharper restore DelegateSubtraction
        }

        public static event PopulateProgressDelegateError PopulateProgressItemError {
            add { Instance._populateError += value; }

            // ReSharper disable DelegateSubtraction
            remove { Instance._populateError -= value; }

            // ReSharper restore DelegateSubtraction
        }

        public static long Count {
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

        #region IEnumerable<RegistryKey> Members

        public IEnumerator<RegistryKey> GetEnumerator() {
            if ( !this._isInitialized ) {
                throw new InvalidOperationException( "Please initialize the backing store first" );
            }

            return this._allKeys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        #endregion IEnumerable<RegistryKey> Members

        #region IEqualityComparer<RegistryKey> Members

        /// <summary>
        /// If either contains a null, the result is false (actually it is null be we do not have
        /// that option. It is 'unknown and indeterminant'. An emptry String however is treated as
        /// 'known to be empty' where null is 'could be anything we have no idea'.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals( RegistryKey x, RegistryKey y ) {
            return x.Name != null && y.Name != null && x.Name == y.Name;
        }

        /// <summary>
        /// For null names here we will calculate a funky random number as null != null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode( RegistryKey obj ) {
            return obj.Name != null ? obj.Name.GetHashCode() : RuntimeHelpers.GetHashCode( new object() );
        }

        #endregion IEqualityComparer<RegistryKey> Members

        #region IInitializable Members

        //void IInitializable.Initialize() { Initialize(); }

        #endregion IInitializable Members

        public static void Initialize() {
            Initialize( Registry.LocalMachine );
        }

        private static IEnumerable<RegistryKey> GetAllSubkeys( RegistryKey startkeyIn, String nodeKey ) {
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

        private static void Initialize( RegistryKey registryStartKey ) {
            if ( Instance._isInitialized ) {
                return;
            }
            Instance._eventArgStatus = new PopulateProgressEventArgs();

            Instance._allKeys = GetAllSubkeys( registryStartKey, "" ).ToHashSet( Instance );

            Instance._isInitialized = true;
        }

        private static void InvokePopulateProgressItemError( PopulateProgressEventArgs args ) {
            var populateProgressDelegateError = Instance._populateError;
            if ( populateProgressDelegateError != null ) {
                populateProgressDelegateError( Instance, args );
            }
        }

        private static Boolean TryOpenSubKey( RegistryKey StartFrom, String Name, out RegistryKey itemOut ) {
            var bIsOk = false;
            itemOut = null;

            try {
                itemOut = StartFrom.OpenSubKey( Name, RegistryKeyPermissionCheck.ReadSubTree );
                if ( itemOut != null ) {
                    bIsOk = true;
                }
            }
            catch ( Exception ex ) {
                InvokePopulateProgressItemError( new PopulateProgressEventArgs( -1, ex.Message + Environment.NewLine + "Key=" + StartFrom.Name + " failed trying " + Name ) );
            }

            return bIsOk;
        }

        private void InvokePopulateProgress() {
            var populateProgressDelegate = Instance._populateEventOk;
            if ( populateProgressDelegate == null ) {
                return;
            }
            this._eventArgStatus.ItemCount = Interlocked.Increment( ref iCounter );
            populateProgressDelegate( this, this._eventArgStatus );
        }
    }
}