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
// "Librainian/PropertyItem.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Imaging {

    using System;
    using Newtonsoft.Json;

    [JsonObject]
    public sealed class PropertyItem {

        /// <summary>
        ///     Represents the ID of the property.
        /// </summary>
        [JsonProperty]
        public Int32 Id {
            get; set;
        }

        /// <summary>
        ///     Represents the length of the property.
        /// </summary>
        [JsonProperty]
        public Int32 Len {
            get; set;
        }

        /// <summary>
        ///     Represents the type of the property.
        /// </summary>
        [JsonProperty]
        public Int16 Type {
            get; set;
        }

        /// <summary>
        ///     Contains the property value.
        /// </summary>
        [JsonProperty]
        public Byte[] Value {
            get; set;
        }
    }
}