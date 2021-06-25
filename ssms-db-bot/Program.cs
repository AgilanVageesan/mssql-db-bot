using System;
using System.Threading;
using ssms_db_bot.Entity;

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

            }


        }
        public static void TriggerSQLJob(JobRequest request)
        {
        }
    }
}
