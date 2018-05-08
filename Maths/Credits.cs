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
// "Librainian/Credits.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Keep count of credits, current and lifetime.</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Credits {

        /// <summary>No credits.</summary>
        public static readonly Credits Zero = new Credits( currentCredits: 0, lifetimeCredits: 0 );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private UInt64 _currentCredits;

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private UInt64 _lifetimeCredits;

        public Credits( UInt64 currentCredits = 0, UInt64 lifetimeCredits = 0 ) {
            this.CurrentCredits = currentCredits;
            this.LifetimeCredits = lifetimeCredits;
        }

        public UInt64 CurrentCredits {
            get => Thread.VolatileRead( ref this._currentCredits );

	        private set => Thread.VolatileWrite( ref this._currentCredits, value );
        }

        public UInt64 LifetimeCredits {
            get => Thread.VolatileRead( ref this._lifetimeCredits );

	        private set => Thread.VolatileWrite( ref this._lifetimeCredits, value );
        }

        public static Credits Combine( [NotNull] Credits left, [NotNull] Credits right ) {
            if ( left is null ) {
                throw new ArgumentNullException( nameof( left ) );
            }
            if ( right is null ) {
                throw new ArgumentNullException( nameof( right ) );
            }
            return new Credits( left.CurrentCredits + right.CurrentCredits, left.LifetimeCredits + right.LifetimeCredits );
        }

        public void AddCredits( UInt64 credits = 1 ) {
            this.CurrentCredits += credits;
            this.LifetimeCredits += credits;
        }

        public Credits Clone() => new Credits( this.CurrentCredits, this.LifetimeCredits );

        public void SubtractCredits( UInt64 credits = 1 ) {
            var currentcredits = ( Int64 )this.CurrentCredits;
            if ( currentcredits - ( Int64 )credits < 0 ) {
                this.CurrentCredits = 0;
            }
            else {
                this.CurrentCredits -= credits;
            }
        }

        public override String ToString() => $"{this.CurrentCredits:N0} credits ({this.LifetimeCredits:N0} lifetime credits).";
    }
}