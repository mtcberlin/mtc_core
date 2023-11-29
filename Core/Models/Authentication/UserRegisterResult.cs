using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Authentication
{
	public class UserRegisterResult
	{
		public Guid UserId { get; set; }

		public List<string> ErrorCodes { get; set; }

	}
}

