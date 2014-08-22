namespace Librainian.Measurement.Time {
    using System.Numerics;
    using Annotations;

    public interface IQuantityOfTime {
        int GetHashCode();

        [Pure]
        BigInteger ToPlanckTimes();

        string ToString();
    }
}