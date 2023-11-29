using System;
using System.Collections.Generic;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Services
{
	public interface IMediaService
	{
		
		void Remove(string path);

		void ClearCache();

		void AddMediaCacheInformations(Dictionary<string, string> result, string path, string root);

		// void UpdateImage(CoreImage img);
		
		// ResponsiveImage CreateResponsiveImage(CoreImage img, WidthAndHeight[] sizes);

		bool CreateImageTransformation(string path);
		CoreImage GetImageById(Guid imagId);
		List<Guid> GetParentIds(Guid imgId);
		CoreVideo GetVideo(Guid itemId);
		string GetImagePath(string path, int targetWidth);
		string GetImagePath2x(string path, int targetWidth);
		string GetImagePath(CoreImage img, int targetWidth, int targetHeight);
		string GetImagePath(CoreImage img, int targetWidth);
		string GetImagePath2x(CoreImage img, int targetWidth);
	}
}
