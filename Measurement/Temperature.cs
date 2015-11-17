// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Temperature.cs" was last cleaned by Rick on 2015/06/18 at 7:38 PM

namespace Librainian.Measurement {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// <see cref="Temperature"/> in <see cref="Temperature.Celsius"/>, with properties in <see cref="Fahrenheit"/> and <see cref="Kelvin"/>.
    /// </summary>
    [DataContract( IsReference = true )]
    public sealed class Temperature {

        [DataMember]
        public Single Celsius {
            get; 
        }

        public Single Fahrenheit => this.Celsius * 9 / 5 + 32;

        public Single Kelvin => this.Celsius + 273.15f;

        /// <summary>
        /// <see cref="Temperature"/> in <see cref="Temperature.Celsius"/>, with properties in <see cref="Fahrenheit"/> and <see cref="Kelvin"/>.
        /// </summary>
        public Temperature(Single celsius) {
            this.Celsius = celsius;
        }

        /// <summary>
        /// no no.
        /// </summary>
        private Temperature() {
        }

        public override String ToString() => $"{this.Celsius} °C";
    }
}