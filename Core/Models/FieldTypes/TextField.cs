using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot]
	public class TextField
	{
		[XmlElement]
		public string Value { get; set; }
		
		[XmlElement]
		public string SimpleText { get; set; }
		
		[XmlElement]
		public string DgsVideo { get; set; }
	}
}
