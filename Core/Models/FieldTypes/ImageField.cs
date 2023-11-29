using System;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot]
	public class ImageField
	{
		[XmlElement]
		public Guid AssetId { get; set; }
		
		[XmlElement]
		public string Caption { get; set; }

		[XmlElement]
		public string CaptionSimple { get; set; }
		
		[XmlElement]
		public string ImageDescription { get; set; }

		[XmlElement]
		public string ImageDescriptionSimple { get; set; }

		[XmlElement]
		public string DgsVideo { get; set; }

	}
}
