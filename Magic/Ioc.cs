namespace Librainian.Magic {
    using Annotations;
    using IoCDI;

    public static class Ioc {
        [NotNull]
        public static IIocContainer Container { get; set; }
    }
}