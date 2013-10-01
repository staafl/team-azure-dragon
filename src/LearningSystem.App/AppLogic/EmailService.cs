﻿using System;
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
            message.Subject = "Confirm your account - " + model.UserName;
            message.IsBodyHtml = true;

            string returnLink = "localhost/Account/Confirm?userId=" + userId;

            message.Body = string.Format(
@"<p>Hi {0},</p>

To start using LearningSystem, you need to verify your account. Please click the link or paste it into your browser's address bar:
{1}
Love,
The LearningSystem crew", model.UserName, returnLink);
            var result = client.SendMail(message);
            return result;
        }
    }
}