using MtcMvcCore.Core.Models;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
	public interface IContextService
	{
		public PageContextModel RootContext { get; }
		public PageContextModel PageContext { get; }
		public PageContextModel PageContextOverride { get; set; }
		public string CurrentLanguage { get; }
		public bool IsEasyLanguage { get; }

	}
}
