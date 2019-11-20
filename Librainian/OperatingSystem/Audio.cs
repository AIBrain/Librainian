// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Audio.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Audio.cs" was last formatted by Protiguous on 2019/08/08 at 9:13 AM.

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
            if ( !( new MMDeviceEnumerator() is IMMDeviceEnumerator deviceEnumerator ) ) {
                yield break;
            }

            deviceEnumerator.GetDefaultAudioEndpoint( EDataFlow.eRender, ERole.eMultimedia, out var speakers );

            // activate the session manager. we need the enumerator
            var IID_IAudioSessionManager2 = typeof( IAudioSessionManager2 ).GUID;
            speakers.Activate( ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out var o );

            // enumerate sessions for on this device
            if ( o is IAudioSessionManager2 mgr ) {
                mgr.GetSessionEnumerator( out var sessionEnumerator );
                sessionEnumerator.GetCount( out var count );

                for ( var i = 0; i < count; i++ ) {
                    sessionEnumerator.GetSession( i, out var ctl );
                    ctl.GetDisplayName( out var dn );

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

            if ( volume is null ) {
                return null;
            }

            volume.GetMute( out var mute );

            return mute;
        }

        public static Single? GetApplicationVolume( String name ) {
            var volume = GetVolumeObject( name );

            if ( volume is null ) {
                return null;
            }

            volume.GetMasterVolume( out var level );

            return level * 100;
        }

        [CanBeNull]
        public static ISimpleAudioVolume GetVolumeObject( String name ) {

            // get the speakers (1st render + multimedia) device

            // ReSharper disable once SuspiciousTypeConversion.Global
            if ( !( new MMDeviceEnumerator() is IMMDeviceEnumerator deviceEnumerator ) ) {
                return null;
            }

            deviceEnumerator.GetDefaultAudioEndpoint( EDataFlow.eRender, ERole.eMultimedia, out var speakers );

            // activate the session manager. we need the enumerator
            var iidIAudioSessionManager2 = typeof( IAudioSessionManager2 ).GUID;
            speakers.Activate( ref iidIAudioSessionManager2, 0, IntPtr.Zero, out var o );
            var mgr = ( IAudioSessionManager2 )o;

            // enumerate sessions for on this device
            mgr.GetSessionEnumerator( out var sessionEnumerator );
            sessionEnumerator.GetCount( out var count );

            // search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume volumeControl = null;

            for ( var i = 0; i < count; i++ ) {
                sessionEnumerator.GetSession( i, out var ctl );
                ctl.GetDisplayName( out var dn );

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

            if ( volume is null ) {
                return;
            }

            var guid = Guid.Empty;
            volume.SetMute( mute, ref guid );
        }

        public static void SetApplicationVolume( String name, Single level ) {
            var volume = GetVolumeObject( name );

            if ( volume is null ) {
                return;
            }

            var guid = Guid.Empty;
            volume.SetMasterVolume( level / 100, ref guid );
        }

        [ComImport]
        [Guid( "BCDE0395-E52F-467C-8E3D-C4579291692E" )]
        public class MMDeviceEnumerator { }
    }
}