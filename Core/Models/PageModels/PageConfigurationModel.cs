using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.PageModels
{
	[XmlRoot(ElementName = "component")]
	public partial class Component
	{
		public string ComponentId = Guid.NewGuid().ToString();

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "datasource")]
		public string Datasource { get; set; }

		[XmlAttribute(AttributeName = "view")]
		public string View { get; set; }

		[XmlAttribute(AttributeName = "parameters")]
		public string Parameters { get; set; }

		[XmlAttribute(AttributeName = "cache")]
		public string Cache { get; set; }

		[XmlElement(ElementName = "component")]
		public List<Component> Components { get; set; }
	}

	[XmlRoot(ElementName = "placeholder")]
	public class Placeholder
	{
		[XmlElement(ElementName = "component")]
		public List<Component> Components { get; set; } = new List<Component>();

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

	}

	[XmlRoot(ElementName = "layout")]
	public class Layout
	{
		[XmlElement(ElementName = "contentview")]
		[RadioEditorConfigAttribute(Section = "Layout", Source = "ContentLayoutKeys, MtcMvcCore.Core.SiteConfiguration", GroupLabel = "Content Layout (Columns)")]
		public string Contentview { get; set; }

		[XmlElement(ElementName = "pagelayout")]
		[RadioEditorConfigAttribute(Section = "Layout", Source = "LayoutKeys, MtcMvcCore.Core.SiteConfiguration", GroupLabel = "Page Layout")]
		public string Pagelayout { get; set; }
	}

	[XmlRoot(ElementName = "meta")]
	public class Meta
	{
		[XmlElement(ElementName = "title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "descritption")]
		public string Descritption { get; set; }
	}

	[XmlRoot(ElementName = "information")]
	public partial class Information
	{
		[XmlElement(ElementName = "title")]
		[EditorConfig(Section = "Common", DisplayName = "Page Title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "descritption")]
		public string Descritption { get; set; }
		[XmlElement(ElementName = "keywords")]
		public string Keywords { get; set; }
		[XmlElement(ElementName = "author")]
		public string Author { get; set; }
		[XmlElement(ElementName = "copyright")]
		public string Copyright { get; set; }
		[XmlElement(ElementName = "robots")]
		public string Robots { get; set; }
	}



	[XmlRoot(ElementName = "page")]
	public partial class PageConfigurationModel
	{
		[XmlArray("placeholders"), XmlArrayItem("placeholder")]
		[EditorConfig]
		public List<Placeholder> Placeholders { get; set; }

		[XmlElement(ElementName = "layout")]
		[EditorConfig]
		public Layout Layout { get; set; }

		[XmlElement(ElementName = "cache")]
		public string Cache { get; set; }

		[XmlElement(ElementName = "meta")]
		public Meta Meta { get; set; }

		[XmlElement(ElementName = "information")]
		[EditorConfig]
		public Information Information { get; set; }

		[XmlElement(ElementName = "seo")]
		public string Seo { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public Guid Id { get; set; }

		public string Name { get; set; }
	}

}

