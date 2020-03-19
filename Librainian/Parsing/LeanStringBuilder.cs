// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "LeanStringBuilder.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "LeanStringBuilder.cs" was last formatted by Protiguous on 2020/03/18 at 10:27 AM.

// ReSharper disable once CheckNamespace

namespace System.Text {

    using Collections.Generic;
    using Diagnostics;
    using JetBrains.Annotations;
    using Linq;
    using Newtonsoft.Json;

    /// <summary>Optimized for .Add()ing many! strings.
    /// <para>Doesn't realize the final string until <see cref="ToString" />.</para>
    /// <para>Won't throw exceptions on null or empty strings being added.</para>
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Serializable]
    public class LeanStringBuilder : IEquatable<LeanStringBuilder> {

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public Boolean Equals( LeanStringBuilder other ) => Equals( this, other );

        [JsonProperty]
        [NotNull]
        [ItemNotNull]
        private readonly List<Char[]> _parts;

        private Int32 charCount;

        private String compiled;

        private const Int32 InitialCapacity = 8;

        /// <summary>Optimized for .Add()ing many! strings.
        /// <para>Doesn't realize the final string until <see cref="ToString" />.</para>
        /// <para>Won't throw exceptions on null or empty strings being added.</para>
        /// </summary>
        public LeanStringBuilder( Int32 initialCapacity = InitialCapacity ) => this._parts = new List<Char[]>( initialCapacity );

        private void ClearCompiled() => this.compiled = null;

        /// <summary>Optimized for .Add()ing many! strings.
        /// <para>Doesn't realize the final string until <see cref="ToString" />.</para>
        /// <para>Won't throw exceptions on null or empty strings being added.</para>
        /// </summary>
        [NotNull]
        public static LeanStringBuilder Create( Int32 initialCapacity = InitialCapacity ) => new LeanStringBuilder( initialCapacity );

        /// <summary>static comparison of <paramref name="left" /> <see cref="LeanStringBuilder" /> vs <paramref name="right" /> <see cref="LeanStringBuilder" />.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] LeanStringBuilder left, [CanBeNull] LeanStringBuilder right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null || ( left.charCount != right.charCount ) || ( left._parts.Count != right._parts.Count ) ) {
                return default;
            }

            if ( ( left.compiled != null ) && ( right.compiled != null ) ) {
                return left.compiled.Equals( right.compiled, StringComparison.Ordinal );
            }

            return left._parts.SequenceEqual( right._parts );
        }

        /// <summary>Returns a value that indicates whether two <see cref="LeanStringBuilder" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static Boolean operator !=( [CanBeNull] LeanStringBuilder left, [CanBeNull] LeanStringBuilder right ) => !Equals( left, right );

        /// <summary>Returns a value that indicates whether the values of two <see cref="LeanStringBuilder" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static Boolean operator ==( [CanBeNull] LeanStringBuilder left, [CanBeNull] LeanStringBuilder right ) => Equals( left, right );

        [NotNull]
        public LeanStringBuilder Add( [CanBeNull] String? item ) => this.Add( item?.ToCharArray() );

        [NotNull]
        public LeanStringBuilder Add( [CanBeNull] Char[] chars ) {
            if ( chars is null ) {
                return this;
            }

            if ( chars.Length == 0 ) {
                return this;
            }

            this.charCount += chars.Length; //*2 ??
            this._parts.Add( chars );
            this.ClearCompiled();

            return this;
        }

        [NotNull]
        public LeanStringBuilder Append( [CanBeNull] String? item ) => this.Add( item?.ToCharArray() );

        [NotNull]
        public LeanStringBuilder Append( [CanBeNull] Char[] chars ) => this.Add( chars );

        [NotNull]
        public LeanStringBuilder Append<T>( [CanBeNull] T obj ) => this.Add( obj?.ToString() );

        [NotNull]
        public LeanStringBuilder Append( Boolean value ) => this.Append( value ? "True" : "False" );

        public void Clear() {
            this.charCount = 0;
            this._parts.Clear();
            this.ClearCompiled();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns><see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override Boolean Equals( Object obj ) => Equals( this, obj as LeanStringBuilder );

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this._parts.GetHashCode();

        public override String ToString() {
            if ( !String.IsNullOrEmpty( this.compiled ) ) {
                return this.compiled;
            }

            this.TrimExcess();
            var final = new Char[ this.charCount ];
            var offest = 0;

            foreach ( var t in this._parts ) {
                var l = t.Length * 2; //*2 because char are 2 bytes??
                Buffer.BlockCopy( t, 0, final, offest, l );
                offest += l;
            }

            return this.compiled = new String( final );
        }

        [NotNull]
        public LeanStringBuilder TrimExcess() {
            this._parts.TrimExcess();

            return this;
        }

    }

}