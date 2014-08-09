namespace Librainian.IoCDI {
    using Annotations;

    public static class Ioc {
        [NotNull]
        public static IIocContainer Container { get; set; }
    }
}