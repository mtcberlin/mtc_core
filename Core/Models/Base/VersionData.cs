using System;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Base
{
	public class VersionData : StatisticsData
	{

		[EditorConfig(Section = "Info", ReadOnly = true, Sort = 100)]
		public string Language { get; set; }

		public int ContentVersion { get; set; }

	}

}
