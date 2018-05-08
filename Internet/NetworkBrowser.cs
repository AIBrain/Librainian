// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/NetworkBrowser.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using OperatingSystem;

    /// <summary>
    ///     Provides a mechanism for supplying a list of all PC names in the local network. This
    ///     collection of PC names is used in the form This class makes use of a DllImport instruction.
    ///     The purpose of which is as follows: When a DllImport declaration is made in managed code
    ///     (C#) it is a call to a legacy unmanaged code module, normally a C++ Dynamic Link Library.
    ///     These C++ Dll's are usually part of the operating system API, or some other vendors API, and
    ///     must be used to carry out operations that are not native within the managed code C#
    ///     framework. This is fairly normal within the windows world. The only thing that needs careful
    ///     consideration is the construction of the correct type of STRUCTS, object pointers, and
    ///     attribute markers, which all contribute to making the link between managed (C#) and
    ///     unmanaged code (C++) more seamless This class makes use of the following Dll calls
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 Netapi32.dll : NetServerEnum, The NetServerEnum
    ///                 function lists all servers of the specified type that are visible in a domain. For example,
    ///                 an application can call NetServerEnum to list all domain controllers only or all SQL servers
    ///                 only. You can combine bit masks to list several types. For example, a value of 0x00000003
    ///                 combines the bit masks for SV_TYPE_WORKSTATION (0x00000001) and SV_TYPE_SERVER (0x00000002).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Netapi32.dll : NetApiBufferFree, The
    ///                 NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates.
    ///                 Call NetApiBufferFree to free the memory that other network management functions return.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    public sealed class NetworkBrowser {

        /// <summary>
        ///     Uses the DllImport : NetServerEnum with all its required parameters (see
        ///     http://msdn.microsoft.com/Library/default.asp?
        ///     url=/Library/en-us/netmgmt/netmgmt/netserverenum.asp for full details or method
        ///     signature) to retrieve a list of domain SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
        /// </summary>
        /// <returns>
        ///     Arraylist that represents all the SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's in the Domain
        /// </returns>
        public static IEnumerable<NativeMethods.ServerInfo101> GetNetworkComputers() {

            //local fields
            var networkComputers = new List<NativeMethods.ServerInfo101>();
            const Int32 maxPreferredLength = -1;

            //const int SV_TYPE_WORKSTATION = 1;
            //const int SV_TYPE_SERVER = 2;
            var buffer = IntPtr.Zero;

            //var tmpBuffer = IntPtr.Zero;
            var sizeofInfo = Marshal.SizeOf( typeof( NativeMethods.ServerInfo101 ) );

            try {

                //call the DllImport : NetServerEnum
                //with all its required parameters
                //see http://msdn.microsoft.com/Library/
                //default.asp?url=/Library/en-us/netmgmt/netmgmt/netserverenum.asp
                //for full details of method signature
                var entriesRead = 0;
                var totalEntries = 0;
                var resHandle = new IntPtr( 0 );
                var ret = NativeMethods.NetServerEnum( null, 100, out buffer, maxPreferredLength, ref entriesRead, ref totalEntries, NativeMethods.Sv101Types.SvTypeWorkstation | NativeMethods.Sv101Types.SvTypeServer, null, resHandle );

                //if the returned with a NERR_Success
                //(C++ term), =0 for C#
                if ( 0 == ret ) {

                    //loop through all SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                    for ( var i = 0; i < totalEntries; i++ ) {

                        //get pointer to, Pointer to the
                        //buffer that received the data from
                        //the call to NetServerEnum.
                        //Must ensure to use correct size of
                        //STRUCTURE to ensure correct
                        //location in memory is pointed to
                        var tmpBuffer = new IntPtr( ( Int32 )buffer + i * sizeofInfo );

                        //Have now got a pointer to the list
                        //of SV_TYPE_WORKSTATION and
                        //SV_TYPE_SERVER PC's, which is unmanaged memory
                        //Needs to Marshal data from an
                        //unmanaged block of memory to a
                        //managed object, again using
                        //STRUCTURE to ensure the correct data
                        //is marshalled
                        var svrInfo = Marshal.PtrToStructure( tmpBuffer, typeof( NativeMethods.ServerInfo101 ) );

                        //add the PC names to the ArrayList
                        networkComputers.Add( ( NativeMethods.ServerInfo101 )svrInfo );
                    }
                }
            }
            catch ( Exception ex ) {
                MessageBox.Show( $"Problem with acessing network computers in NetworkBrowser().\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            finally {

                //The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates
                NativeMethods.NetApiBufferFree( buffer );
            }

            //return entries found
            return networkComputers;
        }

        /// <summary>
        ///     <para>Returns a list of servers</para>
        /// </summary>
        /// <example>Debug.WriteLine( entry.Name );</example>
        /// <returns></returns>
        public static IEnumerable<DirectoryEntry> GetServerList() {
            var root = new DirectoryEntry( "WinNT:" );

            return ( from DirectoryEntry entries in root.Children from DirectoryEntry entry in entries.Children select entry ).Where( entry => !entry.Name.Equals( "Schema", StringComparison.Ordinal ) );
        }

        public static IEnumerable<NativeMethods.ServerInfo101> GetServerListAlt( NativeMethods.Sv101Types serverType ) {
            Int32 entriesread = 0, totalentries = 0;
            var alServers = new List<NativeMethods.ServerInfo101>();

            do {

				// Buffer to store the available servers Filled by the NetServerEnum function

				var ret = NativeMethods.NetServerEnum( servername: null, level: 101, bufptr: out var buf, prefmaxlen: -1, entriesread: ref entriesread, totalentries: ref totalentries, servertype: serverType, domain: null, resumeHandle: IntPtr.Zero );

				// if the function returned any data, fill the tree view
				if ( ret == NativeMethods.ErrorSuccess || ret == NativeMethods.ErrorMoreData || entriesread > 0 ) {
                    var ptr = buf;

                    for ( var i = 0; i < entriesread; i++ ) {

                        // cast pointer to a SERVER_INFO_101 structure
                        var server = ( NativeMethods.ServerInfo101 )Marshal.PtrToStructure( ptr, typeof( NativeMethods.ServerInfo101 ) );

                        //Cast the pointer to a UInt64 so this addition will work on 32-bit or 64-bit systems.
                        ptr = ( IntPtr )( ( UInt64 )ptr + ( UInt64 )Marshal.SizeOf( server ) );

                        // add the machine name and comment to the arrayList.
                        //You could return the entire structure here if desired
                        alServers.Add( server );
                    }
                }

                // free the buffer
                NativeMethods.NetApiBufferFree( buf );
            } while ( entriesread < totalentries && entriesread != 0 );

            return alServers;
        }
    }
}