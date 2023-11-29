using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.PageModels
{
	public class BasePage : BaseItem
	{

		[EditorConfig(Section = "Technical", HideInEditor = true,  Authorize = "Administrator", Sort = 300)]
		public virtual string Controller { get; set; }

		[EditorConfig(Section = "Technical", HideInEditor = true, Authorize = "Administrator", Sort = 300)]
		public virtual string Action { get; set; }

		[EditorConfig(Section = "Technical", HideInEditor = true, Authorize = "Administrator", Sort = 300)]
		public virtual string Area { get; set; }

		[XmlArray("placeholders"), XmlArrayItem("placeholder")]
		[EditorConfig]
		public virtual List<Placeholder> Placeholders { get; set; }

		[XmlElement(ElementName = "contentview")]
		[RadioEditorConfigAttribute(Section = "Layout", Source = "ContentLayoutKeys, MtcMvcCore.Core.SiteConfiguration", GroupLabel = "Content Layout Columns", Sort = 400)]
		public virtual string View { get; set; }

		[XmlElement(ElementName = "pagelayout")]
		[RadioEditorConfigAttribute(Section = "Layout", Source = "LayoutKeys, MtcMvcCore.Core.SiteConfiguration", GroupLabel = "Page Layout", Sort = 400)]
		public virtual string Layout { get; set; }

		public virtual string Redirect { get; set; }

		[EditorConfig(Section = "Common", DisplayName = "Public visible", Sort = 200)]
		public virtual bool Published { get; set; }

		[XmlElement(ElementName = "title")]
		[EditorConfig(Section = "Common", DisplayName = "Page Title", Sort = 200)]
		public virtual string Title { get; set; }

		[XmlElement(ElementName = "descritption")]
		public virtual string Descritption { get; set; }

		[XmlElement(ElementName = "keywords")]
		public virtual string Keywords { get; set; }

		[XmlElement(ElementName = "author")]
		public string Author { get; set; }

		[XmlElement(ElementName = "robots")]
		public virtual string Robots { get; set; }

	}

}
