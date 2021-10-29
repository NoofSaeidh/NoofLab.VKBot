using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Extensions.Logger
{
    public static class LoggerExtensions
    {
        public static void LogUnhandledException(this ILogger logger, Exception e,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogError(e, "Exceptions is not handled. {MemberName}, {SourceFilePath}, {SourceLineNumber}",
                memberName,
                sourceFilePath,
                sourceLineNumber);
        }
    }
}
