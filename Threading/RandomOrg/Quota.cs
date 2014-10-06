namespace Librainian.Threading.RandomOrg {
    using System.Net;

    public static class Quota {
        private const string Unexpected = "Error: unexpected data.";

        public const long Error = long.MinValue;

        public static long Check() {
            long result;
            return long.TryParse( WebInterface.GetWebPage( "http://www.random.org/quota/?format=plain" ), out result ) ? result : Error;
        }

        public static long Check( IPAddress ip ) {
            long result;
            return long.TryParse( WebInterface.GetWebPage( string.Format( "http://www.random.org/quota/?ip={0}&format=plain", ip ) ), out result ) ? result : Error;
        }
    }
}