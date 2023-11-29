using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MtcMvcCore.Core.Models.Base;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Media
{
	[XmlRoot]
	public class CoreMediaBase : VersionData
	{
		[XmlElement]
		[EditorConfig(ReadOnly = true, Section = "Info", Sort = 1)]
		public Guid Id { get; set; }

		[XmlElement]
		[EditorConfig(Section = "Info", Sort = 1)]
		public string Name { get; set; }

		[XmlElement]
		public Guid ParentId { get; set; }

		public int Sort { get; set; }

		public bool HasSubItems { get; set; }

		public List<object> InsertOptions { get; set; }

		public virtual string Type { get; set; }

		public virtual string FileName { get; set; }

		public virtual string MimeType { get; set; }

		[XmlElement]
		public int Height { get; set; }

		[XmlElement]
		public int Width { get; set; }

		public string SavePath { get; set; }
	}

}
