using System;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MtcMvcCore.Core.Models.Media;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Models.Media
{
	[XmlRoot]
	public class CoreImage : CoreMediaBase
	{

		public CoreImage() {
			Type = "image";
		}

		[XmlElement]
		[EditorConfig(InputType = "image", Section = "Daten")]
		public override string FileName { get; set; }

		[XmlElement]
		[EditorConfig(ReadOnly = true, Section = "Daten")]
		public override string MimeType { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Copyright", Section = "Copyright")]
		public string Copyright { get; set; }

		[XmlElement]
		[EditorConfig(DisplayName = "Source", Section = "Copyright")]
		public string Source { get; set; }

		[XmlElement]
		[EditorConfig(HasAdditionalFields = true, InputType = "string", DisplayName = "Caption", Section = "Daten")]
		public TextField Caption { get; set; } = new TextField();

		[XmlElement]
		[EditorConfig(HasAdditionalFields = true, InputType = "string", DisplayName = "Beschreibung", Section = "Daten")]
		public TextField Description { get; set; } = new TextField();

		[XmlElement]
		[EditorConfig(HasAdditionalFields = true, InputType = "string", DisplayName = "Alternativtext", Section = "Daten")]
		public TextField Alt { get; set; } = new TextField();

	}

}
