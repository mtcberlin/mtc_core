// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.PageModels
{
	public class WebRoot : BaseItem {

		[EditorConfig(Section = "Common", DisplayName = "Display Name", Sort = 200, ReadOnly = true)]
		public override string Name { get; set; }

	 }

}
