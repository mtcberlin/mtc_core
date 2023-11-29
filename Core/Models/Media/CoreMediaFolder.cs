using System;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Media
{
	[XmlRoot]
	public class CoreMediaFolder : CoreMediaBase
	{
		public CoreMediaFolder() {
			Type = "folder";
		}
	}

}
