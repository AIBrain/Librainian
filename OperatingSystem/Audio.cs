// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Audio.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    public static class Audio {

        public enum EDataFlow {
            eRender,

            eCapture,

            eAll,

            EDataFlow_enum_count
        }

        public enum ERole {
            eConsole,

            eMultimedia,

            eCommunications,

            ERole_enum_count
        }

        [Guid( "F4B1A599-7266-4319-A8CA-E70ACB11E8CD" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IAudioSessionControl {

            [PreserveSig]
            Int32 GetDisplayName( [MarshalAs( UnmanagedType.LPWStr )] out String pRetVal );

            Int32 NotImpl1();

            // the rest is not implemented
        }

        [Guid( "E2F5BB11-0570-40CA-ACDD-3AA01277DEE8" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IAudioSessionEnumerator {

            [PreserveSig]
            Int32 GetCount( out Int32 SessionCount );

            [PreserveSig]
            Int32 GetSession( Int32 SessionCount, out IAudioSessionControl Session );
        }

        [Guid( "77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IAudioSessionManager2 {

            [PreserveSig]
            Int32 GetSessionEnumerator( out IAudioSessionEnumerator SessionEnum );

            Int32 NotImpl1();

            Int32 NotImpl2();

            // the rest is not implemented
        }

        [Guid( "D666063F-1587-4E43-81F1-B948E807363F" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IMMDevice {

            [PreserveSig]
            Int32 Activate( ref Guid iid, Int32 dwClsCtx, IntPtr pActivationParams, [MarshalAs( UnmanagedType.IUnknown )] out Object ppInterface );

            // the rest is not implemented
        }

        [Guid( "A95664D2-9614-4F35-A746-DE8DB63617E6" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IMMDeviceEnumerator {

            [PreserveSig]
            Int32 GetDefaultAudioEndpoint( EDataFlow dataFlow, ERole role, out IMMDevice ppDevice );

            Int32 NotImpl1();

            // the rest is not implemented
        }

        [Guid( "87CE5498-68D6-44E5-9215-6DA47EF883D8" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface ISimpleAudioVolume {

            [PreserveSig]
            Int32 GetMasterVolume( out Single pfLevel );

            [PreserveSig]
            Int32 GetMute( out Boolean pbMute );

            [PreserveSig]
            Int32 SetMasterVolume( Single fLevel, ref Guid eventContext );

            [PreserveSig]
            Int32 SetMute( Boolean bMute, ref Guid eventContext );
        }

        public static IEnumerable<String> EnumerateApplications() {

            // get the speakers (1st render + multimedia) device

            // ReSharper disable once SuspiciousTypeConversion.Global
            var deviceEnumerator = new MMDeviceEnumerator() as IMMDeviceEnumerator;
            if ( deviceEnumerator == null ) {
                yield break;
            }
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint( EDataFlow.eRender, ERole.eMultimedia, out speakers );

            // activate the session manager. we need the enumerator
            var IID_IAudioSessionManager2 = typeof( IAudioSessionManager2 ).GUID;
            Object o;
            speakers.Activate( ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o );
            var mgr = o as IAudioSessionManager2;

            // enumerate sessions for on this device
            if ( mgr != null ) {
                IAudioSessionEnumerator sessionEnumerator;
                mgr.GetSessionEnumerator( out sessionEnumerator );
                Int32 count;
                sessionEnumerator.GetCount( out count );

                for ( var i = 0; i < count; i++ ) {
                    IAudioSessionControl ctl;
                    sessionEnumerator.GetSession( i, out ctl );
                    String dn;
                    ctl.GetDisplayName( out dn );
                    yield return dn;
                    Marshal.ReleaseComObject( ctl );
                }
                Marshal.ReleaseComObject( sessionEnumerator );
                Marshal.ReleaseComObject( mgr );
            }
            Marshal.ReleaseComObject( speakers );
            Marshal.ReleaseComObject( deviceEnumerator );
        }

        public static Boolean? GetApplicationMute( String name ) {
            var volume = GetVolumeObject( name );
            if ( volume == null ) {
                return null;
            }

            Boolean mute;
            volume.GetMute( out mute );
            return mute;
        }

        public static Single? GetApplicationVolume( String name ) {
            var volume = GetVolumeObject( name );
            if ( volume == null ) {
                return null;
            }

            Single level;
            volume.GetMasterVolume( out level );
            return level * 100;
        }

        [CanBeNull]
        public static ISimpleAudioVolume GetVolumeObject( String name ) {

            // get the speakers (1st render + multimedia) device

            // ReSharper disable once SuspiciousTypeConversion.Global
            var deviceEnumerator = new MMDeviceEnumerator() as IMMDeviceEnumerator;
            if ( deviceEnumerator == null ) {
                return null;
            }
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint( EDataFlow.eRender, ERole.eMultimedia, out speakers );

            // activate the session manager. we need the enumerator
            var iidIAudioSessionManager2 = typeof( IAudioSessionManager2 ).GUID;
            Object o;
            speakers.Activate( ref iidIAudioSessionManager2, 0, IntPtr.Zero, out o );
            var mgr = ( IAudioSessionManager2 )o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator( out sessionEnumerator );
            Int32 count;
            sessionEnumerator.GetCount( out count );

            // search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume volumeControl = null;
            for ( var i = 0; i < count; i++ ) {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession( i, out ctl );
                String dn;
                ctl.GetDisplayName( out dn );
                if ( String.Compare( name, dn, StringComparison.OrdinalIgnoreCase ) == 0 ) {

                    // ReSharper disable once SuspiciousTypeConversion.Global
                    volumeControl = ctl as ISimpleAudioVolume;
                    break;
                }
                Marshal.ReleaseComObject( ctl );
            }
            Marshal.ReleaseComObject( sessionEnumerator );
            Marshal.ReleaseComObject( mgr );
            Marshal.ReleaseComObject( speakers );
            Marshal.ReleaseComObject( deviceEnumerator );
            return volumeControl;
        }

        public static void SetApplicationMute( String name, Boolean mute ) {
            var volume = GetVolumeObject( name );
            if ( volume == null ) {
                return;
            }

            var guid = Guid.Empty;
            volume.SetMute( mute, ref guid );
        }

        public static void SetApplicationVolume( String name, Single level ) {
            var volume = GetVolumeObject( name );
            if ( volume == null ) {
                return;
            }

            var guid = Guid.Empty;
            volume.SetMasterVolume( level / 100, ref guid );
        }

        [ComImport]
        [Guid( "BCDE0395-E52F-467C-8E3D-C4579291692E" )]
        public class MMDeviceEnumerator {
        }
    }
}