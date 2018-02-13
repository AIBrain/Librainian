namespace Librainian.FileSystem {

    using System;

    /// <summary>
    /// Any result less than 1 is an error of some sort.
    /// </summary>
    public enum ResultCode : Int32 {

        FailureUnknown = -11,

        FailureUnableToSetLastWriteTime = -10,

        FailureUnableToSetLastAccessTime=-9,

        FailureUnableToDeleteSourceDocument=-8,

        FailureSourceDoesNotExist=-7,

        FailureSourceIsEmpty=-6,

        FailureOnCopy=-5,

        FailureDestinationDoesNotExist=-4,

        FailureDestinationSizeIsDifferent = -3,

        FailureUnableToSetFileAttributes = -2,

        FailureUnableToSetFileCreationTime = -1,

        Success = 1,

    }

}