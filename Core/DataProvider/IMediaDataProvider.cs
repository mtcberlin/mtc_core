using System;
using System.Collections.Generic;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider
{
	public interface IMediaDataProvider
	{
		CoreMediaFolder GetMediaRootFolder();
		
		bool CreateImageModel(Guid parentId);
		
		List<CoreMediaBase> GetSubItems(Guid parentId, string type);
		bool CreateFolderModel(Guid parentId);
		bool CreateVideoModel(Guid parentId);
		
		CoreMediaBase GetAssetById(Guid id);
		bool UpdateAsset(CoreMediaBase asset);
		bool Delete(Guid assetId);
	}
}
