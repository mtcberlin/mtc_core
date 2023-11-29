using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	public class CachingOptions
	{
		[XmlAttribute(AttributeName = "Cacheable")]
		public bool Cacheable { get; set; }

		[XmlAttribute(AttributeName = "VaryByDatasource")]
		public bool VaryByDatasource { get; set; }

		[XmlAttribute(AttributeName = "VaryBySeoUrl")]
		public bool VaryBySeoUrl { get; set; }

		[XmlAttribute(AttributeName = "VaryByView")]
		public bool VaryByView { get; set; }

		[XmlAttribute(AttributeName = "VaryByDevice")]
		public bool VaryByDevice { get; set; }

		[XmlAttribute(AttributeName = "VaryByLogin")]
		public bool VaryByLogin { get; set; }

		[XmlAttribute(AttributeName = "VaryByParam")]
		public bool VaryByParam { get; set; }

		[XmlAttribute(AttributeName = "VaryByQueryString")]
		public bool VaryByQueryString { get; set; }

		[XmlAttribute(AttributeName = "VaryBySubComponents")]
		public bool VaryBySubComponents { get; set; } = true;

		[XmlAttribute(AttributeName = "MaxCacheTime")]
		public int MaxCacheTime { get; set; }
	}
}
