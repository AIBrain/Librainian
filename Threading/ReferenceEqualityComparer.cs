namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;

    public class ReferenceEqualityComparer : EqualityComparer< Object > {

        public override Boolean Equals( Object x, Object y ) { return ReferenceEquals( x, y ); }

        public override Int32 GetHashCode( Object obj ) { return obj?.GetHashCode() ?? 0; }

    }

}