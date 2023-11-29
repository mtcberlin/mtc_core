using System;
using MtcMvcCore.SharedComponents.CoreVideoPlayer.Models;

namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Service
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public interface ICoreVideoPlayerService
    {
        CoreVideoPlayerModel GetData(Guid id);
		CoreVideoPlayerConfig GetVideo(Guid id);

		bool SaveModel(CoreVideoPlayerModel model);
	}

}
