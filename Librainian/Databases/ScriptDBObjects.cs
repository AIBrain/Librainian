// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ScriptDBObjects.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "ScriptDBObjects.cs" was last formatted by Protiguous on 2020/01/31 at 12:24 AM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.SqlServer.Management.Smo;

    public static class ScriptDBObjects {

        public static void BackupDB( [CanBeNull] Server srv, [CanBeNull] Database db, [CanBeNull] DirectoryInfo saveFolder ) {

            var dboDict = new ConcurrentDictionary<String, IEnumerable<ScriptSchemaObjectBase>>();

            Parallel.Invoke( () => dboDict.TryAdd( nameof( db.Views ), db.Views.Cast<View>().Where( x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( nameof( db.Tables ), db.Tables.Cast<Table>().Where( x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( nameof( db.UserDefinedFunctions ), db.UserDefinedFunctions.Cast<UserDefinedFunction>().Where( x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( nameof( db.StoredProcedures ), db.StoredProcedures.Cast<StoredProcedure>().Where( x => !x.IsSystemObject ) ) );

            Parallel.ForEach( dboDict, dict => {
                var objPath = $@"{saveFolder.FullName}\{dict.Key}\";

                try {
                    if ( !Directory.Exists( objPath ) ) {
                        Directory.CreateDirectory( objPath );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();

                    return;
                }

                Parallel.ForEach( dict.Value, obj => {

                    var objFile = $"{objPath}{obj.Schema}.{obj.Name}.sql";
                    var scriptString = GetScriptString( srv, obj );

                    try {
                        File.WriteAllText( objFile, scriptString );
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                } );
            } );
        }

        [NotNull]
        public static String GetScriptString( [CanBeNull] Server server, [CanBeNull] SqlSmoObject obj ) {
            var output = new StringBuilder();
            var scr = new Scripter( server );

            var script = scr.EnumScript( new[] {
                obj
            } );

            foreach ( var line in script ) {
                output.AppendLine( line );
            }

            return output.ToString();
        }
    }
}