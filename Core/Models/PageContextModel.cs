// ReSharper disable once CheckNamespace
using MtcMvcCore.Core.Models.PageModels;

namespace MtcMvcCore.Core.Models
{
	public class PageContextModel
	{
		public string SeoUrlWithoutLang { get; set; }
		public BasePage Page { get; set; }
		public string Language { get; set; }
		
	}
}

