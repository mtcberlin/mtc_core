using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.AssetEditor.Models
{
	public class ReorderAssetsModel
	{
		public List<AssetOrderInfoModel> Items { get; set; }

	}

	public class AssetOrderInfoModel
	{
		public Guid ItemId { get; set; }
		public int Sort { get; set; }
		public Guid ParentId { get; set; }

	}
}
