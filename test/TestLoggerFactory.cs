namespace test
{
    using Microsoft.Extensions.Logging;

    public static class TestLoggerFactory
    {
        private static readonly ILoggerFactory factory;

        static TestLoggerFactory()
        {
            factory = LoggerFactory.Create(configure =>
            {
                configure.AddConsole();
            });
        }

        public static ILogger<T> CreateLogger<T>() where T:ILogger
        {
            return factory.CreateLogger<T>();
        }
    }
}