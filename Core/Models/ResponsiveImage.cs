using System.Collections.Generic;
using MtcMvcCore.Core.Models.Media;

namespace MtcMvcCore.Core.Models
{
	public class ResponsiveImage
	{
		public List<CoreImage> Images { get; set; } = new List<CoreImage>();

		public string Alt { get; set; }
	}

	public class WidthAndHeight
	{
		public WidthAndHeight(int width, int height)
		{
			Width = width;
			Height = height;
		}
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
