using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using MtcMvcCore.Core.Models;

namespace MtcMvcCore.Core.Services
{
	public interface IMailService
	{
		void Send(string to, string subject, HtmlString body);

		void Send(string to, string subject, string body);

	}
}
