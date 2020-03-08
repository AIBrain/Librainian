// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CodeEngine.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "CodeEngine.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Extensions {

    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;
    using JetBrains.Annotations;
    using Logging;
    using Microsoft.CSharp;
    using Persistence;

    /// <summary>
    /// TODO this engine may need revisted.
    /// </summary>
    public class CodeEngine {

        [CanBeNull]
        private CompilerResults _compilerResults;

        [CanBeNull]
        private String _mSourceCode = String.Empty;

        [NotNull]
        private Object _run { get; } = new Object();

        [NotNull]
        private Object _sourceCode { get; } = new Object();

        [NotNull]
        public static CSharpCodeProvider CSharpCodeProvider { get; } = new CSharpCodeProvider();

        public Guid ID { get; private set; }

        public Action<String> Output { get; } = delegate { };

        [CanBeNull]
        public Object[] Parameters { get; set; }

        [CanBeNull]
        public String SourceCode {
            get {
                lock ( this._sourceCode ) {
                    return this._mSourceCode;
                }
            }

            set {
                lock ( this._sourceCode ) {
                    this._mSourceCode = value;
                    this.Compile();
                }
            }
        }

        [CanBeNull]
        public String? SourcePath { get; }

        public CodeEngine( [NotNull] String sourcePath, [CanBeNull] Action<String> output ) : this( Guid.NewGuid(), sourcePath, output ) { }

        public CodeEngine( Guid id, [NotNull] String sourcePath, [CanBeNull] Action<String> output ) {
            if ( null != output ) {
                this.Output = output;
            }

            //if ( ID.Equals( Guid.Empty ) ) { throw new InvalidOperationException( "Null guid given" ); }
            this.SourcePath = Path.Combine( sourcePath, id + ".cs" );

            if ( !this.Load() ) {
                this.SourceCode = DefaultCode();
            }
        }

        public interface IOutput {

            void Output();
        }

        [NotNull]
        private static String DefaultCode() =>
            @"
using System;
using Libranian;

namespace Coding
{
    public class CodeEngine
    {
        private Action<String> Output = delegate { };

        public object DynamicCode(params object[] Parameters)
        {
            Output(""Hello from dynamic code!"");
            return 0;
        }
    }
}";

        /// <summary>Prepare the assembly for Run()</summary>
        private Boolean Compile() {
            try {
                this._compilerResults = CSharpCodeProvider.CompileAssemblyFromSource( new CompilerParameters {
                    GenerateInMemory = true,
                    GenerateExecutable = false
                }, this.SourceCode );

                if ( this._compilerResults.Errors?.HasErrors == true ) {
                    "".Break();

                    return default;
                }

                if ( !this._compilerResults.Errors?.HasWarnings == true ) {
                    return true;
                }

                "".Break();

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();

                return default;
            }
        }

        public static Boolean Test( [CanBeNull] Action<String> output ) {
            try {
                var test = new CodeEngine( Guid.Empty, Path.GetTempPath(), output );
                test.Run();

                return true;
            }
            catch ( Exception exception ) {
                exception.Log();

                return default;
            }
        }

        public Boolean Load() => String.IsNullOrEmpty( this.SourceCode );

        [CanBeNull]
        public Object Run() {
            lock ( this._run ) {
                if ( null == this._compilerResults ) {
                    this.Compile();
                }

                if ( null == this._compilerResults ) {
                    return null;
                }

                if ( this._compilerResults.Errors?.HasErrors == true ) {
                    "".Break();

                    return null;
                }

                if ( this._compilerResults.Errors?.HasWarnings == true ) {
                    "".Break();
                }

                var loAssembly = this._compilerResults.CompiledAssembly;
                var loObject = loAssembly?.CreateInstance( "Coding.CodeEngine" );

                if ( loObject is null ) {
                    "".Break();

                    return null;
                }

                try {
                    var loResult = loObject.GetType().InvokeMember( "DynamicCode", BindingFlags.InvokeMethod, null, loObject, this.Parameters );

                    return loResult;
                }
                catch ( Exception exception ) {
                    exception.Log();

                    return null;
                }
            }
        }

        public Boolean Save() {
            var sourcePath = this.SourcePath;

            return sourcePath != null && this.SourceCode.Saver( sourcePath );
        }

        //private CodeCompileUnit codeCompileUnit;
        //private CodeNamespace codeNamespace;

        ///// <summary>
        ///// Clears all internal code for this CodeEngine
        ///// </summary>
        //public void Init() {
        //    this.codeCompileUnit = new CodeCompileUnit();
        //    this.codeNamespace = new CodeNamespace( "AIBrain" );
        //    this.codeCompileUnit.Namespaces.Add( this.codeNamespace );
        //}
    }
}