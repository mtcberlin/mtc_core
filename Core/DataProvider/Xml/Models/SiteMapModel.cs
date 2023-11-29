using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.Xml.Models
{
	[XmlRoot(ElementName = "page")]
	public class Page
	{
		[XmlAttribute(AttributeName = "id")]
		public Guid Id { get; set; }

		[XmlAttribute(AttributeName = "pageconfig")]
		public string PageConfiguration { get; set; }
		
		[XmlAttribute(AttributeName = "area")]
		public string Area { get; set; }
		
		[XmlAttribute(AttributeName = "controller")]
		public string Controller { get; set; }
		
		[XmlAttribute(AttributeName = "action")]
		public string Action { get; set; }
		
		[XmlAttribute(AttributeName = "redirect")]
		public string Redirect { get; set; }
		
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		
		[XmlElement(ElementName = "page")]
		public List<Page> Pages { get; set; }

		[XmlElement(ElementName = "lang")]
		public List<Page> Languages { get; set; }

		[XmlAttribute(AttributeName = "seoname")]
		public string SeoName { get; set; }

		[XmlAttribute(AttributeName = "isocode")]
		public string IsoCode { get; set; }

		[XmlAttribute(AttributeName = "doNotShowInNavigation")]
		public string DoNotShowInNavigationRaw { get; set; }

		public bool DoNotShowInNavigation
		{
			get
			{
				return DoNotShowInNavigationRaw == "true";
			}
		}

		public string SeoPath { get; set; }
		public string SeoPathWithLangParam { get; set; }
		public string UrlParameterName { get; set; }
		public string Language { get; set; }

		public Page Parent { get; set; }
		public Guid ParentId { get; set; }
	}

	[XmlRoot(ElementName = "site")]
	public class SiteMapModel
	{
		[XmlElement(ElementName = "page")]
		public List<Page> Pages { get; set; }
	}

}
