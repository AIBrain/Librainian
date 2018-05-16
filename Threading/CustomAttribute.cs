// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "CustomAttribute.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license
// has been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/CustomAttribute.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class CustomAttribute : Attribute {

        static CustomAttribute() {
            MethodsList = new List<MethodInfo>( Assembly.GetExecutingAssembly().GetTypes().SelectMany( t => t.GetMethods() ).Where( m => m.GetCustomAttributes( typeof( CustomAttribute ), false ).Length > 0 ) );

            MethodsList.AddRange( Assembly.GetCallingAssembly().GetTypes().SelectMany( t => t.GetMethods() ).Where( m => m.GetCustomAttributes( typeof( CustomAttribute ), false ).Length > 0 ) );
        }

        /*
                public Boolean someThing;
        */

        public CustomAttribute( [CallerMemberName] String membername = "" ) {
            var method = MethodsList.FirstOrDefault( m => m.Name == membername );

            if ( method?.DeclaringType is null ) {
                return; //Not suppose to happen, but safety comes first
            }

            this.FullMethodPath = method.DeclaringType.Name + membername; //Work it around any way you want it

            // I need here to get the type of membername parent. Here I want to get CustClass, not fooBase
        }

        public static List<MethodInfo> MethodsList { get; }

        public String FullMethodPath { get; }
    }
}