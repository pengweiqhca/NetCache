namespace NetCache
{
    public interface ILogger
    {
        void Error(string message);
        void Message(string message);
        void Message(string action, string value);
    }
}
