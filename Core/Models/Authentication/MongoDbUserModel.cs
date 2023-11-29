using System;
using AspNetCore.Identity.Mongo.Model;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Authentication
{
	public class MongoDbUserModel : MongoUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public UserProfile UserProfile { get; set; } = new UserProfile();
	}

	public class MongoDbRole : MongoRole<Guid>
	{
	}
}

