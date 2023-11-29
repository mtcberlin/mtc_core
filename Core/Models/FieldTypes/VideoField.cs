using System;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	[XmlRoot]
	public class VideoField
	{
		[XmlElement]
		public Guid AssetId { get; set; }
		
	}
}
