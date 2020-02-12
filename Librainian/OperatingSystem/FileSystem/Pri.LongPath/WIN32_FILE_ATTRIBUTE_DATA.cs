namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.IO;

    [Serializable]
    public struct WIN32_FILE_ATTRIBUTE_DATA {

        public FileAttributes fileAttributes;

        public Int32 fileSizeHigh;

        public Int32 fileSizeLow;

        public UInt32 ftCreationTimeHigh;

        public UInt32 ftCreationTimeLow;

        public UInt32 ftLastAccessTimeHigh;

        public UInt32 ftLastAccessTimeLow;

        public UInt32 ftLastWriteTimeHigh;

        public UInt32 ftLastWriteTimeLow;

        public void PopulateFrom( WIN32_FIND_DATA findData ) {
            this.fileAttributes = findData.dwFileAttributes;
            this.ftCreationTimeLow = ( UInt32 )findData.ftCreationTime.dwLowDateTime;
            this.ftCreationTimeHigh = ( UInt32 )findData.ftCreationTime.dwHighDateTime;
            this.ftLastAccessTimeLow = ( UInt32 )findData.ftLastAccessTime.dwLowDateTime;
            this.ftLastAccessTimeHigh = ( UInt32 )findData.ftLastAccessTime.dwHighDateTime;
            this.ftLastWriteTimeLow = ( UInt32 )findData.ftLastWriteTime.dwLowDateTime;
            this.ftLastWriteTimeHigh = ( UInt32 )findData.ftLastWriteTime.dwHighDateTime;
            this.fileSizeHigh = findData.nFileSizeHigh;
            this.fileSizeLow = findData.nFileSizeLow;
        }
    }
}