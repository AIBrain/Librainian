﻿// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CodeEngine.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/CodeEngine.cs" was last formatted by Protiguous on 2018/05/23 at 6:48 PM.

namespace Librainian.Extensions {

    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;
    using Microsoft.CSharp;
    using NUnit.Framework;
    using Persistence;

    public class CodeEngine {

        public interface IOutput {

            void Output();
        }

        private CompilerResults _compilerResults;

        private String _mSourceCode = String.Empty;

        public Action<String> Output = delegate { };

        private Object ORun { get; } = new Object();

        private Object OSourceCode { get; } = new Object();

        public static CSharpCodeProvider CSharpCodeProvider { get; } = new CSharpCodeProvider();

        public Guid ID { get; private set; }

        public Object[] Parameters { get; set; }

        public String SourceCode {
            get {
                lock ( this.OSourceCode ) { return this._mSourceCode; }
            }

            set {
                lock ( this.OSourceCode ) {
                    this._mSourceCode = value;
                    this.Compile();
                }
            }
        }

        public String SourcePath { get; }

        public CodeEngine( String sourcePath, Action<String> output ) : this( Guid.NewGuid(), sourcePath, output ) { }

        public CodeEngine( Guid id, String sourcePath, Action<String> output ) {
            if ( null != output ) { this.Output = output; }

            //if ( ID.Equals( Guid.Empty ) ) { throw new InvalidOperationException( "Null guid given" ); }
            this.SourcePath = Path.Combine( sourcePath, id + ".cs" );

            if ( !this.Load() ) { this.SourceCode = DefaultCode(); }
        }

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

        ///// <summary>
        ///// system.windows.forms.dll
        ///// </summary>
        ///// <param name="dllname"></param>
        //public void AddReference( String dllname ) {
        //    this.codeCompileUnit.ReferencedAssemblies.Add( dllname );
        //}
        /// <summary>
        ///     Prepare the assembly for Run()
        /// </summary>
        private Boolean Compile() {
            try {
                this._compilerResults = CSharpCodeProvider.CompileAssemblyFromSource( new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false }, this.SourceCode );

                if ( this._compilerResults.Errors.HasErrors ) {
                    Logging.Break();

                    return false;
                }

                if ( !this._compilerResults.Errors.HasWarnings ) { return true; }

                Logging.Break();

                return true;
            }
            catch ( Exception exception ) {
                exception.More();

                return false;
            }
        }

        public static Boolean Test( Action<String> output ) {
            try {
                var test = new CodeEngine( id: Guid.Empty, sourcePath: Path.GetTempPath(), output: output );
                var ooo = test.Run();
                Assert.IsNotNull( ooo );

                return true;
            }
            catch ( Exception exception ) {
                exception.More();

                return false;
            }
        }

        public Boolean Load() => String.IsNullOrEmpty( this.SourceCode );

        public Object Run() {
            lock ( this.ORun ) {
                if ( null == this._compilerResults ) { this.Compile(); }

                if ( null == this._compilerResults ) { return null; }

                if ( this._compilerResults.Errors.HasErrors ) {
                    Logging.Break();

                    return null;
                }

                if ( this._compilerResults.Errors.HasWarnings ) { Logging.Break(); }

                var loAssembly = this._compilerResults.CompiledAssembly;
                var loObject = loAssembly.CreateInstance( "Coding.CodeEngine" );

                if ( loObject is null ) {
                    Logging.Break();

                    return null;
                }

                try {
                    var loResult = loObject.GetType().InvokeMember( "DynamicCode", BindingFlags.InvokeMethod, null, loObject, this.Parameters );

                    return loResult;
                }
                catch ( Exception exception ) {
                    exception.More();

                    return null;
                }
            }
        }

        public Boolean Save() => this.SourceCode.Saver( this.SourcePath );

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