
// ReSharper disable once CheckNamespace
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Core.Models;

namespace MtcMvcCore.Core.Models
{
	public class EditorFieldDefinition
	{
		public string FieldName {get; set;}
		public string FieldType {get; set;}
		public string FieldPath {get; set;}
		public dynamic FieldValue {get; set;}
		public string Tab {get; set;}
		public string Section {get; set;}
		public int Sort {get; set;}
		public bool Readonly {get; set;}
		public string Validation {get; set;}
		public string SelectType {get; set;}
		public LayoutConfigurationModel[] SelectOptions {get; set;}
		public string GroupLabel {get; set;}
		public string AdditionalFieldInfos {get; set;}
		public string ChangeHint { get; set; }
		public string DisplayName { get; set; }
		public string FieldConfig { get; set; }
		public bool HasAdditionalFields { get; set; }
	}
}
