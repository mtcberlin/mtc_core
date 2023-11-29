// ReSharper disable once CheckNamespace
using System;

namespace MtcMvcCore.Core.Models
{
	public class PageEditorPageViewModel
	{
		public Guid PageId { get; set; }
		public string SeoName { get; set; }

		public bool NoLangVersion { get; set; }
	}
}

