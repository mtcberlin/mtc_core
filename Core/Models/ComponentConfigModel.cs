using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot(ElementName = "component")]
	public class ComponentConfigModel
	{
		[XmlElement(ElementName = "caching")]
		public CachingOptions Caching { get; set; }

		[XmlElement(ElementName = "editorModel")]
		public string EditorModel { get; set; }

		[XmlElement(ElementName = "displayName")]
		public string DisplayName { get; set; }

		[XmlElement(ElementName = "previewImg")]
		public string PreviewImg { get; set; }

		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
	}
}
