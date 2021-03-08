namespace Librainian.ComputerSystems {

    using System.Diagnostics;
    using NUnit.Framework;

    [TestFixture]
    public class ComputerTests {

        [Test]
        public void TestScores() {
            Debug.WriteLine( Computer.SATScores.CPU() );
            Debug.WriteLine( Computer.SATScores.D3D() );
            Debug.WriteLine( Computer.SATScores.Disk() );
            Debug.WriteLine( Computer.SATScores.Graphics() );
            Debug.WriteLine( Computer.SATScores.Memory() );
            Debug.WriteLine( Computer.SATScores.TimeTaken() );
            Debug.WriteLine( Computer.SATScores.WinSAT_AssessmentState() );
            Debug.WriteLine( Computer.SATScores.WinSPRLevel() );
        }

    }

}