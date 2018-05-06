// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/CodeEngine.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Microsoft.CSharp;
    using NUnit.Framework;
    using Persistence;

    public class CodeEngine {
        public static readonly CSharpCodeProvider CSharpCodeProvider = new CSharpCodeProvider();

        public Action<String> Output = delegate { };

        private readonly Object _oRun = new Object();

        private readonly Object _oSourceCode = new Object();

        private CompilerResults _compilerResults;

        private String _mSourceCode = String.Empty;

        public CodeEngine( String sourcePath, Action<String> output ) : this( Guid.NewGuid(), sourcePath, output ) {
        }

        public CodeEngine( Guid id, String sourcePath, Action<String> output ) {
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

        public Guid ID {
            get; private set;
        }

        public Object[] Parameters {
            get; set;
        }

        public String SourceCode {
            get {
                lock ( this._oSourceCode ) {
                    return this._mSourceCode;
                }
            }

            set {
                lock ( this._oSourceCode ) {
                    this._mSourceCode = value;
                    this.Compile();
                }
            }
        }

        public String SourcePath {
            get;
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
            lock ( this._oRun ) {
                if ( null == this._compilerResults ) {
                    this.Compile();
                }
                if ( null == this._compilerResults ) {
                    return null;
                }

                if ( this._compilerResults.Errors.HasErrors ) {
                    if ( Debugger.IsAttached ) {
                        Debugger.Break();
                    }
                    return null;
                }
                if ( this._compilerResults.Errors.HasWarnings ) {
                    if ( Debugger.IsAttached ) {
                        Debugger.Break();
                    }
                }

                var loAssembly = this._compilerResults.CompiledAssembly;
                var loObject = loAssembly.CreateInstance( "Coding.CodeEngine" );
                if ( loObject is null ) {
                    if ( Debugger.IsAttached ) {
                        Debugger.Break();
                    }
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

        ///// <summary>
        ///// system.windows.forms.dll
        ///// </summary>
        ///// <param name="dllname"></param>
        //public void AddReference( String dllname ) {
        //    this.codeCompileUnit.ReferencedAssemblies.Add( dllname );
        //}

        private static String DefaultCode() => @"
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
                this._compilerResults = CSharpCodeProvider.CompileAssemblyFromSource( new CompilerParameters { GenerateInMemory = true, GenerateExecutable = false }, this.SourceCode );
                if ( this._compilerResults.Errors.HasErrors ) {
                    if ( Debugger.IsAttached ) {
                        Debugger.Break();
                    }
                    return false;
                }
                if ( !this._compilerResults.Errors.HasWarnings ) {
                    return true;
                }
                if ( Debugger.IsAttached ) {
                    Debugger.Break();
                }
                return true;
            }
            catch ( Exception exception ) {
                exception.More();
                return false;
            }
        }
    }
}