namespace Librainian.Measurement.Time.Clocks {
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    class ClockExtensions {

        /// <summary>
        ///    <para>Returns a new decremented <seealso cref="Hour"/>.</para>
        ///     <para>Returns true if the value passed <see cref="Hour.Minimum" /></para>
        /// </summary>
        public static Boolean TickBackward( ref Hour hour ) {
            var value = ( SByte )hour;
            value--;
            if ( value < Hour.Minimum ) {
                hour = Hour.MaxHour;
                return true;
            }
            hour = new Hour( value );
            return false;
        }

        /// <summary>
        ///     <para>Returns a new incremented <seealso cref="Hour"/>.</para>
        ///     <para>Returns true if the value passed <see cref="Hour.Maximum" /></para>
        /// </summary>
        public static Boolean TickForward( ref Hour hour ) {
            var value = ( Byte )hour;
            value++;
            if ( value > Hour.Maximum ) {
                hour = Hour.MinHour;
                return true;
            }
            hour = new Hour( value );
            return false;
        }

        /// <summary>
        ///     <para>Returns a new incremented <seealso cref="Hour"/>.</para>
        ///     <para>Returns true if the value passed <see cref="Minute.Maximum" /></para>
        /// </summary>
        public Boolean TickBackward( ref Minute minute ) {
            var value = ( SByte )minute;
            value--;
            if ( value < Minute.Minimum ) {
                minute = new Minute( Minute.Maximum );
                return true;
            }
            minute = new Minute( value );
            return false;
        }

        public void Set( Byte value ) {
            this.Value = value;
        }

        /// <summary>
        ///     <para>Increase the current minute.</para>
        ///     <para>Returns true if the value passed <see cref="Maximum" /></para>
        /// </summary>
        public Boolean Tick() {
            var value = this.Value;
            value++;
            if ( value > Maximum ) {
                this.Value = Minimum;
                return true;
            }
            this.Value = value;
            return false;
        }

        /// <summary>
        ///     Decrease the current second.
        /// </summary>
        public Boolean Rewind() {
            var value = ( Int16 )this.Value;
            value--;
            if ( value < Minimum ) {
                this.Value = Maximum;
                return true;
            }
            this.Value = ( Byte )value;
            return false;
        }

        /// <summary>
        ///     Increase the current second.
        /// </summary>
        public Boolean Tick() {
            var value = this.Value;
            value++;
            if ( value > Maximum ) {
                this.Value = Minimum;
                return true;
            }
            this.Value = value;
            return false;
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
        private static void Second() {
            Clocks.Second.Maximum.Should().BeGreaterThan( Clocks.Second.Minimum );
        }
    }
}
