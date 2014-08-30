#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/08/29 at 8:17 PM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Extensions;

    /// <summary>
    ///     Provides a mechanism for supplying a list of all PC names in the local network.
    ///     This collection of PC names is used in the form
    ///     This class makes use of a DllImport instruction.
    ///     The purpose of which is as follows:
    ///     When a DllImport declaration is made
    ///     in managed code (C#) it is a call to a legacy
    ///     unmanaged code module, normally
    ///     a C++ Dynamic Link Library. These C++ Dll's are
    ///     usually part of the operating system API,
    ///     or some other vendors API, and must be
    ///     used to carry out operations that are not
    ///     native within the managed code C# framework.
    ///     This is fairly normal within the windows world.
    ///     The only thing that needs careful consideration
    ///     is the construction of the correct type of STRUCTS,
    ///     object pointers, and attribute markers,
    ///     which all contribute to making the link
    ///     between managed (C#) and unmanaged code (C++)
    ///     more seamless
    ///     This class makes use of the following Dll calls
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Netapi32.dll : NetServerEnum,
    ///                 The NetServerEnum function lists all servers
    ///                 of the specified type that are visible in
    ///                 a domain. For example, an application can call
    ///                 NetServerEnum to list all domain controllers
    ///                 only or all SQL servers only.
    ///                 You can combine bit masks to list several
    ///                 types. For example, a value of 0x00000003
    ///                 combines the bit masks for SV_TYPE_WORKSTATION
    ///                 (0x00000001) and SV_TYPE_SERVER (0x00000002).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Netapi32.dll : NetApiBufferFree,
    ///                 The NetApiBufferFree function frees
    ///                 the memory that the NetApiBufferAllocate
    ///                 function allocates. Call NetApiBufferFree
    ///                 to free the memory that other network
    ///                 management functions return.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    public sealed class NetworkBrowser {

        /// <summary>
        /// <para>Returns a list of servers</para>
        /// </summary>
        /// <example>Debug.WriteLine( entry.Name );</example>
        /// <returns></returns>
        public static IEnumerable<DirectoryEntry> GetServerList() {
            var root = new DirectoryEntry( "WinNT:" );

            return ( from DirectoryEntry entries in root.Children
                     from DirectoryEntry entry in entries.Children
                     select entry ).Where( entry => !entry.Name.Equals( "Schema", StringComparison.Ordinal ) );
        }

        public static ArrayList GetServerListAlt( NativeWin32.SV_101_TYPES ServerType ) {
            int entriesread = 0, totalentries = 0;
            var alServers = new ArrayList();

            do {
                // Buffer to store the available servers
                // Filled by the NetServerEnum function
                IntPtr buf;

                var ret = NativeWin32.NetServerEnum( servername: null, level: 101, bufptr: out buf, prefmaxlen: -1, entriesread: ref entriesread, totalentries: ref totalentries, servertype: ServerType, domain: null, resume_handle: IntPtr.Zero );

                // if the function returned any data, fill the tree view
                if ( ret == NativeWin32.ERROR_SUCCESS || ret == NativeWin32.ERROR_MORE_DATA || entriesread > 0 ) {
                    var ptr = buf;

                    for ( var i = 0 ; i < entriesread ; i++ ) {
                        // cast pointer to a SERVER_INFO_101 structure
                        var server = ( NativeWin32.SERVER_INFO_101 )Marshal.PtrToStructure( ptr, typeof( NativeWin32.SERVER_INFO_101 ) );

                        //Cast the pointer to a ulong so this addition will work on 32-bit or 64-bit systems.
                        ptr = ( IntPtr )( ( ulong )ptr + ( ulong )Marshal.SizeOf( server ) );

                        // add the machine name and comment to the arrayList. 
                        //You could return the entire structure here if desired
                        alServers.Add( server );
                    }
                }

                // free the buffer 
                NativeWin32.NetApiBufferFree( buf );

            }
            while ( entriesread < totalentries && entriesread != 0 );

            return alServers;
        }

        /// <summary>
        ///     Uses the DllImport : NetServerEnum
        ///     with all its required parameters
        ///     (see http://msdn.microsoft.com/library/default.asp?
        ///     url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
        ///     for full details or method signature) to
        ///     retrieve a list of domain SV_TYPE_WORKSTATION
        ///     and SV_TYPE_SERVER PC's
        /// </summary>
        /// <returns>
        ///     Arraylist that represents
        ///     all the SV_TYPE_WORKSTATION and SV_TYPE_SERVER
        ///     PC's in the Domain
        /// </returns>
        public static IEnumerable<NativeWin32.SERVER_INFO_101> getNetworkComputers() {
            //local fields
            var networkComputers = new List<NativeWin32.SERVER_INFO_101>();
            const int MAX_PREFERRED_LENGTH = -1;
            //const int SV_TYPE_WORKSTATION = 1;
            //const int SV_TYPE_SERVER = 2;
            var buffer = IntPtr.Zero;
            //var tmpBuffer = IntPtr.Zero;
            var sizeofINFO = Marshal.SizeOf( typeof( NativeWin32.SERVER_INFO_101 ) );

            try {
                //call the DllImport : NetServerEnum 
                //with all its required parameters
                //see http://msdn.microsoft.com/library/
                //default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
                //for full details of method signature
                var entriesRead = 0;
                var totalEntries = 0;
                var resHandle = new IntPtr( 0 );
                var tmpBuffer = new IntPtr( 0 );
                var ret = NativeWin32.NetServerEnum( null, 100, out buffer, MAX_PREFERRED_LENGTH, ref entriesRead, ref totalEntries, NativeWin32.SV_101_TYPES.SV_TYPE_WORKSTATION | NativeWin32.SV_101_TYPES.SV_TYPE_SERVER, null, resHandle );
                //if the returned with a NERR_Success 
                //(C++ term), =0 for C#
                if ( 0 == ret ) {
                    //loop through all SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                    for ( var i = 0 ; i < totalEntries ; i++ ) {
                        //get pointer to, Pointer to the 
                        //buffer that received the data from
                        //the call to NetServerEnum. 
                        //Must ensure to use correct size of 
                        //STRUCTURE to ensure correct 
                        //location in memory is pointed to
                        tmpBuffer = new IntPtr( ( int )buffer + ( i * sizeofINFO ) );
                        //Have now got a pointer to the list 
                        //of SV_TYPE_WORKSTATION and 
                        //SV_TYPE_SERVER PC's, which is unmanaged memory
                        //Needs to Marshal data from an 
                        //unmanaged block of memory to a 
                        //managed object, again using 
                        //STRUCTURE to ensure the correct data
                        //is marshalled 
                        var svrInfo = Marshal.PtrToStructure( tmpBuffer, typeof( NativeWin32.SERVER_INFO_101 ) );

                        //add the PC names to the ArrayList
                        networkComputers.Add( ( NativeWin32.SERVER_INFO_101 )svrInfo );
                    }
                }
            }
            catch ( Exception ex ) {
                MessageBox.Show( string.Format( "Problem with acessing network computers in NetworkBrowser().\r\n{0}", ex.Message ), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally {
                //The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates
                NativeWin32.NetApiBufferFree( buffer );
            }
            //return entries found
            return networkComputers;
        }

        //[ StructLayout( LayoutKind.Sequential ) ]
        //public struct _SERVER_INFO_100 {
        //    internal int sv100_platform_id;
        //    [ MarshalAs( UnmanagedType.LPWStr ) ] internal string sv100_name;
        //}
    }
}
