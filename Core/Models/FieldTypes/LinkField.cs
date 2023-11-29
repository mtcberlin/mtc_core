using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot]
	public class LinkField
	{
		[XmlElement]
		public string Text { get; set; }

		[XmlElement]
		public string TextSimple { get; set; }

		[XmlElement]
		public string Link { get; set; }

		[XmlElement]
		public string Target { get; set; }

		[XmlElement]
		public string Ancher { get; set; }

		[XmlElement]
		public string Icon { get; set; }

		[XmlElement]
		public string Type { get; set; }
		
		[XmlElement]
		public string Classes { get; set; }
	}
}
