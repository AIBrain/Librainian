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
// "Librainian/ClockExtensions.cs" was last cleaned by Rick on 2014/08/12 at 7:00 AM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using FluentAssertions;
    using NUnit.Framework;

    public static class ClockExtensions {

        [Test]
        public static void TestHour() {
            Hour.Min.Value.Should().BeLessThan( Hour.Max.Value );
            Hour.Max.Value.Should().BeGreaterThan( Hour.Min.Value );
        }

        [Test]
        public static void TestMinute() {
            Minute.Min.Value.Should().BeLessThan( Minute.Max.Value );
            Minute.Max.Value.Should().BeGreaterThan( Minute.Min.Value );
        }

        [Test]
        private static void TestSecond() {
            Second.Min.Value.Should().BeLessThan( Second.Max.Value );
            Second.Max.Value.Should().BeGreaterThan( Second.Min.Value );
        }
    }
}
