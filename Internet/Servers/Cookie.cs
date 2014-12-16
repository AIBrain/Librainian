namespace Librainian.Internet.Servers {
    using System;

    public class Cookie {
        public TimeSpan expire;
        public String name;
        public String value;

        public Cookie( String name, String value, TimeSpan expire ) {
            this.name = name;
            this.value = value;
            this.expire = expire;
        }
    }
}