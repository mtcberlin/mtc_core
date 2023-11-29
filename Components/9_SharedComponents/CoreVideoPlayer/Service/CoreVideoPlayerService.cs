using System;
using System.IO;
using System.Linq;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.Models.Media;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Models;
using static MtcMvcCore.SharedComponents.CoreVideoPlayer.Models.CoreVideoPlayerConfig;

namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Service
{
	public class CoreVideoPlayerService : ICoreVideoPlayerService
	{
		readonly IComponentDataProvider _componentDataProvider;

		public CoreVideoPlayerService(IComponentDataProvider componentDataProvider)
		{
			_componentDataProvider = componentDataProvider;
		}

		public CoreVideoPlayerModel GetData(Guid id)
		{
			return _componentDataProvider.GetDatasource<CoreVideoPlayerModel>(id.ToString());
		}

		public CoreVideoPlayerConfig GetVideo(Guid id)
		{
			var config = new CoreVideoPlayerConfig();
			var coreVideoModel = _componentDataProvider.GetDatasource<CoreVideo>(id.ToString());
			var directory = Path.GetDirectoryName($"/uploads/{coreVideoModel.Id}/");

			if (coreVideoModel.Files != null)
			{
				config.Sources = coreVideoModel.Files.Select(i => new VideoSource {
					src = Path.Combine(directory, i.Filename),
					type = i.MimeType
				});
			}

			if (coreVideoModel.DsgFiles != null)
			{
				config.DgsSources = coreVideoModel.DsgFiles.Select(i => new VideoSource {
					src = Path.Combine(directory, i.Filename),
					type = i.MimeType
				});
			}
			
			if (coreVideoModel.ChaptersFilename != null)
			{
				config.CaptionSrc = Path.Combine(directory, coreVideoModel.ChaptersFilename);
			}

			return config;
		}

		public bool SaveModel(CoreVideoPlayerModel model)
		{
			var orgiModel = _componentDataProvider.GetDatasource<CoreVideoPlayerModel>(model.Id.ToString());
			orgiModel.Video = model.Video;
			return _componentDataProvider.SaveModel<CoreVideoPlayerModel>(orgiModel, orgiModel.Id);
		}

	}

}
