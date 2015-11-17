// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/PriorityBlock.cs" was last cleaned by Rick on 2015/11/02 at 12:32 AM

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

    public class PriorityBlock {

        private readonly ConcurrentQueue< OneJob > _jobs = new ConcurrentQueue< OneJob >();

        public PriorityBlock( CancellationToken cancellationToken ) {
            this.CancellationToken = cancellationToken;
            this.Input = new BufferBlock< OneJob >();
            this.Output = new ActionBlock< Action >( action => action?.Invoke(), Blocks.SingleProducer.ConsumeSensible );
            this.TheDoctorsTask = Task.Run( this.Triage );
        }

        public CancellationToken CancellationToken { get; }

        [NotNull]
        public BufferBlock< OneJob > Input { get; }

        [NotNull]
        public ActionBlock< Action > Output { get; }

        public Task TheDoctorsTask { get; private set; }

        public void Add( [NotNull] OneJob oneJob ) {
            if ( oneJob == null ) {
                throw new ArgumentNullException( nameof( oneJob ) );
            }
            this._jobs.Enqueue( oneJob );
            this.Input.TryPost( oneJob );
        }

        public void AddJobs( [NotNull] IEnumerable< OneJob > jobs ) {
            if ( jobs == null ) {
                throw new ArgumentNullException( nameof( jobs ) );
            }
            var enumerable = jobs as IList< OneJob > ?? jobs.ToList();
            foreach ( var job in enumerable ) {
                this._jobs.Enqueue( job );
            }
            foreach ( var job in enumerable.OrderByDescending( job => job.Priority ) ) {
                this.Input.TryPost( job );
            }
        }

        private async Task Triage() {
            Log.Enter();
            while ( !this.CancellationToken.IsCancellationRequested ) {
                await Task.WhenAny( this.Input.OutputAvailableAsync( this.CancellationToken ) );

                OneJob item;
                if ( !this.Input.TryReceive( null, out item ) ) {
                    continue; //Hello? Hello? Hmm. No one is there. Go back to waiting.
                }

                var highest = this._jobs.OrderByDescending( job => job.Priority )
                                  .FirstOrDefault();
                if ( null == highest ) {
                    continue;
                }
                this._jobs.Remove( highest );

                if ( highest != item ) {
                    this.Add( item ); //add back into the pile
                }

                await this.Output.SendAsync( highest.Action, this.CancellationToken );
            }
            Log.Exit();
        }

    }

}
