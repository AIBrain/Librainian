// This file has been modified continuously since Nov 10, 2012 by Brian Pearce.
// Based on http://www.codeproject.com/Articles/137979/Simple-HTTP-Server-in-C

// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske.

// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

namespace Librainian.Internet.Servers {

    using System;

    /// <summary>
    /// An interface which handles logging of exceptions and strings.
    /// </summary>
    public interface ILogger {

        /// <summary>
        /// Log an exception, possibly with additional information provided to assist with debugging.
        /// </summary>
        /// <param name="ex">An exception that was caught.</param>
        /// <param name="additionalInformation">Additional information about the exception.</param>
        void Log( Exception ex, String additionalInformation = "" );

        /// <summary>
        /// Log a String.
        /// </summary>
        /// <param name="str">A String to log.</param>
        void Log( String str );
    }
}