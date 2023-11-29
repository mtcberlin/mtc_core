
// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;

namespace MtcMvcCore.Areas.Admin.Pages.AssetLib.Services
{
	public interface IAssetLibService
	{
		CoreMediaFolder GetTreeViewRootModel();
		List<EditorFieldDefinition> GetItemDefinition(Guid id);

		bool CreateImageModel(Guid parentId);
		bool CreateFolderModel(Guid parentId);
		bool CreateVideoModel(Guid parentId);
		
		List<CoreMediaBase> GetSubItems(Guid parentId, string type);
		CoreMediaBase GetAssetById(Guid id);
		bool UpdateItem(SaveItemDataModel model);
		bool SaveAsset(CoreMediaBase asset);
		bool Delete(Guid assetId);
	}
}
