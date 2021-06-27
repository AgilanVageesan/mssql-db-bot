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
                return;
            }

            var request = new JobRequest();
            request.DbServer = args[0];
            request.UserName = args[1];
            request.Password = args[2];
            request.JobName = args[3];

            var toEmailAddress = args.Length == 5 ? args[4] : "dummy@gmail.com";
            Log.Logger = Logger.CreateLogger(toEmailAddress, request.JobName);
            if (ValidateRequest(request))
            {
                try
                {
                    JobHelper.TriggerSQLJob(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Log.Logger.Information(ex.Message);
                }
            }

        }

        private static bool ValidateRequest(JobRequest request)
        {
            ICollection<ValidationResult> listValidationResult;

            bool isValid = GenericValidator.TryValidate(request, out listValidationResult);
            if (!isValid)
            {
                foreach (ValidationResult res in listValidationResult)
                {
                    Console.WriteLine(res.ErrorMessage);
                }
                return false;

            }
            return true;

        }


    }
}
