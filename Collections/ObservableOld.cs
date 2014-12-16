// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Class1.cs" was last cleaned by Rick on 2014/10/13 at 11:56 AM

namespace Librainian.Collections {
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="http://stackoverflow.com/a/3700580/956364"></seealso>
    public class ObservableOld<T> : IObservable<T>, INotifyPropertyChanged {
        private readonly BehaviorSubject<T> _values;

        private PropertyChangedEventHandler _propertyChanged;

        public ObservableOld() : this( default( T ) ) {
        }

        public ObservableOld( T initalValue ) {
            this._values = new BehaviorSubject<T>( initalValue );

            this._values.DistinctUntilChanged().Subscribe( this.FirePropertyChanged );
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
            add {
                this._propertyChanged += value;
            }
            remove {
                this._propertyChanged -= value;
            }
        }

        public T Value {
            get {
                return this._values.FirstAsync().Wait();
            }

            set {
                this._values.OnNext( value );
            }
        }

        public static implicit operator T( ObservableOld<T> input ) => input.Value;

        public IDisposable Subscribe( IObserver<T> observer ) => this._values.Subscribe( observer );

        public override string ToString() => this._values?.ToString() ?? "Observable<" + typeof( T ).Name + "> with null value.";

        private void FirePropertyChanged( T value ) {
            var handler = this._propertyChanged;

            if ( handler != null ) {
                handler( this, new PropertyChangedEventArgs( "Value" ) );
            }
        }
    }
}