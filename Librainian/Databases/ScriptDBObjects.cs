// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ScriptDBObjects.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "ScriptDBObjects.cs" was last formatted by Protiguous on 2020/03/16 at 4:39 PM.

namespace Librainian.Databases {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.SqlServer.Management.Smo;
    using OperatingSystem.FileSystem.Pri.LongPath;
    using Directory = System.IO.Directory;
    using File = System.IO.File;

    public static class ScriptDBObjects {

        public static void BackupDB( [CanBeNull] Server srv, [CanBeNull] Database db, [CanBeNull] DirectoryInfo saveFolder ) {

            var dboDict = new ConcurrentDictionary<String, IEnumerable<ScriptSchemaObjectBase>>();

            Parallel.Invoke( () => dboDict.TryAdd( key: nameof( db.Views ), value: db.Views.Cast<View>().Where( predicate: x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( key: nameof( db.Tables ), value: db.Tables.Cast<Table>().Where( predicate: x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( key: nameof( db.UserDefinedFunctions ),
                    value: db.UserDefinedFunctions.Cast<UserDefinedFunction>().Where( predicate: x => !x.IsSystemObject ) ),
                () => dboDict.TryAdd( key: nameof( db.StoredProcedures ), value: db.StoredProcedures.Cast<StoredProcedure>().Where( predicate: x => !x.IsSystemObject ) ) );

            Parallel.ForEach( source: dboDict, body: dict => {
                var objPath = $@"{saveFolder.FullPath}\{dict.Key}\";

                try {
                    if ( !Directory.Exists( path: objPath ) ) {
                        Directory.CreateDirectory( path: objPath );
                    }
                }
                catch ( Exception exception ) {
                    exception.Log();

                    return;
                }

                Parallel.ForEach( source: dict.Value, body: obj => {

                    var objFile = $"{objPath}{obj.Schema}.{obj.Name}.sql";
                    var scriptString = GetScriptString( server: srv, obj: obj );

                    try {
                        File.WriteAllText( path: objFile, contents: scriptString );
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
            var scr = new Scripter( svr: server );

            var script = scr.EnumScript( objects: new[] {
                obj
            } );

            foreach ( var line in script ) {
                output.AppendLine( value: line );
            }

            return output.ToString();
        }

    }

}