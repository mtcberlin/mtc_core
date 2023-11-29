using System.Collections.Generic;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models
{
	public class ContentEditorTreeViewModel
	{
		public List<BaseItem> Roots { get; set; }
		public ContentEditorFolderInfo Files { get; set; }
	}

	public class ContentEditorFolderInfo
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public List<ContentEditorFolderInfo> Folder { get; set; } = new List<ContentEditorFolderInfo>();

		public List<ContentEditorFileInfo> Files { get; set; } = new List<ContentEditorFileInfo>();
	}
	
	public class ContentEditorFileInfo
	{
		public string Path { get; set; }

		public string Name { get; set; }
		public bool HasEditedFile { get; set; }

	}
}
