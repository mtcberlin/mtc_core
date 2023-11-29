using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Helper;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;
using Newtonsoft.Json;
using static MtcMvcCore.Core.Models.Media.CoreVideo;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.AssetLib.Services
{
	public class AssetLibService : IAssetLibService
	{
		private readonly ILogger<AssetLibService> _logger;
		private readonly IMediaDataProvider _mediaDataProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AssetLibService(ILogger<AssetLibService> logger, IMediaDataProvider mediaDataProvider, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_mediaDataProvider = mediaDataProvider;
			_httpContextAccessor = httpContextAccessor;
		}

		public CoreMediaFolder GetTreeViewRootModel()
		{
			return _mediaDataProvider.GetMediaRootFolder();
		}

		public List<EditorFieldDefinition> GetItemDefinition(Guid id)
		{
			var result = new List<EditorFieldDefinition>();

			var asset = _mediaDataProvider.GetAssetById(id);
			if (asset != null && asset.Id != Guid.Empty)
			{
				ModelToEditorFieldDefinition.CreateEditorFieldDefinitions(asset.GetType(), result, "", asset, _httpContextAccessor.HttpContext.User);
			}

			if (result.Any())
			{
				result = result.OrderBy(field => field.Sort).ToList();
			}

			return result;
		}

		public bool CreateImageModel(Guid parentId)
		{
			_mediaDataProvider.CreateImageModel(parentId);
			return true;
		}

		public List<CoreMediaBase> GetSubItems(Guid parentId, string type)
		{
			return _mediaDataProvider.GetSubItems(parentId, type);
		}

		public bool CreateFolderModel(Guid parentId)
		{
			_mediaDataProvider.CreateFolderModel(parentId);
			return true;
		}

		public bool CreateVideoModel(Guid parentId)
		{
			_mediaDataProvider.CreateVideoModel(parentId);
			return true;
		}

		public CoreMediaBase GetAssetById(Guid id)
		{
			return _mediaDataProvider.GetAssetById(id);
		}

		public bool UpdateItem(SaveItemDataModel model)
		{
			var orginalItem = _mediaDataProvider.GetAssetById(model.pageId);

			if (orginalItem is CoreImage)
			{
				var fileField = model.fields.FirstOrDefault(i => i.fieldPath == "fileName");
				if (!string.IsNullOrEmpty(orginalItem.FileName))
				{
					if (fileField != null && fileField.fieldValue == "")
					{
						RemoveImageData(model.pageId);
					}
				}
			}
			else if (orginalItem is CoreVideo)
			{
				var files = model.fields.FirstOrDefault(i => i.fieldPath == "files");
				if ((orginalItem as CoreVideo).Files != null && (orginalItem as CoreVideo).Files.Any())
				{
					if (files != null)
					{
						RemoveVideoFiles(model.pageId, files, (orginalItem as CoreVideo).Files);
					}
				}
				
				var dgsFiles = model.fields.FirstOrDefault(i => i.fieldPath == "dsgFiles");
				if ((orginalItem as CoreVideo).DsgFiles != null && (orginalItem as CoreVideo).DsgFiles.Any())
				{
					if (files != null)
					{
						RemoveVideoFiles(model.pageId, dgsFiles, (orginalItem as CoreVideo).DsgFiles);

					}

				}
			}

			foreach (var field in model.fields)
			{
				var path = field.fieldPath.Split('.');
				SetFieldValue(orginalItem, path.ToList(), field.fieldValue, field.fieldType);
			}

			// if (fileField != null)
			// {
			// 	string contentType;
			// 	new FileExtensionContentTypeProvider().TryGetContentType(fileField.fieldValue, out contentType);
			// 	orginalItem.MimeType = contentType;
			// }

			return _mediaDataProvider.UpdateAsset(orginalItem);
		}

		public bool SaveAsset(CoreMediaBase asset)
		{
			return _mediaDataProvider.UpdateAsset(asset);
		}

		private void RemoveVideoFiles(Guid pageId, SaveItemDataModel.FieldInforamtions newFiles, VideoFileDefinition[] oldFiles)
		{
			string rootPath = "wwwroot";
			string rootDirectory = Directory.GetCurrentDirectory();

			string newFieldValue = newFiles.fieldValue;
			List<VideoFileDefinition> newFileList = JsonConvert.DeserializeObject<List<VideoFileDefinition>>(newFieldValue);
			List<VideoFileDefinition> oldFileList = new List<VideoFileDefinition>(oldFiles);
			List<VideoFileDefinition> deletedFiles = oldFileList.Where(a => !newFileList.Any(x => x.Filename == a.Filename)).ToList();

			if (deletedFiles.Any())
			{
				deletedFiles.ForEach(deletedFile =>
				{
					string filePath = Path.Combine(rootDirectory, rootPath, "uploads", pageId.ToString(), deletedFile.Filename);
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
				});
			}
		}

		private void SetFieldValue<T>(T target, List<string> path, string value, string fieldType)
		{
			var currentProp = path.First();
			PropertyInfo[] propInfos = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				if (prop.Name.ToLower() == currentProp.ToLower())
				{
					if (path.Count == 1)
					{
						switch (fieldType)
						{

							case "video":
								var parsed = JsonConvert.DeserializeObject<VideoFileDefinition[]>(value);
								prop.SetValue(target, parsed);
								break;
							case "string":
							case "file":
							case "image":
								prop.SetValue(target, value);
								break;
							case "boolean":
								prop.SetValue(target, bool.Parse(value));
								break;
							default:
								break;
						}
					}
					else
					{
						path.Remove(currentProp);
						SetFieldValue(prop.GetValue(target, null), path, value, fieldType);
					}
				}
			}
		}

		private bool RemoveImageData(Guid id)
		{
			try
			{
				var orgiImg = _mediaDataProvider.GetAssetById(id) as CoreImage;

				string rootPath = "wwwroot";
				string rootDirectory = Directory.GetCurrentDirectory();

				var filePath = Path.Combine(rootDirectory, rootPath, "uploads", id.ToString(), orgiImg.FileName);
				File.Delete(filePath);
				Directory.Delete(Path.Combine(rootDirectory, rootPath, "uploads", id.ToString()));

				orgiImg.MimeType = string.Empty;
				orgiImg.Width = 0;
				orgiImg.Height = 0;
				orgiImg.FileName = "";

				_mediaDataProvider.UpdateAsset(orgiImg);

				return true;
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
			}
			return false;
		}

		public bool Delete(Guid assetId)
		{
			var orginalItem = _mediaDataProvider.GetAssetById(assetId);
			string rootPath = "wwwroot";
			string rootDirectory = Directory.GetCurrentDirectory();

			if (Directory.Exists(Path.Combine(rootDirectory, rootPath, "uploads", assetId.ToString())))
			{
				Directory.Delete(Path.Combine(rootDirectory, rootPath, "uploads", assetId.ToString()), true);
			}

			return _mediaDataProvider.Delete(assetId);
		}
	}
}

