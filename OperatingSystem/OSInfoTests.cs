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
// "Librainian/Class1.cs" was last cleaned by Rick on 2016/07/28 at 4:20 AM

namespace Librainian.OperatingSystem {

    using System.Diagnostics;
    using NUnit.Framework;

    [TestFixture]
    public static class OSInfoTests {

        [Test]
        public static void TestVersions() {
            Debug.WriteLine( $"Server: {OSInfo.IsServer()}" );
            Debug.WriteLine( $"Version Major: {OSInfo.WinMajorVersion()}" );
            Debug.WriteLine( $"Version Minor: {OSInfo.WinMinorVersion()}" );
            Debug.WriteLine( $"Build Major: {OSInfo.BuildMajor()}" );
            Debug.WriteLine( $"Build Minor: {OSInfo.BuildMinor()}" );
            Debug.WriteLine( $"BuildBranch: {OSInfo.BuildBranch()}" );
            Debug.WriteLine( $"Release ID: {OSInfo.ReleaseID()}" );
        }

    }

}
