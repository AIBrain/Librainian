
namespace Librainian.Misc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    namespace SharpEssentials {
        /// <summary>
        /// Pulled from https://github.com/PaddiM8/SharpEssentials/blob/master/SharpEssentials/SharpEssentials/PlaySound.cs
        /// </summary>
        /// <remarks>Untested! Possibility won't work in 64 bit.</remarks>
        public static class PlaySound {

            private static String _command;

            private static Boolean _isOpen;

            [SuppressMessage( "Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1" )]
            [SuppressMessage( "Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0" )]
            [SuppressMessage( "Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "return" )]
            [SuppressMessage( "Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass" )]
            [DllImport( "winmm.dll" ) ]
            private static extern Int64 mciSendString( String strCommand, StringBuilder strReturn, Int32 iReturnLength, IntPtr hwndCallback );

            public static void Stop() {
                _command = "close MediaFile";
                mciSendString( _command, null, 0, IntPtr.Zero );
                _isOpen = false;
            }

            public static async Task Start( String sFileName ) {
                _command = $"open \"{sFileName}\" type mpegvideo alias MediaFile";
                mciSendString( _command, null, 0, IntPtr.Zero );
                _isOpen = true;

                if ( _isOpen ) {
                    _command = "play MediaFile";

                    //if (loop)
                    //    _command += " REPEAT";
                    mciSendString( _command, null, 0, IntPtr.Zero );
                }
                await Task.Delay( GetSoundLength( sFileName ) ).ConfigureAwait( false );
                Stop();
            }

            public static Int32 GetSoundLength( String fileName ) {
                var lengthBuf = new StringBuilder( 32 );

                mciSendString( $"open \"{fileName}\" type waveaudio alias wave", null, 0, IntPtr.Zero );
                mciSendString( "status wave length", lengthBuf, lengthBuf.Capacity, IntPtr.Zero );
                mciSendString( "close wave", null, 0, IntPtr.Zero );

				Int32.TryParse( lengthBuf.ToString(), out var length );

				return length;
            }

        }

    }

}
