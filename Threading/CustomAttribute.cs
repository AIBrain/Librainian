#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/CustomAttribute.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class CustomAttribute : Attribute {
        public static readonly List< MethodInfo > MethodsList = new List< MethodInfo >();

        public string FullMethodPath;

        static CustomAttribute() {
            MethodsList = new List< MethodInfo >( Assembly.GetExecutingAssembly().GetTypes().SelectMany( t => t.GetMethods() ).Where( m => m.GetCustomAttributes( typeof ( CustomAttribute ), false ).Length > 0 ) );

            MethodsList.AddRange( Assembly.GetCallingAssembly().GetTypes().SelectMany( t => t.GetMethods() ).Where( m => m.GetCustomAttributes( typeof ( CustomAttribute ), false ).Length > 0 ) );
        }

/*
        public Boolean someThing;
*/

        public CustomAttribute( [CallerMemberName] string membername = "" ) {
            var method = MethodsList.FirstOrDefault( m => m.Name == membername );
            if ( method == null || method.DeclaringType == null ) {
                return; //Not suppose to happen, but safety comes first
            }
            this.FullMethodPath = method.DeclaringType.Name + membername; //Work it around any way you want it
            //  I need here to get the type of membername parent. 
            //  Here I want to get CustClass, not fooBase
        }
    }
}
