using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models
{
	public class ContentChangeApiModel
	{
		[JsonProperty("pageUrl")]
		public string PageUrl { get; set; }

		[JsonProperty("changes")]
		public List<ContentChangeSet> Changes { get; set; }
	}

	public class ContentChangeSet
	{
		[JsonProperty("filePath")]
		[JsonRequired]
		public string FilePath { get; set; }

		[JsonProperty("fieldName")]
		[JsonRequired]
		public string FieldName { get; set; }

		[JsonProperty("oldValue")]
		[JsonRequired]
		public string OldValue { get; set; }

		[JsonProperty("value")]
		[JsonRequired]
		public string Value { get; set; }
	}
}
