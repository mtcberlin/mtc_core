using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot(ElementName = "adminPage")]
	public class AdminPageConfigurationModel
	{
		[XmlElement(ElementName = "displayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "group")]
		public string Group { get; set; }

		[XmlElement(ElementName = "icon")]
		public string Icon { get; set; }

		[XmlElement(ElementName = "controller")]
		public string Controller { get; set; }

		[XmlElement(ElementName = "action")]
		public string Action { get; set; }

		[XmlElement(ElementName = "area")]
		public string Area { get; set; }

		[XmlElement(ElementName = "roles")]
		public string Roles { get; set; }

		
	}
}
