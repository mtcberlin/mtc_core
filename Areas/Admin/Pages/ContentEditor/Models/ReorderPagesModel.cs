using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class ReorderPagesModel
	{
		public List<PageOrderInfoModel> Pages { get; set; }

	}

	public class PageOrderInfoModel
	{
		public Guid PageId { get; set; }
		public int Sort { get; set; }
		public Guid ParentId { get; set; }

	}
}
