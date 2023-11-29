using System;
using System.Text.Json;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	public class SaveItemDataModel
	{
		[JsonProperty("pageId")]
		public Guid pageId { get; set; }

		[JsonProperty("lang")]
		public string lang { get; set; }

		[JsonProperty("fields")]
		public FieldInforamtions[] fields { get; set; }

		public class FieldInforamtions
		{
			public string fieldValue { get; set; }
			public string fieldType { get; set; }
			public string fieldPath { get; set; }
		}
	}
}
