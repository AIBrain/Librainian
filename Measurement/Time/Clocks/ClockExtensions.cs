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
// "Librainian/ClockExtensions.cs" was last cleaned by Rick on 2014/08/11 at 10:45 PM
#endregion

namespace Librainian.Measurement.Time.Clocks {
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    public static class ClockExtensions {


        /// <summary>
        ///     <para>Returns a new decremented <seealso cref="Hour" />.</para>
        ///     <para>Returns true if the value passed <see cref="Hour.Minimum" /></para>
        /// </summary>
        public static Boolean Backward( this Hour hour ) {
            var value = ( SByte )hour;
            value--;
            if ( value < Hour.Minimum ) { hour = Hour.MaxHour; return true; }
            hour = new Hour( value );
            return false;
        }

        /// <summary>
        ///     <para>Returns a new incremented <seealso cref="Hour" />.</para>
        ///     <para>Returns true if the value passed <see cref="Hour.Maximum" /></para>
        /// </summary>
        public static Boolean Forward( this Hour hour ) {
            var value = ( Byte )hour;
            value++;
            if ( value > Hour.Maximum ) { hour = Hour.MinHour; return true; }
            hour = new Hour( value );
            return false;
        }


        /// <summary>
        ///     <para>Returns a new decremented <see cref="Minute" />.</para>
        ///     <para>Returns true if the value passed <see cref="Minute.Maximum" /></para>
        /// </summary>
        public static Boolean Backward( this Minute minute, out Boolean tocked ) {
            var value = ( SByte )minute;
            value--;
            if ( value < Minute.Minimum ) {
                tocked = true;
                return new Minute( Minute.Maximum );
                
            }
            minute = new Minute( value );
            return false;
        }

        /// <summary>
        ///     <para>Increase the current minute.</para>
        ///     <para>Returns true if the value passed <see cref="Minute.Maximum" /></para>
        /// </summary>
        public static Minute Forward( this Minute minute, out Boolean tocked ) {
            var value = ( SByte )minute;
            value++;
            if ( value > Minute.Maximum ) {
                tocked = true;
                return Minute.MinMinute;
            }
            tocked = false;
            return new Minute( value );

        }

        /// <summary>
        ///     <para>Returns a new decremented <seealso cref="Second" />.</para>
        ///     <para>Returns true if the value passed <see cref="Second.Minimum" /></para>
        /// </summary>
        public static Second Backward( this PartofaClock second, out Boolean tocked ) {
            var value = ( int )second.Value;
            value--;
            if ( value < Second.Minimum ) {
                tocked = true;
                return Second.MaxSecond;
            }
            tocked = false;
            return new Second( value );
        }

        /// <summary>
        ///     <para>Returns a new incremented <seealso cref="Second" />.</para>
        ///     <para>Returns true if the value passed <see cref="Second.Maximum" /></para>
        /// </summary>
        public static T Forward<T>( this PartofaClock second, out Boolean tocked ) where T : new() {
            var value = ( int )second.Value;
            value++;
            if ( value > Second.Maximum ) {
                tocked = true;
                return new T();
            }
            tocked = false;
            return new PartofaClock( value );
        }


        [Test]
        public static void TestHour() {
            Hour.Minimum.Should().BeLessThan( Hour.Maximum );
            Hour.Maximum.Should().BeGreaterThan( Hour.Minimum );
        }

        [Test]
        public static void TestMinute() {
            Minute.Minimum.Should().BeLessThan( Minute.Maximum );
            Minute.Maximum.Should().BeGreaterThan( Minute.Minimum );
        }

        [Test]
        private static void TestSecond() {
            Second.Maximum.Should().BeGreaterThan( Second.Minimum );
        }
    }
}
