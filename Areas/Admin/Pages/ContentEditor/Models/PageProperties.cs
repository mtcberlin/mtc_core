// ReSharper disable once CheckNamespace
using System;

namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class PageProperties
	{
		public string Controller { get; set; }
		public string Action { get; set; }
		public Guid PageConfig { get; set; }
		public string Name { get; set; }
		public string SeoName { get; set; }
	}
}
