using System;
using System.Collections.Generic;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Authentication
{
	[XmlRoot(ElementName = "roles")]
	public class RoleCollection
	{
		[XmlElement(ElementName = "role")]
		public List<RoleModel> RoleModels { get; set; }
	}

	[XmlRoot(ElementName = "role")]
	public class RoleModel
	{
		[XmlAttribute(AttributeName = "id")]
		public Guid RoleId { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		internal static RoleModel FromMongoUser(MongoDbRole role)
		{
			return new RoleModel{
				RoleId = role.Id,
				Name = role.Name
			};
		}
	}
}

