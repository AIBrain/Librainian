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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Strand.cs" was last cleaned by Rick on 2014/12/14 at 7:07 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Security;
    using System.Text;
    using System.Threading;

/*
    /// <summary>
    ///     This class is just a reference copy of <see cref="string" /> to be used for study.
    /// </summary>
    [ ComVisible( true ) ]
    [ Serializable ]
    public sealed class Strand : IComparable, ICloneable, IConvertible, IEnumerable
#if GENERICS_WORK
        , IComparable<Strand>, IEnumerable<char>, IEquatable<Strand>
#endif
    {
        //
        //NOTE NOTE NOTE NOTE
        //These fields map directly onto the fields in an EE StringObject.  See object.h for the layout.
        //
        [ NonSerialized ] private int m_stringLength;

        [ NonSerialized ] private char m_firstChar;

        //private static readonly char FmtMsgMarkerChar='%';
        //private static readonly char FmtMsgFmtCodeChar='!';
        //These are defined in Com99/src/vm/COMStringCommon.h and must be kept in [....].
        private const int TrimHead = 0;
        private const int TrimTail = 1;
        private const int TrimBoth = 2;

        // The Empty constant holds the empty string value. It is initialized by the EE during startup.
        // It is treated as intrinsic by the JIT as so the static constructor would never run.
        // Leaving it uninitialized would confuse debuggers.
        //
        //We need to call the Strand constructor so that the compiler doesn't mark this as a literal.
        //Marking this as a literal would mean that it doesn't show up as a field which we can access 
        //from native.
        public static Strand Empty => "";

        //
        //Native Static Methods
        //

        // Joins an array of strings together as one string with a separator between each original string.
        //
        public static Strand Join( Strand separator, params Strand[] value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }

            return Join( separator, value, 0, value.Length );
        }

        [ ComVisible( false ) ]
        public static Strand Join( Strand separator, params Object[] values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }

            if ( values.Length == 0 || values[ 0 ] == null ) {
                return Empty;
            }

            if ( separator == null ) {
                separator = Empty;
            }

            var result = new StringBuilder();

            Strand value = values[ 0 ].ToString();
            if ( value != null ) {
                result.Append( value );
            }

            for ( var i = 1; i < values.Length; i++ ) {
                result.Append( separator );
                if ( values[ i ] != null ) {
                    // handle the case where their ToString() override is broken
                    value = values[ i ].ToString();
                    if ( value != null ) {
                        result.Append( value );
                    }
                }
            }
            return StringBuilderCache.GetStringAndRelease( result );
        }

        [ ComVisible( false ) ]
        public static Strand Join< T >( Strand separator, IEnumerable< T > values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            if ( separator == null ) {
                separator = Empty;
            }

            using ( var en = values.GetEnumerator() ) {
                if ( !en.MoveNext() ) {
                    return Empty;
                }

                var result = StringBuilderCache.Acquire();
                if ( en.Current != null ) {
                    // handle the case that the enumeration has null entries
                    // and the case where their ToString() override is broken
                    var value = en.Current.ToString();
                    if ( value != null ) {
                        result.Append( value );
                    }
                }

                while ( en.MoveNext() ) {
                    result.Append( separator );
                    if ( en.Current != null ) {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        var value = en.Current.ToString();
                        if ( value != null ) {
                            result.Append( value );
                        }
                    }
                }
                return StringBuilderCache.GetStringAndRelease( result );
            }
        }

        [ ComVisible( false ) ]
        public static Strand Join( Strand separator, IEnumerable< Strand > values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            if ( separator == null ) {
                separator = Empty;
            }

            using ( var en = values.GetEnumerator() ) {
                if ( !en.MoveNext() ) {
                    return Empty;
                }

                var result = StringBuilderCache.Acquire();
                if ( en.Current != null ) {
                    result.Append( en.Current );
                }

                while ( en.MoveNext() ) {
                    result.Append( separator );
                    if ( en.Current != null ) {
                        result.Append( en.Current );
                    }
                }
                return StringBuilderCache.GetStringAndRelease( result );
            }
        }

#if WIN64
        private const int charPtrAlignConst = 3;
        private const int alignConst        = 7;
#else
        private const int charPtrAlignConst = 1;
        private const int alignConst = 3;
#endif

        internal char FirstChar { get { return m_firstChar; } }

        // Joins an array of strings together as one string with a separator between each original string.
        //
        [ SecuritySafeCritical ] // auto-generated
        public static unsafe Strand Join( Strand separator, Strand[] value, int startIndex, int count ) {
            //Range check the array
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }

            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_NegativeCount" ) );
            }

            if ( startIndex > value.Length - count ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_IndexCountBuffer" ) );
            }

            //Treat null as empty string.
            if ( separator == null ) {
                separator = Empty;
            }

            //If count is 0, that skews a whole bunch of the calculations below, so just special case that.
            if ( count == 0 ) {
                return Empty;
            }

            var jointLength = 0;
            //Figure out the total length of the strings in value
            var endIndex = startIndex + count - 1;
            for ( var stringToJoinIndex = startIndex; stringToJoinIndex <= endIndex; stringToJoinIndex++ ) {
                if ( value[ stringToJoinIndex ] != null ) {
                    jointLength += value[ stringToJoinIndex ].Length;
                }
            }

            //Add enough room for the separator.
            jointLength += ( count - 1 ) * separator.Length;

            // Note that we may not catch all overflows with this check (since we could have wrapped around the 4gb range any number of times
            // and landed back in the positive range.) The input array might be modifed from other threads, 
            // so we have to do an overflow check before each append below anyway. Those overflows will get caught down there.
            if ( ( jointLength < 0 ) || ( ( jointLength + 1 ) < 0 ) ) {
                throw new OutOfMemoryException();
            }

            //If this is an empty string, just return.
            if ( jointLength == 0 ) {
                return Empty;
            }

            string jointString = FastAllocateString( jointLength );
            fixed ( char* pointerToJointString = &jointString.m_firstChar ) {
                var charBuffer = new UnSafeCharBuffer( pointerToJointString, jointLength );

                // Append the first string first and then append each following string prefixed by the separator.
                charBuffer.AppendString( value[ startIndex ] );
                for ( var stringToJoinIndex = startIndex + 1; stringToJoinIndex <= endIndex; stringToJoinIndex++ ) {
                    charBuffer.AppendString( separator );
                    charBuffer.AppendString( value[ stringToJoinIndex ] );
                }
                Contract.Assert( *( pointerToJointString + charBuffer.Length ) == '\0', "Strand must be null-terminated!" );
            }

            return jointString;
        }

        [ SecuritySafeCritical ] // auto-generated
        private static unsafe int CompareOrdinalIgnoreCaseHelper( Strand strA, Strand strB ) {
            Contract.Requires( strA != null );
            Contract.Requires( strB != null );

            var length = Math.Min( strA.Length, strB.Length );

            fixed ( char* ap = &strA.m_firstChar ) {
                fixed ( char* bp = &strB.m_firstChar ) {
                    var a = ap;
                    var b = bp;

                    while ( length != 0 ) {
                        int charA = *a;
                        int charB = *b;

                        Contract.Assert( ( charA | charB ) <= 0x7F, "strings have to be ASCII" );

                        // uppercase both chars - notice that we need just one compare per char
                        if ( ( uint ) ( charA - 'a' ) <= 'z' - 'a' ) {
                            charA -= 0x20;
                        }
                        if ( ( uint ) ( charB - 'a' ) <= 'z' - 'a' ) {
                            charB -= 0x20;
                        }

                        //Return the (case-insensitive) difference between them.
                        if ( charA != charB ) {
                            return charA - charB;
                        }

                        // Next char
                        a++;
                        b++;
                        length--;
                    }

                    return strA.Length - strB.Length;
                }
            }
        }

        // native call to COMString::CompareOrdinalEx
        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal static extern int nativeCompareOrdinalEx( Strand strA, int indexA, Strand strB, int indexB, int count );

        //This will not work in case-insensitive mode for any character greater than 0x80.  
        //We'll throw an ArgumentException.
        // 
        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal static extern unsafe int nativeCompareOrdinalIgnoreCaseWC( Strand strA, sbyte* strBBytes );

        //
        // This is a helper method for the security team.  They need to uppercase some strings (guaranteed to be less 
        // than 0x80) before security is fully initialized.  Without security initialized, we can't grab resources (the nlp's)
        // from the assembly.  This provides a workaround for that problem and should NOT be used anywhere else.
        //
        [ SecuritySafeCritical ] // auto-generated
        internal static unsafe string SmallCharToUpper( string strIn ) {
            Contract.Requires( strIn != null );

            //
            // Get the length and pointers to each of the buffers.  Walk the length
            // of the string and copy the characters from the inBuffer to the outBuffer,
            // capitalizing it if necessary.  We assert that all of our characters are
            // less than 0x80.
            //
            var length = strIn.Length;
            var strOut = FastAllocateString( length );
            fixed ( char* inBuff = &strIn.m_firstChar, outBuff = &strOut.m_firstChar ) {
                for ( var i = 0; i < length; i++ ) {
                    int c = inBuff[ i ];
                    Contract.Assert( c <= 0x7F, "string has to be ASCII" );

                    // uppercase - notice that we need just one compare
                    if ( ( uint ) ( c - 'a' ) <= 'z' - 'a' ) {
                        c -= 0x20;
                    }

                    outBuff[ i ] = ( char ) c;
                }

                Contract.Assert( outBuff[ length ] == '\0', "outBuff[length]=='\0'" );
            }
            return strOut;
        }

        //
        //
        // NATIVE INSTANCE METHODS
        //
        //

        //
        // Search/Query methods
        //

        [ SecuritySafeCritical ] // auto-generated
        [ ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail ) ]
        private static unsafe bool EqualsHelper( Strand strA, Strand strB ) {
            Contract.Requires( strA != null );
            Contract.Requires( strB != null );
            Contract.Requires( strA.Length == strB.Length );

            var length = strA.Length;

            fixed ( char* ap = &strA.m_firstChar ) {
                fixed ( char* bp = &strB.m_firstChar ) {
                    var a = ap;
                    var b = bp;

                    // unroll the loop
#if AMD64
    // for AMD64 bit platform we unroll by 12 and
    // check 3 qword at a time. This is less code
    // than the 32 bit case and is shorter
    // pathlength
 
                while (length >= 12)
                {
                    if (*(long*)a     != *(long*)b) return false;
                    if (*(long*)(a+4) != *(long*)(b+4)) return false;
                    if (*(long*)(a+8) != *(long*)(b+8)) return false;
                    a += 12; b += 12; length -= 12;
                }
#else
                    while ( length >= 10 ) {
                        if ( *( int* ) a != *( int* ) b ) {
                            return false;
                        }
                        if ( *( int* ) ( a + 2 ) != *( int* ) ( b + 2 ) ) {
                            return false;
                        }
                        if ( *( int* ) ( a + 4 ) != *( int* ) ( b + 4 ) ) {
                            return false;
                        }
                        if ( *( int* ) ( a + 6 ) != *( int* ) ( b + 6 ) ) {
                            return false;
                        }
                        if ( *( int* ) ( a + 8 ) != *( int* ) ( b + 8 ) ) {
                            return false;
                        }
                        a += 10;
                        b += 10;
                        length -= 10;
                    }
#endif

                    // This depends on the fact that the Strand objects are
                    // always zero terminated and that the terminating zero is not included
                    // in the length. For odd string sizes, the last compare will include
                    // the zero terminator.
                    while ( length > 0 ) {
                        if ( *( int* ) a != *( int* ) b ) {
                            break;
                        }
                        a += 2;
                        b += 2;
                        length -= 2;
                    }

                    return ( length <= 0 );
                }
            }
        }

        [ SecuritySafeCritical ] // auto-generated
        private static unsafe int CompareOrdinalHelper( Strand strA, Strand strB ) {
            Contract.Requires( strA != null );
            Contract.Requires( strB != null );

            var length = Math.Min( strA.Length, strB.Length );
            var diffOffset = -1;

            fixed ( char* ap = &strA.m_firstChar ) {
                fixed ( char* bp = &strB.m_firstChar ) {
                    var a = ap;
                    var b = bp;

                    // unroll the loop
                    while ( length >= 10 ) {
                        if ( *( int* ) a != *( int* ) b ) {
                            diffOffset = 0;
                            break;
                        }

                        if ( *( int* ) ( a + 2 ) != *( int* ) ( b + 2 ) ) {
                            diffOffset = 2;
                            break;
                        }

                        if ( *( int* ) ( a + 4 ) != *( int* ) ( b + 4 ) ) {
                            diffOffset = 4;
                            break;
                        }

                        if ( *( int* ) ( a + 6 ) != *( int* ) ( b + 6 ) ) {
                            diffOffset = 6;
                            break;
                        }

                        if ( *( int* ) ( a + 8 ) != *( int* ) ( b + 8 ) ) {
                            diffOffset = 8;
                            break;
                        }
                        a += 10;
                        b += 10;
                        length -= 10;
                    }

                    if ( diffOffset != -1 ) {
                        // we already see a difference in the unrolled loop above
                        a += diffOffset;
                        b += diffOffset;
                        int order;
                        if ( ( order = *a - *b ) != 0 ) {
                            return order;
                        }
                        Contract.Assert( *( a + 1 ) != *( b + 1 ), "This byte must be different if we reach here!" );
                        return ( *( a + 1 ) - *( b + 1 ) );
                    }

                    // now go back to slower code path and do comparison on 4 bytes one time.
                    // Following code also take advantage of the fact strings will 
                    // use even numbers of characters (runtime will have a extra zero at the end.)
                    // so even if length is 1 here, we can still do the comparsion.  
                    while ( length > 0 ) {
                        if ( *( int* ) a != *( int* ) b ) {
                            break;
                        }
                        a += 2;
                        b += 2;
                        length -= 2;
                    }

                    if ( length > 0 ) {
                        int c;
                        // found a different int on above loop
                        if ( ( c = *a - *b ) != 0 ) {
                            return c;
                        }
                        Contract.Assert( *( a + 1 ) != *( b + 1 ), "This byte must be different if we reach here!" );
                        return ( *( a + 1 ) - *( b + 1 ) );
                    }

                    // At this point, we have compared all the characters in at least one string.
                    // The longer string will be larger.
                    return strA.Length - strB.Length;
                }
            }
        }

        // Determines whether two strings match.
        [ ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail ) ]
        public override bool Equals( Object obj ) {
            if ( this == null ) //this is necessary to guard against reverse-pinvokes and
            {
                throw new NullReferenceException(); //other callers who do not use the callvirt instruction
            }

            var str = obj as Strand;
            if ( str == null ) {
                return false;
            }

            if ( ReferenceEquals( this, obj ) ) {
                return true;
            }

            if ( this.Length != str.Length ) {
                return false;
            }

            return EqualsHelper( this, str );
        }

        // Determines whether two strings match.
        [ Pure ]
        [ ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail ) ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public bool Equals( Strand value ) {
            if ( this == null ) //this is necessary to guard against reverse-pinvokes and
            {
                throw new NullReferenceException(); //other callers who do not use the callvirt instruction
            }

            if ( value == null ) {
                return false;
            }

            if ( ReferenceEquals( this, value ) ) {
                return true;
            }

            if ( this.Length != value.Length ) {
                return false;
            }

            return EqualsHelper( this, value );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        public bool Equals( Strand value, StringComparison comparisonType ) {
            if ( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( this == ( Object ) value ) {
                return true;
            }

            if ( ( Object ) value == null ) {
                return false;
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return ( CultureInfo.CurrentCulture.CompareInfo.Compare( this, value, CompareOptions.None ) == 0 );

                case StringComparison.CurrentCultureIgnoreCase:
                    return ( CultureInfo.CurrentCulture.CompareInfo.Compare( this, value, CompareOptions.IgnoreCase ) == 0 );

                case StringComparison.InvariantCulture:
                    return ( CultureInfo.InvariantCulture.CompareInfo.Compare( this, value, CompareOptions.None ) == 0 );

                case StringComparison.InvariantCultureIgnoreCase:
                    return ( CultureInfo.InvariantCulture.CompareInfo.Compare( this, value, CompareOptions.IgnoreCase ) == 0 );

                case StringComparison.Ordinal:
                    if ( this.Length != value.Length ) {
                        return false;
                    }
                    return EqualsHelper( this, value );

                case StringComparison.OrdinalIgnoreCase:
                    if ( this.Length != value.Length ) {
                        return false;
                    }

                    // If both strings are ASCII strings, we can take the fast path.
                    if ( this.IsAscii() && value.IsAscii() ) {
                        return ( CompareOrdinalIgnoreCaseHelper( this, value ) == 0 );
                    }
                    // Take the slow path.                                    
                    return ( TextInfo.CompareOrdinalIgnoreCase( this, value ) == 0 );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

        // Determines whether two Strings match.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static bool Equals( Strand a, Strand b ) {
            if ( a == ( Object ) b ) {
                return true;
            }

            if ( ( Object ) a == null || ( Object ) b == null ) {
                return false;
            }

            if ( a.Length != b.Length ) {
                return false;
            }

            return EqualsHelper( a, b );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        public static bool Equals( Strand a, Strand b, StringComparison comparisonType ) {
            if ( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( a == ( Object ) b ) {
                return true;
            }

            if ( ( Object ) a == null || ( Object ) b == null ) {
                return false;
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return ( CultureInfo.CurrentCulture.CompareInfo.Compare( a, b, CompareOptions.None ) == 0 );

                case StringComparison.CurrentCultureIgnoreCase:
                    return ( CultureInfo.CurrentCulture.CompareInfo.Compare( a, b, CompareOptions.IgnoreCase ) == 0 );

                case StringComparison.InvariantCulture:
                    return ( CultureInfo.InvariantCulture.CompareInfo.Compare( a, b, CompareOptions.None ) == 0 );

                case StringComparison.InvariantCultureIgnoreCase:
                    return ( CultureInfo.InvariantCulture.CompareInfo.Compare( a, b, CompareOptions.IgnoreCase ) == 0 );

                case StringComparison.Ordinal:
                    if ( a.Length != b.Length ) {
                        return false;
                    }

                    return EqualsHelper( a, b );

                case StringComparison.OrdinalIgnoreCase:
                    if ( a.Length != b.Length ) {
                        return false;
                    }
                    // If both strings are ASCII strings, we can take the fast path.
                    if ( a.IsAscii() && b.IsAscii() ) {
                        return ( CompareOrdinalIgnoreCaseHelper( a, b ) == 0 );
                    }
                    // Take the slow path.

                    return ( TextInfo.CompareOrdinalIgnoreCase( a, b ) == 0 );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static bool operator ==( Strand a, Strand b ) {
            return Equals( a, b );
        }

#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static bool operator !=( Strand a, Strand b ) {
            return !Equals( a, b );
        }

        public static implicit operator Strand( String v ) {
            throw new NotImplementedException();
        }

        // Gets the character at a specified position.
        //
        // Spec#: Apply the precondition here using a contract assembly.  Potential perf issue.
        [ IndexerName( "Chars" ) ]
        public extern char this[ int index ] { [ ResourceExposure( ResourceScope.None ) ] [ MethodImpl( MethodImplOptions.InternalCall ) ] [ SecuritySafeCritical ] // public member
        get; }

        // Converts a substring of this string to an array of characters.  Copies the
        // characters of this string beginning at position startIndex and ending at
        // startIndex + length - 1 to the character array buffer, beginning
        // at bufferStartIndex.
        //
        [ SecuritySafeCritical ] // auto-generated
        public unsafe void CopyTo( int sourceIndex, char[] destination, int destinationIndex, int count ) {
            if ( destination == null ) {
                throw new ArgumentNullException( "destination" );
            }
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_NegativeCount" ) );
            }
            if ( sourceIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "sourceIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }
            if ( count > Length - sourceIndex ) {
                throw new ArgumentOutOfRangeException( "sourceIndex", Environment.GetResourceString( "ArgumentOutOfRange_IndexCount" ) );
            }
            if ( destinationIndex > destination.Length - count || destinationIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "destinationIndex", Environment.GetResourceString( "ArgumentOutOfRange_IndexCount" ) );
            }

            // Note: fixed does not like empty arrays
            if ( count > 0 ) {
                fixed ( char* src = &this.m_firstChar ) {
                    fixed ( char* dest = destination ) {
                        wstrcpy( dest + destinationIndex, src + sourceIndex, count );
                    }
                }
            }
        }

        // Returns the entire string as an array of characters.
        [ SecuritySafeCritical ] // auto-generated
        public unsafe char[] ToCharArray() {
            // <STRIP> huge performance improvement for short strings by doing this </STRIP>
            var length = Length;
            var chars = new char[length];
            if ( length > 0 ) {
                fixed ( char* src = &this.m_firstChar ) {
                    fixed ( char* dest = chars ) {
                        wstrcpy( dest, src, length );
                    }
                }
            }
            return chars;
        }

        // Returns a substring of this string as an array of characters.
        //
        [ SecuritySafeCritical ] // auto-generated
        public unsafe char[] ToCharArray( int startIndex, int length ) {
            // Range check everything.
            if ( startIndex < 0 || startIndex > Length || startIndex > Length - length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }
            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            var chars = new char[length];
            if ( length > 0 ) {
                fixed ( char* src = &this.m_firstChar ) {
                    fixed ( char* dest = chars ) {
                        wstrcpy( dest, src + startIndex, length );
                    }
                }
            }
            return chars;
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static bool IsNullOrEmpty( Strand value ) {
            return ( value == null || value.Length == 0 );
        }

        [ Pure ]
        public static bool IsNullOrWhiteSpace( Strand value ) {
            if ( value == null ) {
                return true;
            }

            for ( var i = 0; i < value.Length; i++ ) {
                if ( !Char.IsWhiteSpace( value[ i ] ) ) {
                    return false;
                }
            }

            return true;
        }

#if FEATURE_RANDOMIZED_STRING_HASHING
        [System.Security.SecurityCritical]
        [System.Security.SuppressUnmanagedCodeSecurity]
        [ResourceExposure(ResourceScope.None)]
        [DllImport(JitHelpers.QCall, CharSet = CharSet.Unicode)]
        internal static extern int InternalMarvin32HashString(string s, int sLen, long additionalEntropy);
 
        [System.Security.SecuritySafeCritical]
        [ResourceExposure(ResourceScope.None)]
        internal static bool UseRandomizedHashing() {
            return InternalUseRandomizedHashing();
        }
 
        [System.Security.SecurityCritical]
        [System.Security.SuppressUnmanagedCodeSecurity]
        [ResourceExposure(ResourceScope.None)]
        [DllImport(JitHelpers.QCall, CharSet = CharSet.Unicode)]
        private static extern bool InternalUseRandomizedHashing();
#endif

        // Gets a hash code for this string.  If strings A and B are such that A.Equals(B), then
        // they will return the same hash code.
        [ SecuritySafeCritical ] // auto-generated
        [ ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail ) ]
        public override int GetHashCode() {
#if FEATURE_RANDOMIZED_STRING_HASHING
            if(HashHelpers.s_UseRandomizedStringHashing)
            {
                return InternalMarvin32HashString(this, this.Length, 0);
            }
#endif // FEATURE_RANDOMIZED_STRING_HASHING

            unsafe {
                fixed ( char* src = this ) {
                    Contract.Assert( src[ this.Length ] == '\0', "src[this.Length] == '\\0'" );
                    Contract.Assert( ( ( int ) src ) % 4 == 0, "Managed string should start at 4 bytes boundary" );

#if WIN32
                    int hash1 = (5381<<16) + 5381;
#else
                    var hash1 = 5381;
#endif
                    var hash2 = hash1;

#if WIN32
    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = this.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }
#else
                    int c;
                    var s = src;
                    while ( ( c = s[ 0 ] ) != 0 ) {
                        hash1 = ( ( hash1 << 5 ) + hash1 ) ^ c;
                        c = s[ 1 ];
                        if ( c == 0 ) {
                            break;
                        }
                        hash2 = ( ( hash2 << 5 ) + hash2 ) ^ c;
                        s += 2;
                    }
#endif
#if DEBUG
                    // We want to ensure we can change our hash function daily.
                    // This is perfectly fine as long as you don't persist the
                    // value from GetHashCode to disk or count on Strand A 
                    // hashing before string B.  Those are bugs in your code.
                    hash1 ^= ThisAssembly.DailyBuildNumber;
#endif
                    return hash1 + ( hash2 * 1566083941 );
                }
            }
        }

        // Use this if and only if you need the hashcode to not change across app domains (e.g. you have an app domain agile
        // hash table).
        [ SecuritySafeCritical ] // auto-generated
        [ ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail ) ]
        internal int GetLegacyNonRandomizedHashCode() {
            unsafe {
                fixed ( char* src = this ) {
                    Contract.Assert( src[ this.Length ] == '\0', "src[this.Length] == '\\0'" );
                    Contract.Assert( ( ( int ) src ) % 4 == 0, "Managed string should start at 4 bytes boundary" );

#if WIN32
                    int hash1 = (5381<<16) + 5381;
#else
                    var hash1 = 5381;
#endif
                    var hash2 = hash1;

#if WIN32
    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = this.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }
#else
                    int c;
                    var s = src;
                    while ( ( c = s[ 0 ] ) != 0 ) {
                        hash1 = ( ( hash1 << 5 ) + hash1 ) ^ c;
                        c = s[ 1 ];
                        if ( c == 0 ) {
                            break;
                        }
                        hash2 = ( ( hash2 << 5 ) + hash2 ) ^ c;
                        s += 2;
                    }
#endif
#if DEBUG
                    // We want to ensure we can change our hash function daily.
                    // This is perfectly fine as long as you don't persist the
                    // value from GetHashCode to disk or count on Strand A 
                    // hashing before string B.  Those are bugs in your code.
                    hash1 ^= ThisAssembly.DailyBuildNumber;
#endif
                    return hash1 + ( hash2 * 1566083941 );
                }
            }
        }

        // Gets the length of this string
        //
        /// This is a EE implemented function so that the JIT can recognise is specially
        /// and eliminate checks on character fetchs in a loop like:
        /// for(int I = 0; I
        /// < str.Length; i++) str[ i]
        ///     The actually code generated for this will be one instruction and will be inlined.
        //
        // Spec#: Add postcondition in a contract assembly.  Potential perf problem.
        public extern int Length { [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ] [ MethodImpl( MethodImplOptions.InternalCall ) ] get; }

        // Creates an array of strings by splitting this string at each
        // occurence of a separator.  The separator is searched for, and if found,
        // the substring preceding the occurence is stored as the first element in
        // the array of strings.  We then continue in this manner by searching
        // the substring that follows the occurence.  On the other hand, if the separator
        // is not found, the array of strings will contain this instance as its only element.
        // If the separator is null
        // whitespace (i.e., Character.IsWhitespace) is used as the separator.
        //
        public Strand[] Split( params char[] separator ) {
            Contract.Ensures( Contract.Result< Strand[] >() != null );
            return SplitInternal( separator, Int32.MaxValue, StringSplitOptions.None );
        }

        // Creates an array of strings by splitting this string at each
        // occurence of a separator.  The separator is searched for, and if found,
        // the substring preceding the occurence is stored as the first element in
        // the array of strings.  We then continue in this manner by searching
        // the substring that follows the occurence.  On the other hand, if the separator
        // is not found, the array of strings will contain this instance as its only element.
        // If the spearator is the empty string (i.e., Strand.Empty), then
        // whitespace (i.e., Character.IsWhitespace) is used as the separator.
        // If there are more than count different strings, the last n-(count-1)
        // elements are concatenated and added as the last Strand.
        //
        public string[] Split( char[] separator, int count ) {
            Contract.Ensures( Contract.Result< Strand[] >() != null );
            return SplitInternal( separator, count, StringSplitOptions.None );
        }

        [ ComVisible( false ) ]
        public Strand[] Split( char[] separator, StringSplitOptions options ) {
            Contract.Ensures( Contract.Result< Strand[] >() != null );
            return SplitInternal( separator, Int32.MaxValue, options );
        }

        [ ComVisible( false ) ]
        public Strand[] Split( char[] separator, int count, StringSplitOptions options ) {
            Contract.Ensures( Contract.Result< Strand[] >() != null );
            return SplitInternal( separator, count, options );
        }

        [ ComVisible( false ) ]
        internal Strand[] SplitInternal( char[] separator, int count, StringSplitOptions options ) {
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_NegativeCount" ) );
            }

            if ( options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries ) {
                throw new ArgumentException( Environment.GetResourceString( "Arg_EnumIllegalVal", options ) );
            }
            Contract.Ensures( Contract.Result< Strand[] >() != null );

            var omitEmptyEntries = ( options == StringSplitOptions.RemoveEmptyEntries );

            if ( ( count == 0 ) || ( omitEmptyEntries && this.Length == 0 ) ) {
                return new Strand[0];
            }

            var sepList = new int[Length];
            var numReplaces = MakeSeparatorList( separator, ref sepList );

            //Handle the special case of no replaces and special count.
            if ( 0 == numReplaces || count == 1 ) {
                var stringArray = new Strand[1];
                stringArray[ 0 ] = this;
                return stringArray;
            }

            if ( omitEmptyEntries ) {
                return InternalSplitOmitEmptyEntries( sepList, null, numReplaces, count );
            }
            return InternalSplitKeepEmptyEntries( sepList, null, numReplaces, count );
        }

        [ ComVisible( false ) ]
        public Strand[] Split( Strand[] separator, StringSplitOptions options ) {
            Contract.Ensures( Contract.Result< Strand[] >() != null );
            return Split( separator, Int32.MaxValue, options );
        }

        [ ComVisible( false ) ]
        public Strand[] Split( Strand[] separator, Int32 count, StringSplitOptions options ) {
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_NegativeCount" ) );
            }

            if ( options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries ) {
                throw new ArgumentException( Environment.GetResourceString( "Arg_EnumIllegalVal", ( int ) options ) );
            }

            var omitEmptyEntries = ( options == StringSplitOptions.RemoveEmptyEntries );

            if ( separator == null || separator.Length == 0 ) {
                return SplitInternal( null, count, options );
            }

            if ( ( count == 0 ) || ( omitEmptyEntries && this.Length == 0 ) ) {
                return new Strand[0];
            }

            var sepList = new int[Length];
            var lengthList = new int[Length];
            var numReplaces = MakeSeparatorList( separator, ref sepList, ref lengthList );

            //Handle the special case of no replaces and special count.
            if ( 0 == numReplaces || count == 1 ) {
                var stringArray = new Strand[1];
                stringArray[ 0 ] = this;
                return stringArray;
            }

            if ( omitEmptyEntries ) {
                return InternalSplitOmitEmptyEntries( sepList, lengthList, numReplaces, count );
            }
            return InternalSplitKeepEmptyEntries( sepList, lengthList, numReplaces, count );
        }

        // Note a few special case in this function:
        //     If there is no separator in the string, a string array which only contains 
        //     the original string will be returned regardless of the count. 
        //

        private Strand[] InternalSplitKeepEmptyEntries( Int32[] sepList, Int32[] lengthList, Int32 numReplaces, int count ) {
            Contract.Requires( numReplaces >= 0 );
            Contract.Requires( count >= 2 );
            Contract.Ensures( Contract.Result< Strand[] >() != null );

            var currIndex = 0;
            var arrIndex = 0;

            count--;
            var numActualReplaces = ( numReplaces < count ) ? numReplaces : count;

            //Allocate space for the new array.
            //+1 for the string from the end of the last replace to the end of the Strand.
            var splitStrings = new Strand[numActualReplaces + 1];

            for ( var i = 0; i < numActualReplaces && currIndex < Length; i++ ) {
                splitStrings[ arrIndex++ ] = Substring( currIndex, sepList[ i ] - currIndex );
                currIndex = sepList[ i ] + ( ( lengthList == null ) ? 1 : lengthList[ i ] );
            }

            //Handle the last string at the end of the array if there is one.
            if ( currIndex < Length && numActualReplaces >= 0 ) {
                splitStrings[ arrIndex ] = Substring( currIndex );
            }
            else if ( arrIndex == numActualReplaces ) {
                //We had a separator character at the end of a string.  Rather than just allowing
                //a null character, we'll replace the last element in the array with an empty string.
                splitStrings[ arrIndex ] = Empty;
            }

            return splitStrings;
        }

        // This function will not keep the Empty Strand 
        private Strand[] InternalSplitOmitEmptyEntries( Int32[] sepList, Int32[] lengthList, Int32 numReplaces, int count ) {
            Contract.Requires( numReplaces >= 0 );
            Contract.Requires( count >= 2 );
            Contract.Ensures( Contract.Result< Strand[] >() != null );

            // Allocate array to hold items. This array may not be 
            // filled completely in this function, we will create a 
            // new array and copy string references to that new array.

            var maxItems = ( numReplaces < count ) ? ( numReplaces + 1 ) : count;
            var splitStrings = new Strand[maxItems];

            var currIndex = 0;
            var arrIndex = 0;

            for ( var i = 0; i < numReplaces && currIndex < Length; i++ ) {
                if ( sepList[ i ] - currIndex > 0 ) {
                    splitStrings[ arrIndex++ ] = Substring( currIndex, sepList[ i ] - currIndex );
                }
                currIndex = sepList[ i ] + ( ( lengthList == null ) ? 1 : lengthList[ i ] );
                if ( arrIndex == count - 1 ) {
                    // If all the remaining entries at the end are empty, skip them
                    while ( i < numReplaces - 1 && currIndex == sepList[ ++i ] ) {
                        currIndex += ( ( lengthList == null ) ? 1 : lengthList[ i ] );
                    }
                    break;
                }
            }

            // we must have at least one slot left to fill in the last string.
            Contract.Assert( arrIndex < maxItems );

            //Handle the last string at the end of the array if there is one.
            if ( currIndex < Length ) {
                splitStrings[ arrIndex++ ] = Substring( currIndex );
            }

            var stringArray = splitStrings;
            if ( arrIndex != maxItems ) {
                stringArray = new Strand[arrIndex];
                for ( var j = 0; j < arrIndex; j++ ) {
                    stringArray[ j ] = splitStrings[ j ];
                }
            }
            return stringArray;
        }

        //--------------------------------------------------------------------    
        // This function returns number of the places within baseString where 
        // instances of characters in Separator occur.         
        // Args: separator  -- A string containing all of the split characters.
        //       sepList    -- an array of ints for split char indicies.
        //--------------------------------------------------------------------    
        [ SecuritySafeCritical ] // auto-generated
        private unsafe int MakeSeparatorList( char[] separator, ref int[] sepList ) {
            var foundCount = 0;

            if ( separator == null || separator.Length == 0 ) {
                fixed ( char* pwzChars = &this.m_firstChar ) {
                    //If they passed null or an empty string, look for whitespace.
                    for ( var i = 0; i < Length && foundCount < sepList.Length; i++ ) {
                        if ( Char.IsWhiteSpace( pwzChars[ i ] ) ) {
                            sepList[ foundCount++ ] = i;
                        }
                    }
                }
            }
            else {
                var sepListCount = sepList.Length;
                var sepCount = separator.Length;
                //If they passed in a string of chars, actually look for those chars.
                fixed ( char* pwzChars = &this.m_firstChar, pSepChars = separator ) {
                    for ( var i = 0; i < Length && foundCount < sepListCount; i++ ) {
                        var pSep = pSepChars;
                        for ( var j = 0; j < sepCount; j++, pSep++ ) {
                            if ( pwzChars[ i ] == *pSep ) {
                                sepList[ foundCount++ ] = i;
                                break;
                            }
                        }
                    }
                }
            }
            return foundCount;
        }

        //--------------------------------------------------------------------    
        // This function returns number of the places within baseString where 
        // instances of separator strings occur.         
        // Args: separators -- An array containing all of the split strings.
        //       sepList    -- an array of ints for split string indicies.
        //       lengthList -- an array of ints for split string lengths.
        //--------------------------------------------------------------------    
        [ SecuritySafeCritical ] // auto-generated
        private unsafe int MakeSeparatorList( Strand[] separators, ref int[] sepList, ref int[] lengthList ) {
            Contract.Assert( separators != null && separators.Length > 0, "separators != null && separators.Length > 0" );

            var foundCount = 0;
            var sepListCount = sepList.Length;
            var sepCount = separators.Length;

            fixed ( char* pwzChars = &this.m_firstChar ) {
                for ( var i = 0; i < Length && foundCount < sepListCount; i++ ) {
                    for ( var j = 0; j < separators.Length; j++ ) {
                        var separator = separators[ j ];
                        if ( IsNullOrEmpty( separator ) ) {
                            continue;
                        }
                        var currentSepLength = separator.Length;
                        if ( pwzChars[ i ] == separator[ 0 ] && currentSepLength <= Length - i ) {
                            if ( currentSepLength == 1 || CompareOrdinal( this, i, separator, 0, currentSepLength ) == 0 ) {
                                sepList[ foundCount ] = i;
                                lengthList[ foundCount ] = currentSepLength;
                                foundCount++;
                                i += currentSepLength - 1;
                                break;
                            }
                        }
                    }
                }
            }
            return foundCount;
        }

        // Returns a substring of this string.
        //
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand Substring( int startIndex ) {
            return this.Substring( startIndex, Length - startIndex );
        }

        // Returns a substring of this string.
        //
        [ SecuritySafeCritical ] // auto-generated
        public Strand Substring( int startIndex, int length ) {
            //Bounds Checking.
            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }

            if ( startIndex > Length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndexLargerThanLength" ) );
            }

            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_NegativeLength" ) );
            }

            if ( startIndex > Length - length ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_IndexLength" ) );
            }

            if ( length == 0 ) {
                return Empty;
            }

            if ( startIndex == 0 && length == this.Length ) {
                return this;
            }

            return InternalSubString( startIndex, length );
        }

        [ SecurityCritical ] // auto-generated
        private unsafe string InternalSubString( int startIndex, int length ) {
            Contract.Assert( startIndex >= 0 && startIndex <= this.Length, "StartIndex is out of range!" );
            Contract.Assert( length >= 0 && startIndex <= this.Length - length, "length is out of range!" );

            var result = FastAllocateString( length );

            fixed ( char* dest = &result.m_firstChar ) {
                fixed ( char* src = &this.m_firstChar ) {
                    wstrcpy( dest, src + startIndex, length );
                }
            }

            return result;
        }

        // Removes a string of characters from the ends of this string.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand Trim( params char[] trimChars ) {
            if ( null == trimChars || trimChars.Length == 0 ) {
                return TrimHelper( TrimBoth );
            }
            return TrimHelper( trimChars, TrimBoth );
        }

        // Removes a string of characters from the beginning of this string.
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand TrimStart( params char[] trimChars ) {
            if ( null == trimChars || trimChars.Length == 0 ) {
                return TrimHelper( TrimHead );
            }
            return TrimHelper( trimChars, TrimHead );
        }

        // Removes a string of characters from the end of this string.
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand TrimEnd( params char[] trimChars ) {
            if ( null == trimChars || trimChars.Length == 0 ) {
                return TrimHelper( TrimTail );
            }
            return TrimHelper( trimChars, TrimTail );
        }

        // Creates a new string with the characters copied in from ptr. If
        // ptr is null, a 0-length string (like Strand.Empty) is returned.
        //
        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ CLSCompliant( false ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern unsafe Strand( char* value );

        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ CLSCompliant( false ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern unsafe Strand( char* value, int startIndex, int length );

        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ CLSCompliant( false ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern unsafe Strand( sbyte* value );

        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ CLSCompliant( false ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern unsafe Strand( sbyte* value, int startIndex, int length );

        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ CLSCompliant( false ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern unsafe Strand( sbyte* value, int startIndex, int length, Encoding enc );

        [ SecurityCritical ] // auto-generated
        private static unsafe Strand CreateString( sbyte* value, int startIndex, int length, Encoding enc ) {
            if ( enc == null ) {
                return new Strand( value, startIndex, length ); // default to ANSI
            }

            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_NeedNonNegNum" ) );
            }
            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }
            if ( ( value + startIndex ) < value ) {
                // overflow check
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_PartialWCHAR" ) );
            }

            var b = new byte[length];

            try {
                Buffer.Memcpy( b, 0, ( byte* ) value, startIndex, length );
            }
            catch ( NullReferenceException ) {
                // If we got a NullReferencException. It means the pointer or 
                // the index is out of range
                throw new ArgumentOutOfRangeException( "value", Environment.GetResourceString( "ArgumentOutOfRange_PartialWCHAR" ) );
            }

            return enc.GetString( b );
        }

        // Helper for encodings so they can talk to our buffer directly
        // stringLength must be the exact size we'll expect
        [ SecurityCritical ] // auto-generated
        internal static unsafe Strand CreateStringFromEncoding( byte* bytes, int byteLength, Encoding encoding ) {
            Contract.Requires( bytes != null );
            Contract.Requires( byteLength >= 0 );

            // Get our string length
            var stringLength = encoding.GetCharCount( bytes, byteLength, null );
            Contract.Assert( stringLength >= 0, "stringLength >= 0" );

            // They gave us an empty string if they needed one
            // 0 bytelength might be possible if there's something in an encoder
            if ( stringLength == 0 ) {
                return Empty;
            }

            var s = FastAllocateString( stringLength );
            fixed ( char* pTempChars = &s.m_firstChar ) {
                var doubleCheck = encoding.GetChars( bytes, byteLength, pTempChars, stringLength, null );
                Contract.Assert( stringLength == doubleCheck, "Expected encoding.GetChars to return same length as encoding.GetCharCount" );
            }

            return s;
        }

        [ SecuritySafeCritical ] // auto-generated
        internal unsafe int ConvertToAnsi( byte* pbNativeBuffer, int cbNativeBuffer, bool fBestFit, bool fThrowOnUnmappableChar ) {
            Contract.Assert( cbNativeBuffer >= ( Length + 1 ) * Marshal.SystemMaxDBCSCharSize, "Insufficient buffer length passed to ConvertToAnsi" );

            const uint CP_ACP = 0;
            int nb;

            const uint WC_NO_BEST_FIT_CHARS = 0x00000400;

            var flgs = ( fBestFit ? 0 : WC_NO_BEST_FIT_CHARS );
            uint DefaultCharUsed = 0;

            fixed ( char* pwzChar = &this.m_firstChar ) {
                nb = Win32Native.WideCharToMultiByte( CP_ACP, flgs, pwzChar, this.Length, pbNativeBuffer, cbNativeBuffer, IntPtr.Zero, ( fThrowOnUnmappableChar ? new IntPtr( &DefaultCharUsed ) : IntPtr.Zero ) );
            }

            if ( 0 != DefaultCharUsed ) {
                throw new ArgumentException( Environment.GetResourceString( "Interop_Marshal_Unmappable_Char" ) );
            }

            pbNativeBuffer[ nb ] = 0;
            return nb;
        }

        // Normalization Methods
        // These just wrap calls to Normalization class
        public bool IsNormalized() {
#if !FEATURE_NORM_IDNA_ONLY
            // Default to Form C
            return IsNormalized( NormalizationForm.FormC );
#else
    // Default to Form IDNA
            return IsNormalized((NormalizationForm)ExtendedNormalizationForms.FormIdna);
#endif
        }

        [ SecuritySafeCritical ] // auto-generated
        public bool IsNormalized( NormalizationForm normalizationForm ) {
#if !FEATURE_NORM_IDNA_ONLY
            if ( this.IsFastSort() ) {
                // If its FastSort && one of the 4 main forms, then its already normalized
                if ( normalizationForm == NormalizationForm.FormC || normalizationForm == NormalizationForm.FormKC || normalizationForm == NormalizationForm.FormD || normalizationForm == NormalizationForm.FormKD ) {
                    return true;
                }
            }
#endif // !FEATURE_NORM_IDNA_ONLY            
            return Normalization.IsNormalized( this, normalizationForm );
        }

        public Strand Normalize() {
#if !FEATURE_NORM_IDNA_ONLY
            // Default to Form C
            return Normalize( NormalizationForm.FormC );
#else
    // Default to Form IDNA
            return Normalize((NormalizationForm)ExtendedNormalizationForms.FormIdna);
#endif
        }

        [ SecuritySafeCritical ] // auto-generated
        public Strand Normalize( NormalizationForm normalizationForm ) {
#if !FEATURE_NORM_IDNA_ONLY
            if ( this.IsAscii() ) {
                // If its FastSort && one of the 4 main forms, then its already normalized
                if ( normalizationForm == NormalizationForm.FormC || normalizationForm == NormalizationForm.FormKC || normalizationForm == NormalizationForm.FormD || normalizationForm == NormalizationForm.FormKD ) {
                    return this;
                }
            }
#endif // !FEATURE_NORM_IDNA_ONLY            
            return Normalization.Normalize( this, normalizationForm );
        }

        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal static extern Strand FastAllocateString( int length );

        [ SecuritySafeCritical ] // auto-generated
        private static unsafe void FillStringChecked( Strand dest, int destPos, Strand src ) {
            Contract.Requires( dest != null );
            Contract.Requires( src != null );
            if ( src.Length > dest.Length - destPos ) {
                throw new IndexOutOfRangeException();
            }

            fixed ( char* pDest = &dest.m_firstChar ) {
                fixed ( char* pSrc = &src.m_firstChar ) {
                    wstrcpy( pDest + destPos, pSrc, src.Length );
                }
            }
        }

        // Creates a new string from the characters in a subarray.  The new string will
        // be created from the characters in value between startIndex and
        // startIndex + length - 1.
        //
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern Strand( char[] value, int startIndex, int length );

        // Creates a new string from the characters in a subarray.  The new string will be
        // created from the characters in value.
        //

        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern Strand( char[] value );

        [ SecurityCritical ] // auto-generated
        internal static unsafe void wstrcpy( char* dmem, char* smem, int charCount ) {
            Buffer.Memcpy( ( byte* ) dmem, ( byte* ) smem, charCount * 2 ); // 2 used everywhere instead of sizeof(char)
        }

        [ SecuritySafeCritical ] // auto-generated
        private Strand CtorCharArray( char[] value ) {
            if ( value != null && value.Length != 0 ) {
                var result = FastAllocateString( value.Length );

                unsafe {
                    fixed ( char* dest = result, source = value ) {
                        wstrcpy( dest, source, value.Length );
                    }
                }
                return result;
            }
            return Empty;
        }

        [ SecuritySafeCritical ] // auto-generated
        private Strand CtorCharArrayStartLength( char[] value, int startIndex, int length ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }

            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }

            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_NegativeLength" ) );
            }

            if ( startIndex > value.Length - length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( length > 0 ) {
                var result = FastAllocateString( length );

                unsafe {
                    fixed ( char* dest = result, source = value ) {
                        wstrcpy( dest, source + startIndex, length );
                    }
                }
                return result;
            }
            return Empty;
        }

        [ SecuritySafeCritical ] // auto-generated
        private Strand CtorCharCount( char c, int count ) {
            if ( count > 0 ) {
                var result = FastAllocateString( count );
                unsafe {
                    fixed ( char* dest = result ) {
                        var dmem = dest;
                        while ( ( ( uint ) dmem & 3 ) != 0 && count > 0 ) {
                            *dmem++ = c;
                            count--;
                        }
                        var cc = ( uint ) ( ( c << 16 ) | c );
                        if ( count >= 4 ) {
                            count -= 4;
                            do {
                                ( ( uint* ) dmem )[ 0 ] = cc;
                                ( ( uint* ) dmem )[ 1 ] = cc;
                                dmem += 4;
                                count -= 4;
                            } while ( count >= 0 );
                        }
                        if ( ( count & 2 ) != 0 ) {
                            ( ( uint* ) dmem )[ 0 ] = cc;
                            dmem += 2;
                        }
                        if ( ( count & 1 ) != 0 ) {
                            dmem[ 0 ] = c;
                        }
                    }
                }
                return result;
            }
            if ( count == 0 ) {
                return Empty;
            }
            throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_MustBeNonNegNum", "count" ) );
        }

        [ SecurityCritical ] // auto-generated
#if !FEATURE_CORECLR
#endif //!FEATURE_CORECLR
        private static unsafe int wcslen( char* ptr ) {
            var end = ptr;

            // The following code is (somewhat surprisingly!) significantly faster than a naive loop,
            // at least on x86 and the current jit.

            // First make sure our pointer is aligned on a dword boundary
            while ( ( ( uint ) end & 3 ) != 0 && *end != 0 ) {
                end++;
            }
            if ( *end != 0 ) {
                // The loop condition below works because if "end[0] & end[1]" is non-zero, that means
                // neither operand can have been zero. If is zero, we have to look at the operands individually,
                // but we hope this going to fairly rare.

                // In general, it would be incorrect to access end[1] if we haven't made sure
                // end[0] is non-zero. However, we know the ptr has been aligned by the loop above
                // so end[0] and end[1] must be in the same page, so they're either both accessible, or both not.

                while ( ( end[ 0 ] & end[ 1 ] ) != 0 || ( end[ 0 ] != 0 && end[ 1 ] != 0 ) ) {
                    end += 2;
                }
            }
            // finish up with the naive loop
            for ( ; *end != 0; end++ ) {
                ;
            }

            var count = ( int ) ( end - ptr );

            return count;
        }

        [ SecurityCritical ] // auto-generated
        private unsafe Strand CtorCharPtr( char* ptr ) {
            if ( ptr == null ) {
                return Empty;
            }

#if !FEATURE_PAL
            if ( ptr < ( char* ) 64000 ) {
                throw new ArgumentException( Environment.GetResourceString( "Arg_MustBeStringPtrNotAtom" ) );
            }
#endif // FEATURE_PAL

            Contract.Assert( this == null, "this == null" ); // this is the string constructor, we allocate it

            try {
                var count = wcslen( ptr );
                if ( count == 0 ) {
                    return Empty;
                }

                var result = FastAllocateString( count );
                fixed ( char* dest = result ) {
                    wstrcpy( dest, ptr, count );
                }
                return result;
            }
            catch ( NullReferenceException ) {
                throw new ArgumentOutOfRangeException( "ptr", Environment.GetResourceString( "ArgumentOutOfRange_PartialWCHAR" ) );
            }
        }

        [ SecurityCritical ] // auto-generated
#if !FEATURE_CORECLR
#endif //!FEATURE_CORECLR
        private unsafe Strand CtorCharPtrStartLength( char* ptr, int startIndex, int length ) {
            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_NegativeLength" ) );
            }

            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }

            Contract.Assert( this == null, "this == null" ); // this is the string constructor, we allocate it

            var pFrom = ptr + startIndex;
            if ( pFrom < ptr ) {
                // This means that the pointer operation has had an overflow
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_PartialWCHAR" ) );
            }

            if ( length == 0 ) {
                return Empty;
            }

            var result = FastAllocateString( length );

            try {
                fixed ( char* dest = result ) {
                    wstrcpy( dest, pFrom, length );
                }
                return result;
            }
            catch ( NullReferenceException ) {
                throw new ArgumentOutOfRangeException( "ptr", Environment.GetResourceString( "ArgumentOutOfRange_PartialWCHAR" ) );
            }
        }

        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern Strand( char c, int count );

        //
        //
        // INSTANCE METHODS
        //
        //

        // Provides a culture-correct string comparison. StrA is compared to StrB
        // to determine whether it is lexicographically less, equal, or greater, and then returns
        // either a negative integer, 0, or a positive integer; respectively.
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int Compare( Strand strA, Strand strB ) {
            return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, strB, CompareOptions.None );
        }

        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a
        // negative integer, 0, or a positive integer is returned; respectively.
        // The case-sensitive option is set by ignoreCase
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int Compare( Strand strA, Strand strB, bool ignoreCase ) {
            if ( ignoreCase ) {
                return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, strB, CompareOptions.IgnoreCase );
            }
            return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, strB, CompareOptions.None );
        }

        // Provides a more flexible function for string comparision. See StringComparison 
        // for meaning of different comparisonType.
        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        public static int Compare( Strand strA, Strand strB, StringComparison comparisonType ) {
            // Single comparison to check if comparisonType is within [CurrentCulture .. OrdinalIgnoreCase]
            if ( ( uint ) ( comparisonType - StringComparison.CurrentCulture ) > StringComparison.OrdinalIgnoreCase - StringComparison.CurrentCulture ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( strA == ( Object ) strB ) {
                return 0;
            }

            //they can't both be null;
            if ( strA == null ) {
                return -1;
            }

            if ( strB == null ) {
                return 1;
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, strB, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, strB, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare( strA, strB, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare( strA, strB, CompareOptions.IgnoreCase );

                case StringComparison.Ordinal:
                    // Most common case: first character is different.
                    if ( ( strA.m_firstChar - strB.m_firstChar ) != 0 ) {
                        return strA.m_firstChar - strB.m_firstChar;
                    }

                    return CompareOrdinalHelper( strA, strB );

                case StringComparison.OrdinalIgnoreCase:
                    // If both strings are ASCII strings, we can take the fast path.
                    if ( strA.IsAscii() && strB.IsAscii() ) {
                        return ( CompareOrdinalIgnoreCaseHelper( strA, strB ) );
                    }
                    // Take the slow path.                
                    return TextInfo.CompareOrdinalIgnoreCase( strA, strB );

                default:
                    throw new NotSupportedException( Environment.GetResourceString( "NotSupported_StringComparison" ) );
            }
        }

        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a
        // negative integer, 0, or a positive integer is returned; respectively.
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int Compare( Strand strA, Strand strB, CultureInfo culture, CompareOptions options ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }

            return culture.CompareInfo.Compare( strA, strB, options );
        }

        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a
        // negative integer, 0, or a positive integer is returned; respectively.
        // The case-sensitive option is set by ignoreCase, and the culture is set
        // by culture
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int Compare( Strand strA, Strand strB, bool ignoreCase, CultureInfo culture ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }

            if ( ignoreCase ) {
                return culture.CompareInfo.Compare( strA, strB, CompareOptions.IgnoreCase );
            }
            return culture.CompareInfo.Compare( strA, strB, CompareOptions.None );
        }

        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length count is compared with the substring of strB
        // beginning at indexB of the same length.
        //
        [ Pure ]
        public static int Compare( Strand strA, int indexA, Strand strB, int indexB, int length ) {
            var lengthA = length;
            var lengthB = length;

            if ( strA != null ) {
                if ( strA.Length - indexA < lengthA ) {
                    lengthA = ( strA.Length - indexA );
                }
            }

            if ( strB != null ) {
                if ( strB.Length - indexB < lengthB ) {
                    lengthB = ( strB.Length - indexB );
                }
            }
            return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None );
        }

        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length count is compared with the substring of strB
        // beginning at indexB of the same length.  Case sensitivity is determined by the ignoreCase boolean.
        //
        [ Pure ]
        public static int Compare( Strand strA, int indexA, Strand strB, int indexB, int length, bool ignoreCase ) {
            var lengthA = length;
            var lengthB = length;

            if ( strA != null ) {
                if ( strA.Length - indexA < lengthA ) {
                    lengthA = ( strA.Length - indexA );
                }
            }

            if ( strB != null ) {
                if ( strB.Length - indexB < lengthB ) {
                    lengthB = ( strB.Length - indexB );
                }
            }

            if ( ignoreCase ) {
                return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase );
            }
            return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None );
        }

        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length length is compared with the substring of strB
        // beginning at indexB of the same length.  Case sensitivity is determined by the ignoreCase boolean,
        // and the culture is set by culture.
        //
        [ Pure ]
        public static int Compare( Strand strA, int indexA, Strand strB, int indexB, int length, bool ignoreCase, CultureInfo culture ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }

            var lengthA = length;
            var lengthB = length;

            if ( strA != null ) {
                if ( strA.Length - indexA < lengthA ) {
                    lengthA = ( strA.Length - indexA );
                }
            }

            if ( strB != null ) {
                if ( strB.Length - indexB < lengthB ) {
                    lengthB = ( strB.Length - indexB );
                }
            }

            if ( ignoreCase ) {
                return culture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase );
            }
            return culture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None );
        }

        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length length is compared with the substring of strB
        // beginning at indexB of the same length.
        //
        [ Pure ]
        public static int Compare( Strand strA, int indexA, Strand strB, int indexB, int length, CultureInfo culture, CompareOptions options ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }

            var lengthA = length;
            var lengthB = length;

            if ( strA != null ) {
                if ( strA.Length - indexA < lengthA ) {
                    lengthA = ( strA.Length - indexA );
                }
            }

            if ( strB != null ) {
                if ( strB.Length - indexB < lengthB ) {
                    lengthB = ( strB.Length - indexB );
                }
            }

            return culture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, options );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        public static int Compare( Strand strA, int indexA, Strand strB, int indexB, int length, StringComparison comparisonType ) {
            if ( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( strA == null || strB == null ) {
                if ( strA == ( Object ) strB ) {
                    //they're both null;
                    return 0;
                }

                return ( strA == null ) ? -1 : 1; //-1 if A is null, 1 if B is null.
            }

            // @
            if ( length < 0 ) {
                throw new ArgumentOutOfRangeException( "length", Environment.GetResourceString( "ArgumentOutOfRange_NegativeLength" ) );
            }

            if ( indexA < 0 ) {
                throw new ArgumentOutOfRangeException( "indexA", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( indexB < 0 ) {
                throw new ArgumentOutOfRangeException( "indexB", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( strA.Length - indexA < 0 ) {
                throw new ArgumentOutOfRangeException( "indexA", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( strB.Length - indexB < 0 ) {
                throw new ArgumentOutOfRangeException( "indexB", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( ( length == 0 ) || ( ( strA == strB ) && ( indexA == indexB ) ) ) {
                return 0;
            }

            var lengthA = length;
            var lengthB = length;

            if ( strA != null ) {
                if ( strA.Length - indexA < lengthA ) {
                    lengthA = ( strA.Length - indexA );
                }
            }

            if ( strB != null ) {
                if ( strB.Length - indexB < lengthB ) {
                    lengthB = ( strB.Length - indexB );
                }
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare( strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase );

                case StringComparison.Ordinal:
                    // 
                    return nativeCompareOrdinalEx( strA, indexA, strB, indexB, length );

                case StringComparison.OrdinalIgnoreCase:
                    return ( TextInfo.CompareOrdinalIgnoreCaseEx( strA, indexA, strB, indexB, lengthA, lengthB ) );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ) );
            }
        }

        // Compares this object to another object, returning an integer that
        // indicates the relationship. This method returns a value less than 0 if this is less than value, 0
        // if this is equal to value, or a value greater than 0
        // if this is greater than value.  Strings are considered to be
        // greater than all non-Strand objects.  Note that this means sorted 
        // arrays would contain nulls, other objects, then Strings in that order.
        //
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        [ Pure ]
        public int CompareTo( Object value ) {
            if ( value == null ) {
                return 1;
            }

            if ( !( value is Strand ) ) {
                throw new ArgumentException( Environment.GetResourceString( "Arg_MustBeString" ) );
            }

            return Compare( this, ( Strand ) value, StringComparison.CurrentCulture );
        }

        // Determines the sorting relation of StrB to the current instance.
        //
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        [ Pure ]
        public int CompareTo( Strand strB ) {
            if ( strB == null ) {
                return 1;
            }

            return CultureInfo.CurrentCulture.CompareInfo.Compare( this, strB, 0 );
        }

        // Compares strA and strB using an ordinal (code-point) comparison.
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int CompareOrdinal( Strand strA, Strand strB ) {
            if ( strA == ( Object ) strB ) {
                return 0;
            }

            //they can't both be null;
            if ( strA == null ) {
                return -1;
            }

            if ( strB == null ) {
                return 1;
            }

            // Most common case, first character is different.
            if ( ( strA.m_firstChar - strB.m_firstChar ) != 0 ) {
                return strA.m_firstChar - strB.m_firstChar;
            }

            // 
            return CompareOrdinalHelper( strA, strB );
        }

        // Compares strA and strB using an ordinal (code-point) comparison.
        //
        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static int CompareOrdinal( Strand strA, int indexA, Strand strB, int indexB, int length ) {
            if ( strA == null || strB == null ) {
                if ( strA == ( Object ) strB ) {
                    //they're both null;
                    return 0;
                }

                return ( strA == null ) ? -1 : 1; //-1 if A is null, 1 if B is null.
            }

            return nativeCompareOrdinalEx( strA, indexA, strB, indexB, length );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public bool Contains( string value ) {
            return ( IndexOf( value, StringComparison.Ordinal ) >= 0 );
        }

        // Determines whether a specified string is a suffix of the the current instance.
        //
        // The case-sensitive and culture-sensitive option is set by options,
        // and the default culture is used.
        //        
        [ Pure ]
        public Boolean EndsWith( Strand value ) {
            return EndsWith( value, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ComVisible( false ) ]
        public Boolean EndsWith( Strand value, StringComparison comparisonType ) {
            if ( ( Object ) value == null ) {
                throw new ArgumentNullException( "value" );
            }

            if ( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( this == ( Object ) value ) {
                return true;
            }

            if ( value.Length == 0 ) {
                return true;
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix( this, value, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix( this, value, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix( this, value, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix( this, value, CompareOptions.IgnoreCase );

                case StringComparison.Ordinal:
                    return this.Length < value.Length ? false : ( nativeCompareOrdinalEx( this, this.Length - value.Length, value, 0, value.Length ) == 0 );

                case StringComparison.OrdinalIgnoreCase:
                    return this.Length < value.Length ? false : ( TextInfo.CompareOrdinalIgnoreCaseEx( this, this.Length - value.Length, value, 0, value.Length, value.Length ) == 0 );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

        [ Pure ]
        public Boolean EndsWith( Strand value, Boolean ignoreCase, CultureInfo culture ) {
            if ( null == value ) {
                throw new ArgumentNullException( "value" );
            }

            if ( this == ( object ) value ) {
                return true;
            }

            CultureInfo referenceCulture;
            if ( culture == null ) {
                referenceCulture = ( LegacyMode ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture );
            }
            else {
                referenceCulture = culture;
            }

            return referenceCulture.CompareInfo.IsSuffix( this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        internal bool EndsWith( char value ) {
            var thisLen = this.Length;
            if ( thisLen != 0 ) {
                if ( this[ thisLen - 1 ] == value ) {
                    return true;
                }
            }
            return false;
        }

        // Returns the index of the first occurance of value in the current instance.
        // The search starts at startIndex and runs thorough the next count characters.
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOf( char value ) {
            return IndexOf( value, 0, this.Length );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOf( char value, int startIndex ) {
            return IndexOf( value, startIndex, this.Length - startIndex );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern int IndexOf( char value, int startIndex, int count );

        // Returns the index of the first occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex-1. [startIndex,endIndex).
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOfAny( char[] anyOf ) {
            return IndexOfAny( anyOf, 0, this.Length );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOfAny( char[] anyOf, int startIndex ) {
            return IndexOfAny( anyOf, startIndex, this.Length - startIndex );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern int IndexOfAny( char[] anyOf, int startIndex, int count );

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // the first character of this string, it is case-sensitive and ordinal (code-point)
        // comparison is used.
        //
        [ Pure ]
        public int IndexOf( Strand value ) {
            return IndexOf( value, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // startIndex, it is case-sensitive and ordinal (code-point) comparison is used.
        //
        [ Pure ]
        public int IndexOf( Strand value, int startIndex ) {
            return IndexOf( value, startIndex, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // startIndex, ends at endIndex and ordinal (code-point) comparison is used.
        //
        [ Pure ]
        public int IndexOf( Strand value, int startIndex, int count ) {
            if ( startIndex < 0 || startIndex > this.Length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( count < 0 || count > this.Length - startIndex ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_Count" ) );
            }

            return IndexOf( value, startIndex, count, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOf( Strand value, StringComparison comparisonType ) {
            return IndexOf( value, 0, this.Length, comparisonType );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int IndexOf( Strand value, int startIndex, StringComparison comparisonType ) {
            return IndexOf( value, startIndex, this.Length - startIndex, comparisonType );
        }

        [ Pure ]
        [ SecuritySafeCritical ]
        public int IndexOf( Strand value, int startIndex, int count, StringComparison comparisonType ) {
            // Validate inputs
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }

            if ( startIndex < 0 || startIndex > this.Length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            if ( count < 0 || startIndex > this.Length - count ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_Count" ) );
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );

                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.Ordinal );

                case StringComparison.OrdinalIgnoreCase:
                    if ( value.IsAscii() && this.IsAscii() ) {
                        return CultureInfo.InvariantCulture.CompareInfo.IndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );
                    }
                    return TextInfo.IndexOfStringOrdinalIgnoreCase( this, value, startIndex, count );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

        // Returns the index of the last occurance of value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int LastIndexOf( char value ) {
            return LastIndexOf( value, this.Length - 1, this.Length );
        }

        [ Pure ]
        public int LastIndexOf( char value, int startIndex ) {
            return LastIndexOf( value, startIndex, startIndex + 1 );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern int LastIndexOf( char value, int startIndex, int count );

        // Returns the index of the last occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //

        //ForceInline ... Jit can't recognize Strand.get_Length to determine that this is "fluff"
        [ Pure ]
        public int LastIndexOfAny( char[] anyOf ) {
            return LastIndexOfAny( anyOf, this.Length - 1, this.Length );
        }

        //ForceInline ... Jit can't recognize Strand.get_Length to determine that this is "fluff"
        [ Pure ]
        public int LastIndexOfAny( char[] anyOf, int startIndex ) {
            return LastIndexOfAny( anyOf, startIndex, startIndex + 1 );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        public extern int LastIndexOfAny( char[] anyOf, int startIndex, int count );

        // Returns the index of the last occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        [ Pure ]
        public int LastIndexOf( Strand value ) {
            return LastIndexOf( value, this.Length - 1, this.Length, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
        public int LastIndexOf( Strand value, int startIndex ) {
            return LastIndexOf( value, startIndex, startIndex + 1, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
        public int LastIndexOf( Strand value, int startIndex, int count ) {
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_Count" ) );
            }

            return LastIndexOf( value, startIndex, count, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public int LastIndexOf( Strand value, StringComparison comparisonType ) {
            return LastIndexOf( value, this.Length - 1, this.Length, comparisonType );
        }

        [ Pure ]
        public int LastIndexOf( Strand value, int startIndex, StringComparison comparisonType ) {
            return LastIndexOf( value, startIndex, startIndex + 1, comparisonType );
        }

        [ Pure ]
        [ SecuritySafeCritical ]
        public int LastIndexOf( Strand value, int startIndex, int count, StringComparison comparisonType ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }

            // Special case for 0 length input strings
            if ( this.Length == 0 && ( startIndex == -1 || startIndex == 0 ) ) {
                return ( value.Length == 0 ) ? 0 : -1;
            }

            // Now after handling empty strings, make sure we're not out of range
            if ( startIndex < 0 || startIndex > this.Length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_Index" ) );
            }

            // Make sure that we allow startIndex == this.Length
            if ( startIndex == this.Length ) {
                startIndex--;
                if ( count > 0 ) {
                    count--;
                }

                // If we are looking for nothing, just return 0
                if ( value.Length == 0 && count >= 0 && startIndex - count + 1 >= 0 ) {
                    return startIndex;
                }
            }

            // 2nd half of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if ( count < 0 || startIndex - count + 1 < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_Count" ) );
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );
                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.Ordinal );

                case StringComparison.OrdinalIgnoreCase:
                    if ( value.IsAscii() && this.IsAscii() ) {
                        return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf( this, value, startIndex, count, CompareOptions.IgnoreCase );
                    }
                    return TextInfo.LastIndexOfStringOrdinalIgnoreCase( this, value, startIndex, count );
                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

        //
        //
        [ Pure ]
        public Strand PadLeft( int totalWidth ) {
            return PadHelper( totalWidth, ' ', false );
        }

        [ Pure ]
        public Strand PadLeft( int totalWidth, char paddingChar ) {
            return PadHelper( totalWidth, paddingChar, false );
        }

        [ Pure ]
        public Strand PadRight( int totalWidth ) {
            return PadHelper( totalWidth, ' ', true );
        }

        [ Pure ]
        public Strand PadRight( int totalWidth, char paddingChar ) {
            return PadHelper( totalWidth, paddingChar, true );
        }

        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        private extern Strand PadHelper( int totalWidth, char paddingChar, bool isRightPadded );

        // Determines whether a specified string is a prefix of the current instance
        //
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Boolean StartsWith( Strand value ) {
            if ( ( Object ) value == null ) {
                throw new ArgumentNullException( "value" );
            }

            return StartsWith( value, ( LegacyMode ? StringComparison.Ordinal : StringComparison.CurrentCulture ) );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        [ ComVisible( false ) ]
        public Boolean StartsWith( Strand value, StringComparison comparisonType ) {
            if ( ( Object ) value == null ) {
                throw new ArgumentNullException( "value" );
            }

            if ( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase ) {
                throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }

            if ( this == ( Object ) value ) {
                return true;
            }

            if ( value.Length == 0 ) {
                return true;
            }

            switch ( comparisonType ) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix( this, value, CompareOptions.None );

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix( this, value, CompareOptions.IgnoreCase );

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix( this, value, CompareOptions.None );

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix( this, value, CompareOptions.IgnoreCase );

                case StringComparison.Ordinal:
                    if ( this.Length < value.Length ) {
                        return false;
                    }
                    return ( nativeCompareOrdinalEx( this, 0, value, 0, value.Length ) == 0 );

                case StringComparison.OrdinalIgnoreCase:
                    if ( this.Length < value.Length ) {
                        return false;
                    }

                    return ( TextInfo.CompareOrdinalIgnoreCaseEx( this, 0, value, 0, value.Length, value.Length ) == 0 );

                default:
                    throw new ArgumentException( Environment.GetResourceString( "NotSupported_StringComparison" ), "comparisonType" );
            }
        }

        [ Pure ]
        public Boolean StartsWith( Strand value, Boolean ignoreCase, CultureInfo culture ) {
            if ( null == value ) {
                throw new ArgumentNullException( "value" );
            }

            if ( this == ( object ) value ) {
                return true;
            }

            CultureInfo referenceCulture;
            if ( culture == null ) {
                referenceCulture = ( LegacyMode ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture );
            }
            else {
                referenceCulture = culture;
            }

            return referenceCulture.CompareInfo.IsPrefix( this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None );
        }

        // Creates a copy of this string in lower case.
        [ Pure ]
        public Strand ToLower() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this.ToLower( LegacyMode ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture );
        }

        // Creates a copy of this string in lower case.  The culture is set by culture.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand ToLower( CultureInfo culture ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return culture.TextInfo.ToLower( this );
        }

        // Creates a copy of this string in lower case based on invariant culture.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand ToLowerInvariant() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this.ToLower( CultureInfo.InvariantCulture );
        }

        // Creates a copy of this string in upper case.
        [ Pure ]
        public Strand ToUpper() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this.ToUpper( LegacyMode ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture );
        }

        // Creates a copy of this string in upper case.  The culture is set by culture.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand ToUpper( CultureInfo culture ) {
            if ( culture == null ) {
                throw new ArgumentNullException( "culture" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return culture.TextInfo.ToUpper( this );
        }

        //Creates a copy of this string in upper case based on invariant culture.
        [ Pure ]
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand ToUpperInvariant() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this.ToUpper( CultureInfo.InvariantCulture );
        }

        // Returns this string.
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public override Strand ToString() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this;
        }

#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand ToString( IFormatProvider provider ) {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return this;
        }

        // Method required for the ICloneable interface.
        // There's no point in cloning a string since they're immutable, so we simply return this.
        public Object Clone() {
            Contract.Ensures( Contract.Result< Object >() != null );

            return this;
        }

        private static bool IsBOMWhitespace( char c ) {
#if FEATURE_LEGACYNETCF
            if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8 && c == '\xFEFF')
            {
                // Dev11 450846 quirk:
                // NetCF treats the BOM as a whitespace character when performing trim operations.
                return true;
            }
            else
#endif
            {
                return false;
            }
        }

        // Trims the whitespace from both ends of the string.  Whitespace is defined by
        // Char.IsWhiteSpace.
        //
        [ Pure ]
        public Strand Trim() {
            Contract.Ensures( Contract.Result< Strand >() != null );

            return TrimHelper( TrimBoth );
        }

        [ SecuritySafeCritical ] // auto-generated
        private Strand TrimHelper( int trimType ) {
            //end will point to the first non-trimmed character on the right
            //start will point to the first non-trimmed character on the Left
            var end = this.Length - 1;
            var start = 0;

            //Trim specified characters.
            if ( trimType != TrimTail ) {
                for ( start = 0; start < this.Length; start++ ) {
                    if ( !Char.IsWhiteSpace( this[ start ] ) && !IsBOMWhitespace( this[ start ] ) ) {
                        break;
                    }
                }
            }

            if ( trimType != TrimHead ) {
                for ( end = Length - 1; end >= start; end-- ) {
                    if ( !Char.IsWhiteSpace( this[ end ] ) && !IsBOMWhitespace( this[ start ] ) ) {
                        break;
                    }
                }
            }

            return CreateTrimmedString( start, end );
        }

        [ SecuritySafeCritical ] // auto-generated
        private Strand TrimHelper( char[] trimChars, int trimType ) {
            //end will point to the first non-trimmed character on the right
            //start will point to the first non-trimmed character on the Left
            var end = this.Length - 1;
            var start = 0;

            //Trim specified characters.
            if ( trimType != TrimTail ) {
                for ( start = 0; start < this.Length; start++ ) {
                    var i = 0;
                    var ch = this[ start ];
                    for ( i = 0; i < trimChars.Length; i++ ) {
                        if ( trimChars[ i ] == ch ) {
                            break;
                        }
                    }
                    if ( i == trimChars.Length ) {
                        // the character is not white space
                        break;
                    }
                }
            }

            if ( trimType != TrimHead ) {
                for ( end = Length - 1; end >= start; end-- ) {
                    var i = 0;
                    var ch = this[ end ];
                    for ( i = 0; i < trimChars.Length; i++ ) {
                        if ( trimChars[ i ] == ch ) {
                            break;
                        }
                    }
                    if ( i == trimChars.Length ) {
                        // the character is not white space
                        break;
                    }
                }
            }

            return CreateTrimmedString( start, end );
        }

        [ SecurityCritical ] // auto-generated
        private Strand CreateTrimmedString( int start, int end ) {
            //Create a new STRINGREF and initialize it from the range determined above.
            var len = end - start + 1;
            if ( len == this.Length ) {
                // Don't allocate a new string as the trimmed string has not changed.
                return this;
            }

            if ( len == 0 ) {
                return Empty;
            }
            return InternalSubString( start, len );
        }

        [ SecuritySafeCritical ] // auto-generated
        public Strand Insert( int startIndex, Strand value ) {
            if ( value == null ) {
                throw new ArgumentNullException( "value" );
            }
            if ( startIndex < 0 || startIndex > this.Length ) {
                throw new ArgumentOutOfRangeException( "startIndex" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == this.Length + value.Length );

            var oldLength = Length;
            var insertLength = value.Length;
            // In case this computation overflows, newLength will be negative and FastAllocateString throws OutOfMemoryException
            var newLength = oldLength + insertLength;
            if ( newLength == 0 ) {
                return Empty;
            }
            var result = FastAllocateString( newLength );
            unsafe {
                fixed ( char* srcThis = &m_firstChar ) {
                    fixed ( char* srcInsert = &value.m_firstChar ) {
                        fixed ( char* dst = &result.m_firstChar ) {
                            wstrcpy( dst, srcThis, startIndex );
                            wstrcpy( dst + startIndex, srcInsert, insertLength );
                            wstrcpy( dst + startIndex + insertLength, srcThis + startIndex, oldLength - startIndex );
                        }
                    }
                }
            }
            return result;
        }

        // Replaces all instances of oldChar with newChar.
        //
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        private extern Strand ReplaceInternal( char oldChar, char newChar );

        public Strand Replace( char oldChar, char newChar ) {
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == this.Length );

            return ReplaceInternal( oldChar, newChar );
        }

        // This method contains the same functionality as StringBuilder Replace. The only difference is that
        // a new Strand has to be allocated since Strings are immutable
        [ SecuritySafeCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        private extern Strand ReplaceInternal( Strand oldValue, Strand newValue );

#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public Strand Replace( Strand oldValue, Strand newValue ) {
            if ( oldValue == null ) {
                throw new ArgumentNullException( "oldValue" );
            }
            // Note that if newValue is null, we treat it like Strand.Empty.
            Contract.Ensures( Contract.Result< Strand >() != null );

            string s = ReplaceInternal( oldValue, newValue );
#if FEATURE_LEGACYNETCF
            if (CompatibilitySwitches.IsAppEarlierThanWindowsPhoneMango)
            {
                // Dev11 453753 quirk
                // for pre-Mango this function had a bug that would cause it to
                // drop all characters to the right of the first embedded NULL.
                // this was quirked on Mango for pre-Mango apps however for apps
                // targeting Mango the bug was fixed.
                int i = s.IndexOf('\0');
                if (i > 0)
                    return s.Substring(0, i);
                else
                    return s;
            }
            else
#endif
            {
                return s;
            }
        }

        [ SecuritySafeCritical ] // auto-generated
        public Strand Remove( int startIndex, int count ) {
            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_NegativeCount" ) );
            }
            if ( count > Length - startIndex ) {
                throw new ArgumentOutOfRangeException( "count", Environment.GetResourceString( "ArgumentOutOfRange_IndexCount" ) );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == this.Length - count );

            var newLength = Length - count;
            if ( newLength == 0 ) {
                return Empty;
            }
            var result = FastAllocateString( newLength );
            unsafe {
                fixed ( char* src = &m_firstChar ) {
                    fixed ( char* dst = &result.m_firstChar ) {
                        wstrcpy( dst, src, startIndex );
                        wstrcpy( dst + startIndex, src + startIndex + count, newLength - startIndex );
                    }
                }
            }
            return result;
        }

        // a remove that just takes a startindex. 
        public string Remove( int startIndex ) {
            if ( startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndex" ) );
            }

            if ( startIndex >= Length ) {
                throw new ArgumentOutOfRangeException( "startIndex", Environment.GetResourceString( "ArgumentOutOfRange_StartIndexLessThanLength" ) );
            }

            Contract.Ensures( Contract.Result< Strand >() != null );

            return Substring( 0, startIndex );
        }

        public static Strand Format( Strand format, Object arg0 ) {
            if ( format == null ) {
                throw new ArgumentNullException( "format" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return Format( null, format, arg0 );
        }

        public static Strand Format( Strand format, Object arg0, Object arg1 ) {
            if ( format == null ) {
                throw new ArgumentNullException( "format" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return Format( null, format, arg0, arg1 );
        }

        public static Strand Format( Strand format, Object arg0, Object arg1, Object arg2 ) {
            if ( format == null ) {
                throw new ArgumentNullException( "format" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return Format( null, format, arg0, arg1, arg2 );
        }

        public static Strand Format( Strand format, params Object[] args ) {
            if ( format == null || args == null ) {
                throw new ArgumentNullException( ( format == null ) ? "format" : "args" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            return Format( null, format, args );
        }

        public static Strand Format( IFormatProvider provider, Strand format, params Object[] args ) {
            if ( format == null || args == null ) {
                throw new ArgumentNullException( ( format == null ) ? "format" : "args" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            var sb = StringBuilderCache.Acquire( format.Length + args.Length * 8 );
            sb.AppendFormat( provider, format, args );
            return StringBuilderCache.GetStringAndRelease( sb );
        }

        [ SecuritySafeCritical ] // auto-generated
        public static unsafe Strand Copy( Strand str ) {
            if ( str == null ) {
                throw new ArgumentNullException( "str" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            var length = str.Length;

            var result = FastAllocateString( length );

            fixed ( char* dest = &result.m_firstChar ) {
                fixed ( char* src = &str.m_firstChar ) {
                    wstrcpy( dest, src, length );
                }
            }
            return result;
        }

        public static Strand Concat( Object arg0 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );

            if ( arg0 == null ) {
                return Empty;
            }
            return arg0.ToString();
        }

        public static Strand Concat( Object arg0, Object arg1 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );

            if ( arg0 == null ) {
                arg0 = Empty;
            }

            if ( arg1 == null ) {
                arg1 = Empty;
            }
            return Concat( arg0.ToString(), arg1.ToString() );
        }

        public static Strand Concat( Object arg0, Object arg1, Object arg2 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );

            if ( arg0 == null ) {
                arg0 = Empty;
            }

            if ( arg1 == null ) {
                arg1 = Empty;
            }

            if ( arg2 == null ) {
                arg2 = Empty;
            }

            return Concat( arg0.ToString(), arg1.ToString(), arg2.ToString() );
        }

        [ CLSCompliant( false ) ]
        public static Strand Concat( Object arg0, Object arg1, Object arg2, Object arg3, __arglist ) {
            Contract.Ensures( Contract.Result< Strand >() != null );

            Object[] objArgs;
            int argCount;

            var args = new ArgIterator( __arglist );

            //+4 to account for the 4 hard-coded arguments at the beginning of the list.
            argCount = args.GetRemainingCount() + 4;

            objArgs = new Object[argCount];

            //Handle the hard-coded arguments
            objArgs[ 0 ] = arg0;
            objArgs[ 1 ] = arg1;
            objArgs[ 2 ] = arg2;
            objArgs[ 3 ] = arg3;

            //Walk all of the args in the variable part of the argument list.
            for ( var i = 4; i < argCount; i++ ) {
                objArgs[ i ] = TypedReference.ToObject( args.GetNextArg() );
            }

            return Concat( objArgs );
        }

        public static Strand Concat( params Object[] args ) {
            if ( args == null ) {
                throw new ArgumentNullException( "args" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            var sArgs = new Strand[args.Length];
            var totalLength = 0;

            for ( var i = 0; i < args.Length; i++ ) {
                var value = args[ i ];
                sArgs[ i ] = ( ( value == null ) ? ( Empty ) : ( value.ToString() ) );
                if ( sArgs[ i ] == null ) {
                    sArgs[ i ] = Empty; // value.ToString() above could have returned null
                }
                totalLength += sArgs[ i ].Length;
                // check for overflow
                if ( totalLength < 0 ) {
                    throw new OutOfMemoryException();
                }
            }
            return ConcatArray( sArgs, totalLength );
        }

        [ ComVisible( false ) ]
        public static Strand Concat< T >( IEnumerable< T > values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            var result = StringBuilderCache.Acquire();
            using ( var en = values.GetEnumerator() ) {
                while ( en.MoveNext() ) {
                    if ( en.Current != null ) {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        var value = en.Current.ToString();
                        if ( value != null ) {
                            result.Append( value );
                        }
                    }
                }
            }
            return StringBuilderCache.GetStringAndRelease( result );
        }

        [ ComVisible( false ) ]
        public static Strand Concat( IEnumerable< Strand > values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );

            var result = StringBuilderCache.Acquire();
            using ( var en = values.GetEnumerator() ) {
                while ( en.MoveNext() ) {
                    if ( en.Current != null ) {
                        result.Append( en.Current );
                    }
                }
            }
            return StringBuilderCache.GetStringAndRelease( result );
        }

        [ SecuritySafeCritical ] // auto-generated
        public static Strand Concat( Strand str0, Strand str1 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == ( str0 == null ? 0 : str0.Length ) + ( str1 == null ? 0 : str1.Length ) );

            if ( IsNullOrEmpty( str0 ) ) {
                if ( IsNullOrEmpty( str1 ) ) {
                    return Empty;
                }
                return str1;
            }

            if ( IsNullOrEmpty( str1 ) ) {
                return str0;
            }

            var str0Length = str0.Length;

            var result = FastAllocateString( str0Length + str1.Length );

            FillStringChecked( result, 0, str0 );
            FillStringChecked( result, str0Length, str1 );

            return result;
        }

        [ SecuritySafeCritical ] // auto-generated
        public static Strand Concat( Strand str0, Strand str1, Strand str2 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == ( str0 == null ? 0 : str0.Length ) + ( str1 == null ? 0 : str1.Length ) + ( str2 == null ? 0 : str2.Length ) );

            if ( str0 == null && str1 == null && str2 == null ) {
                return Empty;
            }

            if ( str0 == null ) {
                str0 = Empty;
            }

            if ( str1 == null ) {
                str1 = Empty;
            }

            if ( str2 == null ) {
                str2 = Empty;
            }

            var totalLength = str0.Length + str1.Length + str2.Length;

            var result = FastAllocateString( totalLength );
            FillStringChecked( result, 0, str0 );
            FillStringChecked( result, str0.Length, str1 );
            FillStringChecked( result, str0.Length + str1.Length, str2 );

            return result;
        }

        [ SecuritySafeCritical ] // auto-generated
        public static Strand Concat( Strand str0, Strand str1, Strand str2, Strand str3 ) {
            Contract.Ensures( Contract.Result< Strand >() != null );
            Contract.Ensures( Contract.Result< Strand >().Length == ( str0 == null ? 0 : str0.Length ) + ( str1 == null ? 0 : str1.Length ) + ( str2 == null ? 0 : str2.Length ) + ( str3 == null ? 0 : str3.Length ) );

            if ( str0 == null && str1 == null && str2 == null && str3 == null ) {
                return Empty;
            }

            if ( str0 == null ) {
                str0 = Empty;
            }

            if ( str1 == null ) {
                str1 = Empty;
            }

            if ( str2 == null ) {
                str2 = Empty;
            }

            if ( str3 == null ) {
                str3 = Empty;
            }

            var totalLength = str0.Length + str1.Length + str2.Length + str3.Length;

            var result = FastAllocateString( totalLength );
            FillStringChecked( result, 0, str0 );
            FillStringChecked( result, str0.Length, str1 );
            FillStringChecked( result, str0.Length + str1.Length, str2 );
            FillStringChecked( result, str0.Length + str1.Length + str2.Length, str3 );

            return result;
        }

        [ SecuritySafeCritical ] // auto-generated
        private static Strand ConcatArray( Strand[] values, int totalLength ) {
            var result = FastAllocateString( totalLength );
            var currPos = 0;

            for ( var i = 0; i < values.Length; i++ ) {
                Contract.Assert( ( currPos <= totalLength - values[ i ].Length ), "[Strand.ConcatArray](currPos <= totalLength - values[i].Length)" );

                FillStringChecked( result, currPos, values[ i ] );
                currPos += values[ i ].Length;
            }

            return result;
        }

        public static Strand Concat( params Strand[] values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            Contract.Ensures( Contract.Result< Strand >() != null );
            // Spec#: Consider a postcondition saying the length of this string == the sum of each string in array

            var totalLength = 0;

            // Always make a copy to prevent changing the array on another thread.
            var internalValues = new Strand[values.Length];

            for ( var i = 0; i < values.Length; i++ ) {
                string value = values[ i ];
                internalValues[ i ] = ( ( value == null ) ? ( Empty ) : ( value ) );
                totalLength += internalValues[ i ].Length;
                // check for overflow
                if ( totalLength < 0 ) {
                    throw new OutOfMemoryException();
                }
            }

            return ConcatArray( internalValues, totalLength );
        }

        [ SecuritySafeCritical ] // auto-generated
#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public static Strand Intern( Strand str ) {
            if ( str == null ) {
                throw new ArgumentNullException( "str" );
            }
            Contract.Ensures( Contract.Result< Strand >().Length == str.Length );
            Contract.Ensures( str.Equals( Contract.Result< Strand >() ) );

            return Thread.GetDomain().GetOrInternString( str );
        }

        [ Pure ]
        [ SecuritySafeCritical ] // auto-generated
        public static Strand IsInterned( Strand str ) {
            if ( str == null ) {
                throw new ArgumentNullException( "str" );
            }
            Contract.Ensures( Contract.Result< Strand >() == null || Contract.Result< Strand >().Length == str.Length );

            return Thread.GetDomain().IsStringInterned( str );
        }

        //
        // IConvertible implementation
        // 

#if !FEATURE_CORECLR
        [ TargetedPatchingOptOut( "Performance critical to inline across NGen image boundaries" ) ]
#endif
        public TypeCode GetTypeCode() {
            return TypeCode.Strand;
        }

        /// <internalonly />
        bool IConvertible.ToBoolean( IFormatProvider provider ) {
            return Convert.ToBoolean( this, provider );
        }

        /// <internalonly />
        char IConvertible.ToChar( IFormatProvider provider ) {
            return Convert.ToChar( this, provider );
        }

        /// <internalonly />
        sbyte IConvertible.ToSByte( IFormatProvider provider ) {
            return Convert.ToSByte( this, provider );
        }

        /// <internalonly />
        byte IConvertible.ToByte( IFormatProvider provider ) {
            return Convert.ToByte( this, provider );
        }

        /// <internalonly />
        short IConvertible.ToInt16( IFormatProvider provider ) {
            return Convert.ToInt16( this, provider );
        }

        /// <internalonly />
        ushort IConvertible.ToUInt16( IFormatProvider provider ) {
            return Convert.ToUInt16( this, provider );
        }

        /// <internalonly />
        int IConvertible.ToInt32( IFormatProvider provider ) {
            return Convert.ToInt32( this, provider );
        }

        /// <internalonly />
        uint IConvertible.ToUInt32( IFormatProvider provider ) {
            return Convert.ToUInt32( this, provider );
        }

        /// <internalonly />
        long IConvertible.ToInt64( IFormatProvider provider ) {
            return Convert.ToInt64( this, provider );
        }

        /// <internalonly />
        ulong IConvertible.ToUInt64( IFormatProvider provider ) {
            return Convert.ToUInt64( this, provider );
        }

        /// <internalonly />
        float IConvertible.ToSingle( IFormatProvider provider ) {
            return Convert.ToSingle( this, provider );
        }

        /// <internalonly />
        double IConvertible.ToDouble( IFormatProvider provider ) {
            return Convert.ToDouble( this, provider );
        }

        /// <internalonly />
        Decimal IConvertible.ToDecimal( IFormatProvider provider ) {
            return Convert.ToDecimal( this, provider );
        }

        /// <internalonly />
        DateTime IConvertible.ToDateTime( IFormatProvider provider ) {
            return Convert.ToDateTime( this, provider );
        }

        /// <internalonly />
        Object IConvertible.ToType( Type type, IFormatProvider provider ) {
            return Convert.DefaultToType( this, type, provider );
        }

        //
        // Silverlight v2 - v3 defaulted to using Ordinal for the following APIs
        //    [System.Strand]
        //        public Boolean EndsWith(Strand value)
        //        public Boolean EndsWith(Strand value, Boolean ignoreCase, CultureInfo culture)
        //        public int IndexOf(Strand value)
        //        public int IndexOf(Strand value, int startIndex)
        //        public int IndexOf(Strand value, int startIndex, int count)
        //        public int LastIndexOf(Strand value)
        //        public int LastIndexOf(Strand value, int startIndex)
        //        public int LastIndexOf(Strand value, int startIndex, int count)
        //        public Boolean StartsWith(Strand value)
        //        public Boolean StartsWith(Strand value, Boolean ignoreCase, CultureInfo culture)
        //        public Strand ToLower()
        //        public Strand ToUpper()
        //    [System.Char]
        //        public static char ToUpper(char c)
        //        public static char ToLower(char c)
        //
        // Starting with Silverlight 4 these APIs default to using CurrentCulture
        // for alignment with Desktop CLR.  Applications can enable the legacy v2-v3
        // System.Strand behavior by using the 'APP_EARLIER_THAN_SL4.0' configuration option.
        //        
        internal static bool LegacyMode { get { return CompatibilitySwitches.IsAppEarlierThanSilverlight4; } }

        // Is this a string that can be compared quickly (that is it has only characters > 0x80 
        // and not a - or '
        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal extern bool IsFastSort();

        // Is this a string that only contains characters < 0x80.
        [ SecurityCritical ] // auto-generated
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal extern bool IsAscii();

        // Set extra byte for odd-sized strings that came from interop as BSTR.
        [ SecurityCritical ]
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal extern void SetTrailByte( byte data );

        // Try to retrieve the extra byte - returns false if not present.
        [ SecurityCritical ]
        [ ResourceExposure( ResourceScope.None ) ]
        [ MethodImpl( MethodImplOptions.InternalCall ) ]
        internal extern bool TryGetTrailByte( out byte data );

#if !FEATURE_CORECLR
        public CharEnumerator GetEnumerator() {
            Contract.Ensures( Contract.Result< CharEnumerator >() != null );

            BCLDebug.Perf( false, "Avoid using Strand's CharEnumerator until C# special cases foreach on Strand - use the indexed property on Strand instead." );
            return new CharEnumerator( this );
        }
#endif // !FEATURE_CORECLR

        IEnumerator< char > IEnumerable< char >.GetEnumerator() {
            Contract.Ensures( Contract.Result< IEnumerator< char > >() != null );

            BCLDebug.Perf( false, "Avoid using Strand's CharEnumerator until C# special cases foreach on Strand - use the indexed property on Strand instead." );
            return new CharEnumerator( this );
        }

        /// <internalonly />
        IEnumerator IEnumerable.GetEnumerator() {
            Contract.Ensures( Contract.Result< IEnumerator >() != null );

            BCLDebug.Perf( false, "Avoid using Strand's CharEnumerator until C# special cases foreach on Strand - use the indexed property on Strand instead." );
            return new CharEnumerator( this );
        }

        // Copies the source Strand (byte buffer) to the destination IntPtr memory allocated with len bytes.
        [ SecurityCritical ] // auto-generated
#if !FEATURE_CORECLR
#endif //!FEATURE_CORECLR
        internal static unsafe void InternalCopy( Strand src, IntPtr dest, int len ) {
            if ( len == 0 ) {
                return;
            }
            fixed ( char* charPtr = &src.m_firstChar ) {
                var srcPtr = ( byte* ) charPtr;
                var dstPtr = ( byte* ) dest;
                Buffer.Memcpy( dstPtr, srcPtr, len );
            }
        }
    }
*/
}
