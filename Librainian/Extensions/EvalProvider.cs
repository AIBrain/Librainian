// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "EvalProvider.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "EvalProvider.cs" was last formatted by Protiguous on 2020/01/31 at 12:25 AM.

namespace Librainian.Extensions {

    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JetBrains.Annotations;
    using Microsoft.CSharp;

    /// <summary>
    ///     <para>Pulled from <see cref="http://www.ckode.dk/programming/eval-in-c-yes-its-possible/" />.</para>
    /// </summary>
    public static class EvalProvider {

        [NotNull]
        private static String GetUsing( [NotNull] IEnumerable<String> usingStatements ) {
            var result = new StringBuilder();

            foreach ( var usingStatement in usingStatements ) {
                result.AppendLine( $"using {usingStatement};" );
            }

            return result.ToString();
        }

        /// <summary>Example:
        /// <para>var HelloWorld = EvalProvider.CreateEvalMethod&lt;Int32, string&gt;(@"return ""Hello world "" + arg;");</para>
        /// <para>Console.WriteLine(HelloWorld(42));</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="code">           </param>
        /// <param name="usingStatements"></param>
        /// <param name="assemblies">     </param>
        /// <returns></returns>
        [NotNull]
        public static Func<T, TResult> CreateEvalMethod<T, TResult>( [CanBeNull] String code, [CanBeNull] String[] usingStatements = null,
            [CanBeNull] String[] assemblies = null ) {
            var returnType = typeof( TResult );
            var inputType = typeof( T );

            var includeUsings = new HashSet<String>( new[] {
                "System"
            } ) {
                returnType.Namespace, inputType.Namespace
            };

            if ( usingStatements != null ) {
                foreach ( var usingStatement in usingStatements ) {
                    includeUsings.Add( usingStatement );
                }
            }

            using ( var compiler = new CSharpCodeProvider() ) {
                var includeAssemblies = new HashSet<String>( new[] {
                    "system.dll"
                } );

                if ( assemblies != null ) {
                    foreach ( var assembly in assemblies ) {
                        includeAssemblies.Add( assembly );
                    }
                }

                var name = "F" + Guid.NewGuid().ToString().Replace( "-", String.Empty );

                var source = $@"
{GetUsing( includeUsings )}
namespace {name}
{{
	public static class EvalClass
	{{
		public static {returnType.Name} Eval({inputType.Name} arg)
		{{
			{code}
		}}
	}}
}}";

                var parameters = new CompilerParameters( includeAssemblies.ToArray() ) {
                    GenerateInMemory = true
                };

                var compilerResult = compiler.CompileAssemblyFromSource( parameters, source );
                var compiledAssembly = compilerResult.CompiledAssembly;
                var type = compiledAssembly.GetType( $"{name}.EvalClass" );
                var method = type.GetMethod( "Eval" );

                return ( Func<T, TResult> ) Delegate.CreateDelegate( typeof( Func<T, TResult> ), method ?? throw new InvalidOperationException() );
            }
        }

    }

}