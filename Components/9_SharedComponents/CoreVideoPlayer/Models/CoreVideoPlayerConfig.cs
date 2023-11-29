
using System.Collections.Generic;

namespace MtcMvcCore.SharedComponents.CoreVideoPlayer.Models
{
	public class CoreVideoPlayerConfig
	{
		public IEnumerable<VideoSource> Sources { get; set; }
		public IEnumerable<VideoSource> DgsSources { get; set; }
		
		public string CaptionSrc { get; set; }

		public class VideoSource
		{
			public string src { get; set; }
			public string type { get; set; }
		}
	}
}
