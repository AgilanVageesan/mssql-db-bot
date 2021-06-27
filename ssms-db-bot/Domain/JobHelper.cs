using System;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Serilog;
using ssms_db_bot.Entity;

namespace ssms_db_bot.Domain
{
    public class JobHelper
    {
        public static void TriggerSQLJob(JobRequest request)
        {

            var server = CreateServer(request);
            var job = server.JobServer.Jobs[request.JobName];

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
                Console.WriteLine("Unable to trigger job because " + job.CurrentRunStatus);
            }
        }

        private static Server CreateServer(JobRequest request)
        {
            Server server = new Server(request.DbServer);
            server.ConnectionContext.LoginSecure = false;
            server.ConnectionContext.Login = request.UserName;
            server.ConnectionContext.Password = request.Password;
            return server;
        }


    }
}
