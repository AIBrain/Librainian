namespace Librainian.Threading {
    using System;

    /// <summary>
    /// Thrown when the actor fails.
    /// </summary>
    /// <seealso cref="Actor"/>
    public class ActorException : Exception {

        public ActorException( String because ) {
            this.Reason = because;
        }

        public String Reason { get; private set; }
    }
}