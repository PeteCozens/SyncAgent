using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Common.Extensions
{
    public static class SeriLogExtensions
    {
        /// <summary>
        /// Enriches the logger with the name of the method and the line number that the log was called from
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static ILogger Here(this ILogger logger,
             [CallerMemberName] string memberName = "",
             //[CallerFilePath] string sourceFilePath = "",
             [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogContext.PushProperty("Method", memberName);
            LogContext.PushProperty("Line", sourceLineNumber);
            return logger;
        }
    }
}
