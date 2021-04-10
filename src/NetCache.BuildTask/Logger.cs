using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NetCache
{
    /// <summary>
    /// 表示日志
    /// </summary>
    internal class Logger : ILogger
    {
        /// <summary>
        /// 日志标签
        /// </summary>
        private const string TagName = "NetCache";

        /// <summary>
        /// 包装的日志类
        /// </summary>
        private readonly TaskLoggingHelper _logger;

        /// <summary>
        /// 表示日志
        /// </summary>
        /// <param name="logger"></param>
        public Logger(TaskLoggingHelper logger) => _logger = logger;

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="action">行为</param>
        /// <param name="value">值</param>
        public void Message(string action, string value) => Message($"{action}: {value}");

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Message(string message) => _logger.LogMessage(MessageImportance.High, $"{TagName} -> {message}");

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="message">异常</param>
        public void Error(string message) => _logger.LogError($"{TagName} -> {message}");
    }
}
