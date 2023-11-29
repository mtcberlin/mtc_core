using System;

namespace MtcMvcCore.Core.Models.Exceptions
{
	public class InvalidPageTypeException : Exception
	{
		public InvalidPageTypeException()
		{
		}

		public InvalidPageTypeException(string message)
			: base(message)
		{
		}

		public InvalidPageTypeException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}