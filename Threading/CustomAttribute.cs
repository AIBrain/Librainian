// Copyright 2018 Protiguous.
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
// "Librainian/CustomAttribute.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

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

        public static List<MethodInfo> MethodsList {
            get;
        }

        public String FullMethodPath {
            get;
        }
    }
}