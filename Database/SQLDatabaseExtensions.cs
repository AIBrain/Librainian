#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian2/SQLDatabaseExtensions.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM

#endregion License & Information

namespace Librainian.Database {

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Media;
    using System.Net.NetworkInformation;
    using System.Threading;
    using Measurement.Time;
    using Threading;

    public static class SQLDatabaseExtensions {

        /// <summary>
        /// Returns the total time taken for a simple query. (connect + execute + fetch...)
        /// </summary>
        /// <returns></returns>
        [Obsolete( "No access to a local server atm." )]
        public static TimeSpan EasyPing( SQLQuery db ) {
            var stopwatch = Stopwatch.StartNew();
            try {
                var stack = new Stack<Object>();
                db.Params.AddWithValue( "@when", DateTime.Now ).DbType = DbType.DateTime;
                using ( var reader = db.Query( "[dbo].[HalloWrold]" ) ) {
                    while ( reader.Read() ) {
                        for ( var i = 0; i < reader.FieldCount; i++ ) {
                            stack.Push( reader.GetFieldValue<Object>( i ) );
                        }

                        //DateTime wesaid;
                        //DateTime theysaid;
                        //if ( DateTime.TryParse( bob[ 0 ].ToString(), out wesaid ) && DateTime.TryParse( bob[ 1 ].ToString(), out theysaid ) ) {
                        //    var differ = TimeSpan.FromTicks( Math.Abs( theysaid.Ticks - wesaid.Ticks ) );
                        //    //Generic.Report( String.Format( "Database ping replied: {0}.", differ.Simple() ) );
                        //}
                    }
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }

            //Generic.Report( String.Format( "Database ping actual: {0}.", stopwatch.Elapsed.Simple() ) );
            return stopwatch.Elapsed;
        }

        public static Boolean IsNetworkConnected( int retries = 3 ) {
            var counter = retries;
            while ( !NetworkInterface.GetIsNetworkAvailable() && counter > 0 ) {
                --counter;
                Debug.WriteLine( "Network disconnected. Waiting {0}. {1} retries...", Seconds.One, counter );
                Thread.Sleep( 1000 );
            }
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public static void TryPlayFile( this String fileName ) {
            try {
                using ( var player = new SoundPlayer() ) {
                    player.SoundLocation = fileName;
                    player.Load();
                    player.Play();
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }

        /*

                /// <summary>
                /// Returns the total time taken for a simple query. (connect + execute + fetch...)
                /// </summary>
                /// <returns></returns>
                [Obsolete( "No access to a local server atm." )]
                public static TimeSpan Ping() {
                    var stopwatch = Stopwatch.StartNew();
                    try {
                        using ( var query = new SQLQuery( ) ) {
                            query.Params.AddWithValue( "@when", DateTime.Now )
                                 .DbType = DbType.DateTime;
                            using ( var reader = query.Query( "[dbo].[HalloWrold]" ) ) {
                                if ( reader.Read() ) {
                                    DateTime wesaid;
                                    DateTime theysaid;
                                    if ( DateTime.TryParse( reader[ 0 ].ToString(), out wesaid ) && DateTime.TryParse( reader[ 1 ].ToString(), out theysaid ) ) {
                                        var differ = TimeSpan.FromTicks( Math.Abs( theysaid.Ticks - wesaid.Ticks ) );

                                        //Generic.Report( String.Format( "Database ping replied: {0}.", differ.Simple() ) );
                                    }
                                }
                            }
                        }
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }

                    //Generic.Report( String.Format( "Database ping actual: {0}.", stopwatch.Elapsed.Simple() ) );
                    return stopwatch.Elapsed;
                }
        */
        /*
                [Obsolete( "No access to a local server atm." )]
                public static TimeSpan PingAverage() {
                    var stopwatch = Stopwatch.StartNew();
                    var db = new SQLQuery();
                    var bag = new ConcurrentBag< TimeSpan >();
                    do {
                        bag.Add( EasyPing( db ) ); //hack
                    } while ( stopwatch.Elapsed < Second.Three || bag.Count < 10 );

                    //var list = new List<TimeSpan>( bag.Distinct() );
                    var average = bag.Average( timeSpan => timeSpan.TotalMilliseconds );
                    return TimeSpan.FromMilliseconds( average );
                }
        */
    }
}