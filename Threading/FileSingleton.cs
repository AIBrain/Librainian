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
// "Librainian/FileSingleton.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.IO;
    using System.Threading;
    using Measurement.Time;
    using NUnit.Framework;

    /// <summary>
    ///     Uses a named semaphore to allow only ONE of name.
    ///     No proof if this class actually helps at all.
    ///     Variation of a singleton... I think.
    /// </summary>
    /// <example>
    ///     using ( new Snag( anyName ) ) { DoCode(); }
    /// </example>
    public class FileSingleton : IDisposable {
        /// <summary>
        ///     Uses a named semaphore to allow only ONE of name.
        /// </summary>
        /// <example>
        ///     using ( var snag = new Snag( guid ) ) { DoCode(); }
        /// </example>
        public FileSingleton( Guid name ) {
            Assert.AreNotEqual( name, Guid.Empty );
            this.Snag( byGuid: name );
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of name.
        /// </summary>
        /// <example>
        ///     using ( var snag = new Snag( info ) ) { DoCode(); }
        /// </example>
        public FileSingleton( FileSystemInfo name ) {
            Assert.NotNull( name );
            this.Snag( byInfo: name );
        }

        /// <summary>
        ///     Uses a named semaphore to allow only ONE of name.
        /// </summary>
        /// <example>
        ///     using ( var snag = new Snag( name ) ) { DoCode(); }
        /// </example>
        public FileSingleton( String name ) {
            Assert.IsNotNullOrEmpty( name );
            this.Snag( byName: name );
        }

        public String Name { get; private set; }

        public Semaphore Semaphore { get; private set; }

        public Boolean Snagged { get; private set; }

        /// <summary>
        ///     Immediately releases all resources owned by the object.
        /// </summary>
        public void Dispose() {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        ///     Finalizer that could be called by the GC.
        /// </summary>
        ~FileSingleton() {
            this.Dispose( false );
        }

        /// <summary>
        ///     Immediately releases all resources owned by the object.
        /// </summary>
        /// <param name="calledByUser">
        ///     If true, the object is being disposed explicitely and can still access the other managed
        ///     objects it is referencing.
        /// </param>
        private void Dispose( Boolean calledByUser ) {
            if ( calledByUser ) {
                /*Release any unmanaged here.*/
            }

            try {
                if ( this.Snagged ) {
                    var semaphore = this.Semaphore;
                    if ( null != semaphore ) {
                        semaphore.Release();
                        semaphore.Dispose();
                    }
                }
            }
            finally {
                this.Snagged = false;
                //String.Format( "UnSnagged from `{0}`.", this.Name ).TimeDebug();
            }

            this.Semaphore = null;
        }

        private void Snag( Guid? byGuid = null, FileSystemInfo byInfo = null, String byName = null, TimeSpan? timeout = null ) {
            try {
                this.Snagged = false;
                this.Semaphore = null;

                if ( byGuid.HasValue ) {
                    this.Name = byGuid.Value.ToString();
                }
                else if ( null != byInfo ) {
                    this.Name = byInfo.Name;
                }
                else if ( !String.IsNullOrEmpty( byName ) ) {
                    this.Name = byName;
                }

                if ( String.IsNullOrWhiteSpace( this.Name ) ) {
                    return;
                }

                //String.Format( "Wanting to snag `{0}`.", this.Name ).TimeDebug();
                this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: this.Name );
                this.Snagged = this.Semaphore.WaitOne( timeout.GetValueOrDefault( Seconds.Ten ) );
                if ( this.Snagged ) {
                    //String.Format( "Snagged on `{0}`.", this.Name ).TimeDebug();
                }
            }
            catch ( ObjectDisposedException exception ) {
                exception.Debug();
            }
            catch ( AbandonedMutexException exception ) {
                exception.Debug();
            }
            catch ( InvalidOperationException exception ) {
                exception.Debug();
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.Debug();
            }
            catch ( ArgumentException exception ) {
                exception.Debug();
            }
            catch ( IOException exception ) {
                exception.Debug();
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Debug();
            }
            catch ( WaitHandleCannotBeOpenedException exception ) {
                exception.Debug();
            }
            catch ( Exception exception ) {
                exception.Debug();
            }
        }
    }

    /*
    /// <summary>
    /// One and only one instance of this class should ever get instantiated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton2<T> where T : new() {
        static Mutex mutex = new Mutex();
        static T instance;
        public static T Instance {
            get {
                mutex.WaitOne();
                if ( instance == null ) { instance = new T(); }
                mutex.ReleaseMutex();
                return instance;
            }
        }
    }
    */

    /*
    /// <summary>
    /// One and only one instance of this class should ever get instantiated.
    /// </summary>
    public class Singleton<T> {
        Singleton() {
            AIBrain.Brain.BlackBoxClass.Diagnostic( String.Format( "Creating Singleton of type {0}.", typeof( T ).FullName ) );
        }
        ~Singleton() { }

        public static T Instance {
            get { return Nested.instance; }
        }

        class Nested {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested() {
                Utility.DebugAssert( null != instance, "Singleton fukd up" );
                instance = new T;
            }
            internal static readonly T instance;
        }
    }
    */

    /*
    /// <summary>
    /// Courtesy of http://www.yoda.arachsys.com/csharp/singleton.html
    /// </summary>
    public sealed class BaseSingleton {
        //Singleton() {
        //    Debug.WriteLine( "Singleton ctor()" );
        //    Debugger.Break();
        //}

        public static BaseSingleton Instance {
            get {
                Debug.WriteLine( "Singleton get" );
                return Nested.instance;
            }
        }

        class Nested {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested() {
                Debug.WriteLine( "Singleton.Nested ctor()" );
                Debugger.Break();
            }

            internal static readonly BaseSingleton instance = new BaseSingleton();
        }
    }
    */
}
