using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Authentication
{
	[XmlRoot(ElementName = "users")]
	public class UserCollection
	{
		[XmlElement(ElementName = "user")]
		public List<UserModel> UserModels { get; set; }
	}

	[XmlRoot(ElementName = "user")]
	public class UserModel
	{
		public Guid UserId { get; set; }

		[XmlAttribute(AttributeName = "username")]
		public string UserName { get; set; }

		[XmlAttribute(AttributeName = "firstname")]
		public string FirstName { get; set; }

		[XmlAttribute(AttributeName = "lastname")]
		public string LastName { get; set; }

		[XmlAttribute(AttributeName = "email")]
		public string Email { get; set; }

		[XmlAttribute(AttributeName = "password")]
		public string Password { get; set; }

		[XmlIgnore]
		public List<string> Roles { get; set; }

		[XmlAttribute(AttributeName = "roles")]
		public string RolesText
		{
			get => string.Join(", ", Roles);
			set
			{
				var split = value.Split(',').Select(i => i.Trim()).ToList();
				Roles = split.ToList();
			}
		}

		internal static UserModel FromMongoUser(MongoDbUserModel user)
		{
			return new UserModel
			{
				UserId = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email
			};
		}
	}
}

