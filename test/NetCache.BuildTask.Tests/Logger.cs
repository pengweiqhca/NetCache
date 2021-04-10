using Xunit.Abstractions;

namespace NetCache.BuildTask.Tests
{
    public class Logger : ILogger
    {
        private readonly ITestOutputHelper _output;

        public Logger(ITestOutputHelper output) => _output = output;

        public void Message(string action, string value) => Message($"{action}: {value}");

        public void Message(string message) => _output.WriteLine(message);

        public void Error(string message) => _output.WriteLine($"Error -> {message}");
    }
}
