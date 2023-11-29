using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MtcMvcCore.Core.Models;
using System.Reflection;
using System.Collections.Generic;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Models;
using MtcMvcCore.Core.Models.PageModels;
using MtcMvcCore.Core;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentEditor.Helper
{
	public static class HtmlHelperExtensions
	{
		public static EditHtmlHelper Edit(this IHtmlHelper htmlHelper)
		{
			return new EditHtmlHelper(htmlHelper);
		}
	}
	public class EditHtmlHelper
	{
		private static string[] complexExcludedTypes = new[] { "decimal", "string", "datetime", "guid" };
		private IHtmlHelper _htmlHelper;

		public EditHtmlHelper(IHtmlHelper htmlHelper)
		{
			_htmlHelper = htmlHelper;
		}
		public IHtmlContent RenderPageEditorFields()
		{
			var fields = new List<PageEditorFieldDefinition>();
			var s = new StringBuilder();

			Type t = typeof(BasePage);
			// Get the public properties.
			AddPageEditorFieldDefinitions(t, fields, string.Empty);

			// Type tPc = typeof(PageConfigurationModel);
			// // Get the public properties.
			// AddPageEditorFieldDefinitions(tPc, fields, string.Empty);

			var grouped = fields.GroupBy(i => i.EditorConfigAttribute.Section);
			foreach (var group in grouped)
			{
				s.Append($"<core-panel expanded='false'><span slot='title'>{group.Key ?? "default"}</span><div slot='content'>");
				foreach (var field in group)
				{
					s.Append(RenderFieldForProperty(field));
				}
				s.Append($"</div></core-panel>");
			}

			return new HtmlString(s.ToString());
		}



		// public static IHtmlContent RenderTextField(this IHtmlHelper htmlHelper, TextField text, string label)
		// {

		// 	var s = new StringBuilder();
		// 	s.Append($"<div class='fld-group'><label for='title'>{label}</label>" +
		// 			$"<input type='text' class='control js-editor-field' id='{label}' name='title' value='{text.Value}'>" +
		// 		"</div>");

		// 	return new HtmlString("");
		// }

		public IHtmlContent RenderImageField<T>(T model, Expression<Func<T, ImageField>> expression, string label)
		{
			var result = expression.Compile().Invoke(model) as ImageField;

			if (result == null)
			{
				result = new ImageField();
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldPath = "",
				FieldType = "",
				FieldValue = result, 
				Tab = null,
				Section = null,
				//Sort = null,
				Readonly = false,
				Validation = null,
				SelectOptions = null,
				AdditionalFieldInfos = null,
				//ChangeHint = editorAttribute.ChangeHint,
				DisplayName = label,
				HasAdditionalFields = true
			};

			var data = new
			{
				definition = fd,
				values = result
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-imagefield-edit data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-imagefield-edit>" +
				"</div>");

			return new HtmlString(s.ToString());
		}


		public IHtmlContent RenderVideoField<T>(T model, Expression<Func<T, VideoField>> expression, string label)
		{
			var result = expression.Compile().Invoke(model) as VideoField;

			if (result == null)
			{
				result = new VideoField();
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldPath = "",
				FieldType = "",
				FieldValue = result, 
				Tab = null,
				Section = null,
				//Sort = null,
				Readonly = false,
				Validation = null,
				SelectOptions = null,
				AdditionalFieldInfos = null,
				//ChangeHint = editorAttribute.ChangeHint,
				DisplayName = label,
				HasAdditionalFields = true
			};

			var data = new
			{
				definition = fd,
				values = result
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-videofield-edit data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-videofield-edit>" +
				"</div>");

			return new HtmlString(s.ToString());
		}



		public IHtmlContent RenderLinkField<T>(T model, Expression<Func<T, LinkField>> expression, string label)
		{
			var result = expression.Compile().Invoke(model) as LinkField;

			if (result == null)
			{
				result = new LinkField();
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldValue = result,
				DisplayName = label,
				HasAdditionalFields = true
			};

			var data = new
			{
				definition = fd
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-link-field data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-link-field>" +
				"</div>");

			return new HtmlString(s.ToString());
		}

		public IHtmlContent RenderTextField<T>(T model, Expression<Func<T, TextField>> expression, string label)
		{
			var result = expression.Compile().Invoke(model) as TextField;

			if (result == null)
			{
				result = new TextField();
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldValue = result,
				DisplayName = label,
				HasAdditionalFields = true
			};

			var data = new
			{
				definition = fd
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-string-field data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-string-field>" +
				"</div>");

			return new HtmlString(s.ToString());
		}


		public IHtmlContent RenderRichTextField<T>(T model, Expression<Func<T, TextField>> expression, string label)
		{
			var result = expression.Compile().Invoke(model) as TextField;

			if (result == null)
			{
				result = new TextField();
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldValue = result,
				DisplayName = label,
				HasAdditionalFields = true
			};

			var data = new
			{
				definition = fd
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-richtext-field data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-richtext-field>" +
				"</div>");

			return new HtmlString(s.ToString());
		}

		public IHtmlContent RenderRadioField<T>(T model, Expression<Func<T, string>> expression, string label, Dictionary<string,string> options)
		{
			var result = expression.Compile().Invoke(model) as string;

			if (result == null)
			{
				result = string.Empty;
			}

			var fd = new EditorFieldDefinition
			{
				FieldName = FirstCharToLowerCase(GetPropertyFieldName(expression)),
				FieldValue = result,
				DisplayName = label,
				HasAdditionalFields = false,
				FieldType = "string",
				GroupLabel = label,
				SelectType = "radio",
				SelectOptions = options.Select(i => new LayoutConfigurationModel{Name=i.Value, Key=i.Key }).ToArray()
			};

			var data = new
			{
				definition = fd
			};

			var s = new StringBuilder();
			s.Append($"<div class='c-fld mb-3'>" +
				 	$"<core-radio-field data-publishchange='false' data-data='{JsonSerializer.Serialize(data)}' class='js-editor-field'></core-radio-field>" +
				"</div>");

			return new HtmlString(s.ToString());
		}

		private static string GetPropertyFieldName<T,K>(Expression<Func<T, K>> expression)
		{

			MemberExpression member = expression.Body as MemberExpression;
			PropertyInfo propInfo = member.Member as PropertyInfo;
			return propInfo.Name;
		}
	
		private static void AddPageEditorFieldDefinitions(Type type, List<PageEditorFieldDefinition> fields, string parentPath)
		{
			PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				var editorAttribute = prop.GetCustomAttributes<EditorConfigAttribute>().FirstOrDefault();
				if (editorAttribute != null && !prop.PropertyType.IsPrimitive && !complexExcludedTypes.Contains(prop.PropertyType.Name.ToLower()))
				{
					var name = string.IsNullOrEmpty(parentPath) ? FirstCharToLowerCase(prop.Name) : $"{parentPath}.{FirstCharToLowerCase(prop.Name)}";
					AddPageEditorFieldDefinitions(prop.PropertyType, fields, name);
				}
				else if (editorAttribute != null)
				{
					var fd = new PageEditorFieldDefinition
					{
						EditorConfigAttribute = editorAttribute,
						Type = prop.PropertyType.Name,
						Name = string.IsNullOrEmpty(parentPath) ? FirstCharToLowerCase(prop.Name) : $"{parentPath}.{FirstCharToLowerCase(prop.Name)}",
						DisplayName = prop.Name
					};
					fields.Add(fd);
				}
			}
		}

		private static string RenderFieldForProperty(PageEditorFieldDefinition field)
		{
			// if (!string.IsNullOrEmpty(field.EditorConfigAttribute.Source))
			// {
			// 	return RenderPageEditorRadioField(field);
			// }

			switch (field.Type)
			{
				case "Guid":
				case "String":
					return RenderPageEditorStringField(field);
				default:
					return string.Empty;
			}
		}

		private static string RenderPageEditorStringField(PageEditorFieldDefinition field)
		{
			var readOnly = string.Empty;

			if (field.EditorConfigAttribute.ReadOnly)
			{
				readOnly = "disabled";
			}

			return $"<div class='fld-group'><label for='{field.Name}'>{field.DisplayName}</label>" +
			$"<input type='text' class='control js-editor-field' id='{field.Name}' name='{field.Name}' placeholder='Index' {readOnly} />" +
			"</div>";
		}

		private static string RenderPageEditorRadioField(PageEditorFieldDefinition field)
		{
			var result = new StringBuilder();
			// var readOnly = string.Empty;

			// if (field.EditorConfigAttribute.ReadOnly)
			// {
			// 	readOnly = "disabled";
			// }

			// var splitted = field.EditorConfigAttribute.RadioSource.Split(",");
			// var ty = Type.GetType(splitted[1].Trim());
			// FieldInfo fieldInfo = ty.GetField(splitted[0]);
			// if (fieldInfo.FieldType.Name != "String[]")
			// {
			// 	return "RadioSource should be a valid String[]";
			// }
			// string[] options = (string[])fieldInfo.GetValue(null);
			// var label = field.EditorConfigAttribute.RadioGroupLabel;
			// result.Append($"<div class='fld-group'><label>{label}</label><div class='control flds-radio-group'>");
			// foreach (var option in options)
			// {
			// 	result.Append($"<div><input type='radio' class='js-editor-field-radio' id='{option}' name='{splitted[0]}' value='{option}'><label for='{option}'>{option}</label></div>");
			// }
			// result.Append($"</div></div>");
			return result.ToString(); ;
		}

		private static string FirstCharToLowerCase(string str)
		{
			if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
				return str;

			return char.ToLower(str[0]) + str.Substring(1);
		}

	}
}