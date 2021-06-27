using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;
using ssms_db_bot.Entity;
using Microsoft.SqlServer.Management.Smo.Agent;
using Serilog;
using ssms_db_bot.Domain;
using System.ComponentModel.DataAnnotations;

namespace ssms_db_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Please provide the requested details in this format : <dbserver> <username> <password> <jobname> <emailId>");
                Thread.Sleep(150000);
            }
            else
            {
                var request = new JobRequest();
                request.DbServer = args[0];
                request.UserName = args[1];
                request.Password = args[2];
                request.JobName = args[3];

                var toEmailAddress = args.Length == 5 ? args[4] : "dummy@gmail.com";
                Log.Logger = CreateLogger(toEmailAddress, request.JobName);
                if(ValidateRequest(request))
                {
                    try
                    {
                        TriggerSQLJob(request);
                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        Log.Logger.Information(ex.Message);
                    }
                }
            }


        }
        public static void TriggerSQLJob(JobRequest request)
        {
            Server server = new Server(request.DbServer);
            server.ConnectionContext.LoginSecure = false;
            server.ConnectionContext.Login = request.UserName;
            server.ConnectionContext.Password = request.Password;

            var job = server.JobServer.Jobs[request.JobName];

            IList<string> messageList = new List<string>();
            Console.WriteLine("Starting " + request.JobName + " in server " + request.DbServer + " requested by " + request.UserName);
            Console.WriteLine("Current Job Status : " + job.CurrentRunStatus);

            DateTime startTime = DateTime.Now;

            if (job.CurrentRunStatus == JobExecutionStatus.Idle)
            {
                job.Start();
                Console.WriteLine("StartTime: " + startTime);
                Thread.Sleep(20000);
                job.Refresh();
                Console.WriteLine(request.JobName + " - " + job.CurrentRunStatus);
                Console.WriteLine("executing...");


                do
                {
                    Thread.Sleep(10000);
                    job.Refresh();
                    if (job.CurrentRunStatus == JobExecutionStatus.Idle)
                    {
                        break;
                    }
                } while (job.CurrentRunStatus == JobExecutionStatus.Executing);

                Console.WriteLine("Job result : " + job.LastRunOutcome);
                Console.WriteLine("Start Time " + startTime);
                Console.WriteLine("EndTime :" + DateTime.Now);

                var duration = Math.Round((DateTime.Now - startTime).TotalMinutes);
                Console.WriteLine("Duration : " + duration + " mins");

                Log.Logger.Information("The requested job " + request.JobName + " has been completed !!! " + Environment.NewLine + Environment.NewLine +
                    "StartTime : " + startTime + Environment.NewLine + "EndTime : " + DateTime.Now + Environment.NewLine + "Duration : " + duration + " mins.");
            }
            else
            {
                Console.WriteLine("Unable to trigger job because "+job.CurrentRunStatus);
            }
        }

        public static bool ValidateRequest(JobRequest request)
        {
            ICollection<ValidationResult> listValidationResult;

            bool isValid = GenericValidator.TryValidate(request, out listValidationResult);
            if (!isValid)
            {
                foreach(ValidationResult res in listValidationResult)
                {
                    Console.WriteLine(res.ErrorMessage);
                }
                return false;

            }
            return true;

        }

        public static ILogger CreateLogger(string toEmailAddress,string jobName)
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
