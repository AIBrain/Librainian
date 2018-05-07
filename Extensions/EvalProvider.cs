// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/EvalProvider.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.CSharp;

    /// <summary>
    ///     <para>Pulled from <see cref="http://www.ckode.dk/programming/eval-in-c-yes-its-possible/" />.</para>
    /// </summary>
    public static class EvalProvider {

        /// <summary>
        ///     Example:
        ///     <para>var HelloWorld = EvalProvider.CreateEvalMethod&lt;Int32, string&gt;(@"return ""Hello world "" + arg;");</para>
        ///     <para>Console.WriteLine(HelloWorld(42));</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="code"></param>
        /// <param name="usingStatements"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static Func<T, TResult> CreateEvalMethod<T, TResult>( String code, String[] usingStatements = null, String[] assemblies = null ) {
            var returnType = typeof( TResult );
            var inputType = typeof( T );

            var includeUsings = new HashSet<String>( new[] { "System" } ) { returnType.Namespace, inputType.Namespace };
            if ( usingStatements != null ) {
                foreach ( var usingStatement in usingStatements ) {
                    includeUsings.Add( usingStatement );
                }
            }

            using ( var compiler = new CSharpCodeProvider() ) {
                var includeAssemblies = new HashSet<String>( new[] { "system.dll" } );
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

                var parameters = new CompilerParameters( includeAssemblies.ToArray() ) { GenerateInMemory = true };
                var compilerResult = compiler.CompileAssemblyFromSource( parameters, source );
                var compiledAssembly = compilerResult.CompiledAssembly;
                var type = compiledAssembly.GetType( $"{name}.EvalClass" );
                var method = type.GetMethod( "Eval" );
                return ( Func<T, TResult> )Delegate.CreateDelegate( typeof( Func<T, TResult> ), method ?? throw new InvalidOperationException() );
            }
        }

        private static String GetUsing( IEnumerable<String> usingStatements ) {
            var result = new StringBuilder();
            foreach ( var usingStatement in usingStatements ) {
                result.AppendLine( $"using {usingStatement};" );
            }
            return result.ToString();
        }
    }
}