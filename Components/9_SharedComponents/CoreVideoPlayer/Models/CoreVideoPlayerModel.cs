using System;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;

namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Models
{
    public class CoreVideoPlayerModel
    {
        public Guid Id { get; set; }
		
		[EditorConfig]
        public VideoField Video { get; set; }

        public CoreVideo CoreVideo {get; set;}

    }
}
