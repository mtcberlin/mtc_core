using System.Collections.Generic;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Media
{
	[XmlRoot]
	public class CoreVideo : CoreMediaBase
	{

		public CoreVideo()
		{
			Type = "video";
		}

		[XmlElement]
		[EditorConfig(InputType = "video", DisplayName = "Video", Section = "Video")]
		public VideoFileDefinition[] Files { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Copyright", Section = "Video")]
		public string Copyright { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Source", Section = "Video")]
		public string Source { get; set; }

		[XmlElement]
		[EditorConfig(InputType = "video", DisplayName = "Gebärdensprache Video", Section = "Sign language video")]
		public VideoFileDefinition[] DsgFiles { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Copyright", Section = "Sign language video")]
		public string CopyrightDgs { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Source", Section = "Sign language video")]
		public string SourceDgs { get; set; }

		[XmlElement]
		[EditorConfig( Section = "Weiteres")]
		public TextField Alt { get; set; }

		[XmlElement]
		[EditorConfig(InputType = "file", DisplayName = "Kapitel", Section = "Weiteres")]
		public string ChaptersFilename { get; set; }
		
		[XmlElement]
		[EditorConfig(InputType = "file", DisplayName = "Beschreibung", Section = "Weiteres")]
		public string DescriptionFilename { get; set; }
		
		[XmlElement]
		[EditorConfig(InputType = "file", DisplayName = "Caption", Section = "Weiteres")]
		public string CaptionFilename { get; set; }

		public class VideoFileDefinition
		{
			public string MimeType { get; set; }

			public string Filename { get; set; }

			public double FileSize { get; set; }
		}

	}

}
