// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PriorityBlock.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/PriorityBlock.cs" was last formatted by Protiguous on 2018/05/24 at 7:34 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Collections;
    using JetBrains.Annotations;

    [Obsolete]
    public class PriorityBlock {

        private readonly ConcurrentQueue<OneJob> _jobs = new ConcurrentQueue<OneJob>();

        public CancellationToken CancellationToken { get; }

        [NotNull]
        public BufferBlock<OneJob> Input { get; }

        [NotNull]
        public ActionBlock<Action> Output { get; }

        public Task TheDoctorsTask { get; }

        public PriorityBlock( CancellationToken cancellationToken ) {
            this.CancellationToken = cancellationToken;
            this.Input = new BufferBlock<OneJob>();
            this.Output = new ActionBlock<Action>( action => action?.Invoke(), Blocks.SingleProducer.ConsumeSensible );
            this.TheDoctorsTask = Task.Run( this.Triage );
        }

        private async Task Triage() {
            Logging.Enter();

            while ( !this.CancellationToken.IsCancellationRequested ) {
                await Task.WhenAny( this.Input.OutputAvailableAsync( this.CancellationToken ) );

                if ( !this.Input.TryReceive( null, out var item ) ) {
                    continue; //Hello? Hello? Hmm. No one is there. Go back to waiting.
                }

                var highest = this._jobs.OrderByDescending( job => job.Priority ).FirstOrDefault();

                if ( null == highest ) { continue; }

                this._jobs.Remove( highest );

                if ( highest != item ) {
                    this.Add( item ); //add back into the pile
                }

                await this.Output.SendAsync( highest.Action, this.CancellationToken );
            }

            Logging.Exit();
        }

        public void Add( [NotNull] OneJob oneJob ) {
            if ( oneJob is null ) { throw new ArgumentNullException( nameof( oneJob ) ); }

            this._jobs.Enqueue( oneJob );
            this.Input.TryPost( oneJob );
        }

        public void AddJobs( [NotNull] IEnumerable<OneJob> jobs ) {
            if ( jobs is null ) { throw new ArgumentNullException( nameof( jobs ) ); }

            var enumerable = jobs as IList<OneJob> ?? jobs.ToList();

            foreach ( var job in enumerable ) { this._jobs.Enqueue( job ); }

            foreach ( var job in enumerable.OrderByDescending( job => job.Priority ) ) { this.Input.TryPost( job ); }
        }
    }
}