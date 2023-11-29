using System.Collections.Generic;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot(ElementName = "pagetype")]
	public class PageTypeConfigModel
	{
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "model")]
		public string Model { get; set; }

		[XmlElement(ElementName = "displayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "view")]
		public string DefaultView { get; set; }

		[XmlElement(ElementName = "layout")]
		public string DefaultLayout { get; set; }

		[XmlArray("insertOptions"), XmlArrayItem("insertOption")]
		public List<string> InsertOptions { get; set; }
		
		[XmlArray("components"), XmlArrayItem("component")]
		public List<BrunchComponent> DefaultComponents { get; set; }
		
		[XmlArray("structure"), XmlArrayItem("page")]
		public List<BrunchPage> SubStructure { get; set; }

		public class BrunchComponent {

			[XmlAttribute(AttributeName = "placeholder")]
			public string Placeholder { get; set; }

			[XmlText]
			public string Name { get; set; }
		}

		public class BrunchPage {

			[XmlAttribute(AttributeName = "type")]
			public string Type { get; set; }
			
			[XmlAttribute(AttributeName = "view")]
			public string View { get; set; }
			
			[XmlAttribute(AttributeName = "layout")]
			public string Layout { get; set; }

			[XmlArray("components"), XmlArrayItem("component")]
			public List<BrunchComponent> DefaultComponents { get; set; }

			[XmlArray("structure"), XmlArrayItem("page")]
			public List<BrunchPage> SubStructure { get; set; }

		}
	}
}
