namespace Librainian.Financial {
    public class CheckingAccount : BankAccount {
        public override bool TryDeposit() {
            throw new System.NotImplementedException();
        }

        public override bool Deposit() {
            throw new System.NotImplementedException();
        }
    }
}