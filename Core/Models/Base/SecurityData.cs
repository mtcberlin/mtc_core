
// ReSharper disable once CheckNamespace
using MongoDB.Extensions.Migration;

namespace MtcMvcCore.Core.Models.Base
{
	public class SecurityData : IVersioned
	{

		[EditorConfig(Section = "Security", ReadOnly = true, Sort = 990)]
		public string CreatedBy { get; set; }

		[EditorConfig(Section = "Security", ReadOnly = true, Sort = 990)]
		public string UpdatedBy { get; set; }

		public int Version { get; set; }

	}

}
