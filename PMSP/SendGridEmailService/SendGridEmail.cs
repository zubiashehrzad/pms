using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace PMS.SendGridEmailService
{
    public static class SendGridEmail
    {
        public static void SendEmail(string toEmailAddress, string subject, string body)
        {
            var apiKey = "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";//Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("notifications@icaremanager.com", "notifications@icaremanager.com");
            var to = new EmailAddress(toEmailAddress, toEmailAddress);
            //var plainTextContent = "";
            var htmlContent = body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}
