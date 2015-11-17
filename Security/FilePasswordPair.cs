namespace Librainian.Security {
    using System;

    public struct FilePasswordPair {
        public String FileName {
            get;
        }
        public String Password {
            get;
        }

        public FilePasswordPair( String fileName, String password ) {
            this.FileName = fileName;
            this.Password = password;
        }
    }
}