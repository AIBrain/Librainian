namespace Librainian.Gaming {
    public class PlayerTable : GameContainer, IPlayerTable {


        public void PickupAllDice( IGameContainer container ) {
            this.MoveAll<IDice>( container );
        }

    }
}