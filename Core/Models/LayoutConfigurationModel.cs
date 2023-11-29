using System.Collections.Generic;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot(ElementName = "layout")]
	public class LayoutConfigurationModel
	{
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		[XmlElement(ElementName = "key")]
		public string Key { get; set; }

		[XmlElement(ElementName = "thumbnail")]
		public string Thumbnail { get; set; }

		[XmlArray("placeholders"), XmlArrayItem("placeholder")]
		public List<PlaceholderInformation> Placeholders { get; set; }

		[XmlArray("grid"), XmlArrayItem("column")]
		public GridInformation[] Grid { get; set; }
	}

	[XmlRoot(ElementName = "placeholder")]
	public class PlaceholderInformation
	{
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }

		// [XmlElement(ElementName = "columns")]
		// public int Columns { get; set; } = 12;

		[XmlArray("components"), XmlArrayItem("component")]
		public List<string> AllowedComponents { get; set; }

		[XmlArray("columns"), XmlArrayItem("column")]
		public ColumnInformation[] Columns { get; set; }
	}

	[XmlRoot(ElementName = "column")]
	public class GridInformation
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "maxWidth")]
		public int MaxWidth { get; set; }
	}

	[XmlRoot(ElementName = "column")]
	public class ColumnInformation
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlText]
		public int Columns { get; set; }
	}
}
