namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential )]
    public struct FILE_TIME {

        public FILE_TIME( Int64 fileTime ) {
            this.ftTimeLow = ( UInt32 )fileTime;
            this.ftTimeHigh = ( UInt32 )( fileTime >> 32 );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Int64 ToTicks() => ( ( Int64 )this.ftTimeHigh << 32 ) + this.ftTimeLow;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public DateTime ToDateTime() => DateTime.FromFileTime( this.ToTicks() );

        public readonly UInt32 ftTimeLow;

        public readonly UInt32 ftTimeHigh;
    }
}