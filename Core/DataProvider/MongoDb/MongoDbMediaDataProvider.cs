using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MtcMvcCore.Core.Models.Media;
using NLog;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.DataProvider.MongoDb
{

	public class MongoDbMediaDataProvider : IMediaDataProvider
	{

		private readonly Logger _logger;
		private readonly IMongoDbDataProvider _dbDataProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MongoDbMediaDataProvider(IMongoDbDataProvider dbDataProvider, IHttpContextAccessor httpContextAccessor)
		{
			_logger = LogManager.GetCurrentClassLogger();
			_dbDataProvider = dbDataProvider;
			_httpContextAccessor = httpContextAccessor;
		}

		public bool CreateFolderModel(Guid parentId)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			_dbDataProvider.Create(
				new CoreMediaFolder
				{
					Id = Guid.NewGuid(),
					Name = "New Folder",
					ParentId = parentId,
					ContentVersion = 1,
					Created = DateTime.Now,
					CreatedBy = userIdClaim.Value
				}
			);
			return true;
		}

		public bool CreateVideoModel(Guid parentId)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			_dbDataProvider.Create(
				new CoreVideo
				{
					Id = Guid.NewGuid(),
					Name = "New Video",
					ParentId = parentId,
					ContentVersion = 1,
					Created = DateTime.Now,
					CreatedBy = userIdClaim.Value
				}
			);
			return true;
		}

		public bool CreateImageModel(Guid parentId)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			_dbDataProvider.Create(
				new CoreImage
				{
					Id = Guid.NewGuid(),
					Name = "New Image",
					ParentId = parentId,
					ContentVersion = 1,
					Created = DateTime.Now,
					CreatedBy = userIdClaim.Value
				}
			);
			return true;
		}

		public CoreMediaBase GetAssetById(Guid id)
		{
			var media = _dbDataProvider.Get<CoreMediaBase, Guid>("Id", id);
			return media;
		}

		public CoreMediaFolder GetMediaRootFolder()
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			CoreMediaFolder root = _dbDataProvider.Get<CoreMediaFolder, Guid>("Id", Guid.Parse("{22222222-2222-2222-2222-222222222222}"));
			if (_httpContextAccessor.HttpContext.User.IsInRole("Administrator"))
			{
				root.HasSubItems = _dbDataProvider.Where<CoreMediaBase, Guid>("ParentId", root.Id).Count > 0;
			}
			else
			{
				var all = _dbDataProvider.Where<CoreMediaBase, Guid>("ParentId", root.Id);
				root.HasSubItems = all.Count(i => i.CreatedBy == userIdClaim.Value) > 0;
			}
			root.InsertOptions = new List<object>{
				new{displayName = "Folder", insertType = "folder"},
				new{displayName = "Image", insertType = "image"},
				new{displayName = "Video", insertType = "video"},
				//new{displayName = "Audio", insertType = "audio"}
			};
			return root;
		}

		public List<CoreMediaBase> GetSubItems(Guid parentId, string type)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			var resultList = new List<CoreMediaBase>();
			var subItems = _dbDataProvider.Where<CoreMediaBase, Guid>("ParentId", parentId);
			if (!_httpContextAccessor.HttpContext.User.IsInRole("Administrator"))
			{
				subItems = subItems.Where(i => i.CreatedBy == userIdClaim.Value).ToList();
			}
			foreach (var sub in subItems.OrderBy(i => i.Sort))
			{
				if ((string.IsNullOrEmpty(type) && sub.Type != "folder") || sub.Type == type)
				{
					resultList.Add(sub);
				}
				else if (sub.Type == "folder")
				{
					sub.HasSubItems = _dbDataProvider.Where<CoreMediaBase, Guid>("ParentId", sub.Id).Count > 0;
					sub.InsertOptions = new List<object>{
						new{displayName = "Folder", insertType = "folder"},
						new{displayName = "Image", insertType = "image"},
						new{displayName = "Video", insertType = "video"},
						//new{displayName = "Audio", insertType = "audio"}
					};
					resultList.Add(sub);
				}
			}

			return resultList;
		}

		public bool UpdateAsset(CoreMediaBase asset)
		{
			var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
			asset.Updated = DateTime.Now;
			asset.UpdatedBy = userIdClaim.Value;
			return _dbDataProvider.Update<CoreMediaBase>(asset, asset.Id);
		}

		public bool Delete(Guid assetId)
		{
			_dbDataProvider.Delete<CoreMediaBase>(assetId);
			return true;
		}
	}

}
