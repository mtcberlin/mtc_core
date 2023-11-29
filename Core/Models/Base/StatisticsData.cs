using System;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Base
{
	public class StatisticsData : SecurityData
	{

		[EditorConfig(Section = "Statistik", ReadOnly = true, InputType = "string", Sort = 1000)]
		public virtual DateTime Created { get; set; }

		[EditorConfig(Section = "Statistik", ReadOnly = true, InputType = "string", Sort = 1000)]
		public virtual DateTime Updated { get; set; }

	}

}
