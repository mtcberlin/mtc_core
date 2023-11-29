using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Models.Media;
using SkiaSharp;


// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
	public class MediaService : IMediaService
	{

		private Dictionary<string, SKEncodedImageFormat> EncodeMapping = new Dictionary<string, SKEncodedImageFormat> {
			{ "bmp", SKEncodedImageFormat.Bmp },
			{ "gif", SKEncodedImageFormat.Gif },
			{ "jpeg", SKEncodedImageFormat.Jpeg },
			{ "png", SKEncodedImageFormat.Png },
			{ "webp", SKEncodedImageFormat.Webp }
		};
		private const string rootPath = "wwwroot";
		private readonly string rootDirectory = Directory.GetCurrentDirectory();

		private static Dictionary<string, LocalImageSize> localImageSizes = new Dictionary<string, LocalImageSize>();
		private static Dictionary<string, ImageTransformationInformation> allowedImagePaths = new Dictionary<string, ImageTransformationInformation>();
		private IMediaDataProvider _mediaDataProvider;

		public MediaService(IMediaDataProvider mediaDataProvider)
		{
			_mediaDataProvider = mediaDataProvider;
		}

		public void Remove(string path)
		{
			var filePath = Path.Combine(rootDirectory, rootPath, "temp", TrimPath(path));
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public void ClearCache()
		{
			var filePath = Path.Combine(rootDirectory, rootPath, "temp");
			if (Directory.Exists(filePath))
			{
				Directory.Delete(filePath, true);
			}
		}

		public void AddMediaCacheInformations(Dictionary<string, string> result, string path, string root)
		{
			if (Directory.Exists(path))
			{
				string[] fileEntries = Directory.GetFiles(path);
				foreach (string fileName in fileEntries)
				{
					result.Add(fileName.Replace(root, string.Empty), "Media");
				}

				string[] subdirectoryEntries = Directory.GetDirectories(path);
				foreach (string subdirectory in subdirectoryEntries)
				{
					AddMediaCacheInformations(result, subdirectory, root);
				}
			}
		}

		public bool CreateImageTransformation(string path)
		{
			var decodedPath = WebUtility.UrlDecode(path);
			if (!allowedImagePaths.ContainsKey(decodedPath)) return false;
			if (File.Exists(decodedPath)) return false;

			ImageTransformationInformation info = allowedImagePaths[decodedPath];
			SKBitmap image = SKBitmap.Decode(info.Src);
			TransformImage(info, image, decodedPath);
			return true;
		}

		private void TransformImage(ImageTransformationInformation info, SKBitmap image, string path)
		{
			SKBitmap scaledBitmap = null;
			SKBitmap resultBitmap = new SKBitmap(info.TargetWidth, info.TargetHeight);
			if (info.TargetHeight == 0)
			{
				scaledBitmap = image.Resize(new SKImageInfo(info.TargetWidth, (int)Math.Round((double)image.Height / image.Width * info.TargetWidth, 0)), SKFilterQuality.High);
				resultBitmap = scaledBitmap;
			}
			else if ((int)Math.Round((double)image.Height / image.Width * info.TargetWidth, 0) > info.TargetHeight)
			{
				scaledBitmap = image.Resize(new SKImageInfo(info.TargetWidth, (int)Math.Round((double)image.Height / image.Width * info.TargetWidth, 0)), SKFilterQuality.High);
				SKCanvas canvas = new SKCanvas(resultBitmap);
				canvas.DrawBitmap(scaledBitmap, new SKRect
				{
					Left = 0,
					Right = scaledBitmap.Width,
					Top = scaledBitmap.Height / 2 - info.TargetHeight / 2,
					Bottom = scaledBitmap.Height / 2 + info.TargetHeight / 2
				}, new SKRect
				{
					Left = 0,
					Top = 0,
					Right = info.TargetWidth,
					Bottom = info.TargetHeight
				});
			}
			else
			{
				scaledBitmap = image.Resize(new SKImageInfo((int)Math.Round((double)image.Width / image.Height * info.TargetHeight, 0), info.TargetHeight), SKFilterQuality.High);
				SKCanvas canvas = new SKCanvas(resultBitmap);
				canvas.DrawBitmap(scaledBitmap, new SKRect
				{
					Left = scaledBitmap.Width / 2 - info.TargetWidth / 2,
					Right = scaledBitmap.Width / 2 + info.TargetWidth / 2,
					Top = 0,
					Bottom = scaledBitmap.Height
				}, new SKRect
				{
					Left = 0,
					Top = 0,
					Right = info.TargetWidth,
					Bottom = info.TargetHeight
				});
			}

			Directory.CreateDirectory(Path.Combine(rootDirectory, rootPath, Path.GetDirectoryName(TrimPath(path))));
			var imagePath = Path.Combine(rootDirectory, rootPath, TrimPath(path));

			using (var stream = System.IO.File.Create(imagePath))
			{
				resultBitmap.Encode(stream, EncodeMapping[Settings.ImageTransformFormat], 90);
				resultBitmap.Dispose();
				scaledBitmap.Dispose();
			}

		}

		private string TrimPath(string path)
		{
			if (path.Trim().StartsWith("/") || path.Trim().StartsWith("\\"))
			{
				return path.Trim().Substring(1);
			}
			return path;
		}

		private string WebPath(string path)
		{
			var newPath = path.Replace("\\", "/");
			if (!newPath.StartsWith("/"))
			{
				newPath = string.Concat("/", newPath);
			}
			return newPath;
		}


		private string GetTargetFilename(CoreImage img, int width, int height, double scaleFaktor)
		{
			var basePath = "temp";
			var directory = Path.GetDirectoryName($"/uploads/{img.Id}/");
			var filenameWithoutExtension = Path.GetFileNameWithoutExtension(TrimPath(img.FileName));

			int heightCalc;
			if (height == 0)
			{
				heightCalc = (int)Math.Round((int)Math.Round((double)img.Height / img.Width * width, 0) * scaleFaktor, 0);
			}
			else
			{
				heightCalc = (int)Math.Round(height * scaleFaktor, 0);
			}

			var filename = $"{filenameWithoutExtension}_{(int)Math.Round(width * scaleFaktor, 0)}_{heightCalc}.{Settings.ImageTransformFormat}";

			var path = Path.Combine(basePath, TrimPath(directory), filename).ToString();
			return path;
		}

		private string GetTargetFilename(string path, int width, double scaleFaktor)
		{
			var basePath = "temp";
			var directory = Path.GetDirectoryName(path);
			var filenameWithoutExtension = Path.GetFileNameWithoutExtension(TrimPath(path));
			var sizes = localImageSizes[path];

			var filename = $"{filenameWithoutExtension}_{(int)Math.Round(width * scaleFaktor, 0)}_{(int)Math.Round((int)Math.Round((double)sizes.Height / sizes.Width * width, 0) * scaleFaktor, 0)}.{Settings.ImageTransformFormat}";

			return Path.Combine(basePath, TrimPath(directory), filename).ToString();
		}

		public CoreImage GetImageById(Guid imgId)
		{
			return _mediaDataProvider.GetAssetById(imgId) as CoreImage;
		}

		public List<Guid> GetParentIds(Guid imgId)
		{
			var result = new List<Guid>();
			var currentId = imgId;
			var rootId = Guid.Parse("{22222222-2222-2222-2222-222222222222}");
			while (currentId != rootId)
			{
				var parentId = GetParentId(currentId);
				result.Add(parentId);
				currentId = parentId;
			}

			return result;
		}

		private Guid GetParentId(Guid id)
		{
			var asset = _mediaDataProvider.GetAssetById(id);
			return asset.ParentId;
		}

		public CoreVideo GetVideo(Guid itemId)
		{
			var asset = (CoreVideo)_mediaDataProvider.GetAssetById(itemId);
			return asset;
		}

		public string GetImagePath(string path, int targetWidth)
		{
			return GetImagePath(path, targetWidth, 1);
		}

		public string GetImagePath2x(string path, int targetWidth)
		{
			return GetImagePath(path, targetWidth, 2);
		}

		private string GetImagePath(string path, int targetWidth, int scale)
		{
			if (!localImageSizes.ContainsKey(path))
			{
				ReadLocalImageSize(path);
			}
			string targetFilename = GetTargetFilename(path, targetWidth, scale);

			// check if the image is already transformed
			var webPath = WebPath(targetFilename);

			if (!allowedImagePaths.ContainsKey(webPath))
			{
				allowedImagePaths.Add(webPath, new ImageTransformationInformation
				{
					TargetWidth = targetWidth * scale,
					Src = Path.Combine(rootDirectory, rootPath, TrimPath(path))
				});
			}

			return webPath;
		}

		public string GetImagePath(CoreImage img, int targetWidth, int targetHeight)
		{
			return GetImagePath(img, targetWidth, targetHeight, 1);
		}

		public string GetImagePath(CoreImage img, int targetWidth)
		{
			return GetImagePath(img, targetWidth, 0, 1);
		}

		public string GetImagePath2x(CoreImage img, int targetWidth)
		{
			return GetImagePath(img, targetWidth, 0, 2);
		}

		private string GetImagePath(CoreImage img, int targetWidth, int targetHeight, int scale)
		{
			if(string.IsNullOrEmpty(img.FileName)) {
				return "/assets/dummy-300x163.gif";
			}

			if (img.Width == 0 || img.Height == 0)
			{
				UpdateImageInformations(img);
			}
			string targetFilename = GetTargetFilename(img, targetWidth, targetHeight, scale);

			// check if the image is already transformed
			var webPath = WebPath(targetFilename);

			if (!allowedImagePaths.ContainsKey(webPath))
			{
				allowedImagePaths.TryAdd(webPath, new ImageTransformationInformation
				{
					TargetWidth = targetWidth * scale,
					TargetHeight = targetHeight * scale,
					ImageHeight = img.Height,
					ImageWidth = img.Width,
					Src = Path.Combine(rootDirectory, rootPath, Path.Combine("uploads", img.Id.ToString(), img.FileName))
				});
			}

			return webPath;
		}

		private void UpdateImageInformations(CoreImage img)
		{
			if (img == null) return;

			var savePath = string.IsNullOrEmpty(img.SavePath) ? $"/uploads/{img.Id}" : img.SavePath;

			// check if the file exist
			var path = Path.Combine(rootDirectory, rootPath, TrimPath(Path.Combine(savePath, img.FileName)));

			if (!File.Exists(path)) return;

			SKBitmap image = SKBitmap.Decode(path);

			// check if the image could loaded
			if (image == null || image.Width == 0 && image.Height == 0) return;

			img.Width = image.Width;
			img.Height = image.Height;
			//SET MimeType

			_mediaDataProvider.UpdateAsset(img);
		}

		private void ReadLocalImageSize(string path)
		{
			// check if the file exist
			var imgPath = Path.Combine(rootDirectory, rootPath, TrimPath(path));

			if (!File.Exists(imgPath)) return;

			SKBitmap image = SKBitmap.Decode(imgPath);

			// check if the image could loaded
			if (image == null || image.Width == 0 && image.Height == 0) return;

			localImageSizes.Add(path, new LocalImageSize
			{
				Width = image.Width,
				Height = image.Height
			});
		}

		class ImageTransformationInformation
		{
			public string Src { get; set; }
			public int TargetWidth { get; set; }
			public int TargetHeight { get; set; }
			public int ImageHeight { get; set; }
			public int ImageWidth { get; set; }
		}

		class LocalImageSize
		{
			public int Width { get; set; }
			public int Height { get; set; }
		}
	}
}
