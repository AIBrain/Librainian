namespace Librainian.Magic {
    using Annotations;

    public static class Ioc {
        [NotNull]
        public static IIocContainer Container { get; set; }
    }
}