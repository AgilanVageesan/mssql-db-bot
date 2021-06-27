using System;
using Serilog;

namespace ssms_db_bot.Domain
{
    public static class Logger
    {
        public static ILogger CreateLogger(string toEmailAddress, string jobName)
        {
            var log = new LoggerConfiguration().WriteTo.Email(
                fromEmail: "fromAddress@gmail.com",
                toEmail: toEmailAddress,
                mailServer: "mailserver",
                mailSubject: "[AppName] " + jobName + " job succeeded at " + DateTime.Now.ToString("dd MMMM yyyy"),
                outputTemplate: "{Message}",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).CreateLogger();

            return log;
        }
    }
}
