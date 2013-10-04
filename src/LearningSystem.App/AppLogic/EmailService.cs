using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LearningSystem.App.ViewModels;
using Typesafe.Mailgun;
using System.Net.Mail;
namespace LearningSystem.App.AppLogic
{
    public static class EmailService
    {
        public static CommandResult SendConfirmationEmail(RegisterViewModel model, string userId)
        {
            // https://api.mailgun.net/v2
            // http://documentation.mailgun.com/quickstart.html#sending-messages

            var client = new MailgunClient("app4756.mailgun.org", "key-1g8ild93unp4dcswe0pgt4ce62ar24z0");

            var message = new MailMessage("admin@learningsystem.apphb.com", model.Email);
            message.Sender = new MailAddress("admin@learningsystem.apphb.com");
            message.From = message.Sender;
            message.Subject = "Learningsystem.apphb.com Account Activation - " + model.UserName;
            message.IsBodyHtml = true;

            string returnLink = "localhost:51903/Account/Confirm?userId=" + userId;

            message.Body = string.Format(
@"<p>Hi {0},</p>
<p>You are receiving this email because you have registered in Learningsystem.apphb.com. If you have not registered this account, you can safely ignore this email. Please do not reply to this email because we are not monitoring this inbox. </p>

<p>To start using LearningSystem, you need to verify your account. Please click the link or paste it into your browser's address bar:</p>
<p><a href='{1}'>{1}</a></p>



<p>Sincerely,</p>
<p><a href='http://learningsystem.apphb.com/'>The LearningSystem Development Team</a></p>", model.UserName, returnLink);

            var result = client.SendMail(message);
            return result;
        }
    }
}