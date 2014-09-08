namespace Librainian.Gaming {

    public class DiceCup : GameContainer, IDiceCup {

        public void PourDice( IPlayerTable table ) {
            this.MoveAll<Dice>( table );
        }

    }
}