using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MtcMvcCore.Core.Constants.Image
{
	public class MimeType
	{
		public const string JPEG = "jpeg";
		public const string PNG = "png";

		public static string GetFileExtension(string mimeType)
		{
			switch (mimeType)
			{
				case "jpeg":
				case "image/jpeg":
					return "jpg";
				case "image/png":
				default:
					return "png";
			}
		}
	}

}
