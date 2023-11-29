using System;

namespace MtcMvcCore.Core.Models
{
	public class EditorConfigAttribute : Attribute
	{
		public bool ReadOnly { get; set; }

		public string Section { get; set; }

		public string Authorize { get; set; }

		public string Tab { get; set; }

		public int Sort { get; set; } = 999;

		public string ChangeHint { get; set; }

		public string DisplayName { get; set; }

		public string InputType { get; set; }
		public string FieldConfig { get; set; }

		public bool HasAdditionalFields { get; set; }
		public bool HideInEditor { get; set; }
	}

	public class RadioEditorConfigAttribute : EditorConfigAttribute
	{
		public string Source { get; set; }

		public string GroupLabel { get; set; }

	}
}