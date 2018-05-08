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
// "Librainian/Temperature.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     <see cref="Temperature" /> in <see cref="Temperature.Celsius" />, with properties in <see cref="Fahrenheit" /> and
    ///     <see cref="Kelvin" />.
    /// </summary>
    [JsonObject]
    [Immutable]
    public sealed class Temperature {

        /// <summary>
        ///     <see cref="Temperature" /> in <see cref="Temperature.Celsius" />, with properties in <see cref="Fahrenheit" /> and
        ///     <see cref="Kelvin" />.
        /// </summary>
        public Temperature( Single celsius ) => this.Celsius = celsius;

	    /// <summary>
        ///     no no.
        /// </summary>
        private Temperature() {
        }

        [JsonProperty]
        public Single Celsius {
            get;
        }

        public Single Fahrenheit => this.Celsius * 9 / 5 + 32;

        public Single Kelvin => this.Celsius + 273.15f;

        public override String ToString() => $"{this.Celsius} °C";
    }
}