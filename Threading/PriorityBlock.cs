namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Annotations;
    using Collections;

    public class PriorityBlock {

        [NotNull]
        public BufferBlock<OneJob> Input { get; private set; }

        [NotNull]
        public ActionBlock<Action> Output { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public PriorityBlock( CancellationToken cancellationToken ) {
            this.CancellationToken = cancellationToken;
            this.Input = new BufferBlock<OneJob>();
            this.Output = new ActionBlock<Action>( action => {
                if ( action != null ) {
                    action();
                }
            }, Blocks.SingleProducer.ConsumeSensible );
            this.TheDoctorsTask = Task.Run( () => this.Triage() );
        }

        public void Add( [NotNull] OneJob oneJob ) {
            if ( oneJob == null ) {
                throw new ArgumentNullException( "oneJob" );
            }
            this._jobs.Enqueue( oneJob );
            this.Input.TryPost( oneJob );
        }

        public void AddJobs( [NotNull]IEnumerable<OneJob> jobs ) {
            if ( jobs == null ) {
                throw new ArgumentNullException( "jobs" );
            }
            var enumerable = jobs as IList<OneJob> ?? jobs.ToList();
            foreach ( var job in enumerable ) {
                this._jobs.Enqueue( job );
            }
            foreach ( var job in enumerable.OrderByDescending( job => job.Priority ) ) {
                this.Input.TryPost( job );
            }
        }

        private readonly ConcurrentQueue<OneJob> _jobs = new ConcurrentQueue<OneJob>();

        private async Task Triage() {
            Log.Enter( );
            while ( !this.CancellationToken.IsCancellationRequested ) {
                await Task.WhenAny( this.Input.OutputAvailableAsync( this.CancellationToken ) );

                OneJob item;
                if ( !this.Input.TryReceive( null, out item ) ) {
                    continue;   //Hello? Hello? Hmm. No one is there. Go back to waiting.
                }

                var highest = this._jobs.OrderByDescending( job => job.Priority ).FirstOrDefault();
                if ( null == highest ) {
                    continue;
                }
                this._jobs.Remove( highest );

                if ( highest != item ) {
                    this.Add( item );   //add back into the pile
                }

                await this.Output.SendAsync( highest.Action, this.CancellationToken );
            }
            Log.Exit();
        }

        public Task TheDoctorsTask { get; private set; }
    }
}