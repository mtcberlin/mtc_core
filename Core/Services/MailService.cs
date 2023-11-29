using System;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Controllers
{
	public class MailService : IMailService
	{
		private readonly ILogger<MailService> _logger;

		public MailService(ILogger<MailService> logger)
		{
			_logger = logger;
		}

		public void Send(string to, string subject, string body)
		{
			SendMail(to, subject, body, false);
		}

		public void Send(string to, string subject, HtmlString body)
		{
			SendMail(to, subject, body.Value, true);
		}

		private void SendMail(string to, string subject, string body, bool isHtml)
		{
			MailMessage message = new MailMessage(Settings.EmailUser, to);
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = isHtml;
			SmtpClient client = new SmtpClient(Settings.EmailServer);
			client.Port = Settings.EmailPort;
			client.UseDefaultCredentials = false;
			client.EnableSsl = Settings.EmailSsl;
			client.Credentials = new NetworkCredential(Settings.EmailUser,
					Settings.EmailPasswort);
			try
			{
				client.Send(message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
					ex.ToString());
			}
		}
	}
}
