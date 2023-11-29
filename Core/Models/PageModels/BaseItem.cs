using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MtcMvcCore.Core.Models.Base;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.PageModels
{
	public class BaseItem : VersionData
	{

		public Guid Id { get; set; }

		[EditorConfig(ReadOnly = true, Section = "Info", Sort = 100)]
		public Guid GroupId { get; set; }

		[EditorConfig(Section = "Common", DisplayName = "Display Name", Sort = 200)]
		public virtual string Name { get; set; }

		public Guid ParentId { get; set; }

		public int Sort { get; set; }

		[XmlIgnore]
		[BsonIgnore]
		public BaseItem Parent { get; set; }

		[XmlIgnore]
		[BsonIgnore]
		public List<BaseItem> SubItems { get; set; } = new List<BaseItem>();

		[XmlIgnore]
		[BsonIgnore]
		public bool HasSubItems { get; set; }

		[XmlIgnore]
		[BsonIgnore]
		public string Type { get; set; }

		[XmlIgnore]
		[BsonIgnore]
		public List<object> InsertOptions { get; set; }

	}

}
