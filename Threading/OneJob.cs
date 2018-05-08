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
// "Librainian/OneJob.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using Maths.Numbers;

    public class OneJob : IComparable<OneJob> {

        public OneJob( Single priority, Action action ) {
            this.Priority = priority;
            this.Action = action;
        }

        public Action Action {
            get;
        }

        public Percentage Priority {
            get;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return
        ///     value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the
        ///     <paramref name="other" /> parameter.Zero This object is equal to
        ///     <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Int32 CompareTo( OneJob other ) => this.Priority.CompareTo( other.Priority );
    }
}