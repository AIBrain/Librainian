// Copyright 2018 Protiguous.
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
// "Librainian/Class1.cs" was last cleaned by Protiguous on 2016/07/28 at 4:20 AM

namespace Librainian.OperatingSystem {

    using System.Diagnostics;
    using NUnit.Framework;

    [TestFixture]
    public static class OSInfoTests {

        [Test]
        public static void TestVersions() {
            Debug.WriteLine( $"Server: {Info.IsServer()}" );
            Debug.WriteLine( $"Version Major: {Info.VersionMajor()}" );
            Debug.WriteLine( $"Version Minor: {Info.VersionMinor()}" );
            Debug.WriteLine( $"Build Major: {Info.BuildMajor()}" );
            Debug.WriteLine( $"Build Minor: {Info.BuildMinor()}" );
            Debug.WriteLine( $"BuildBranch: {Info.BuildBranch()}" );
            Debug.WriteLine( $"Release ID: {Info.ReleaseId()}" );
        }

    }

}
