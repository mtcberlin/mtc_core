using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using MtcMvcCore.Core.Models;
using Microsoft.Extensions.Localization;
using MtcMvcCore.Areas.Admin.Pages.ContentEditor.Controller;

namespace MtcMvcCore.Core.Helper
{
	public static class ModelToEditorFieldDefinition
	{

		private static string[] complexExcludedTypes = new[] { "decimal", "string", "datetime", "guid", "linkfield", "imagefield" };

		public static void CreateEditorFieldDefinitions(Type type, List<EditorFieldDefinition> fields, string parentPath, dynamic model, ClaimsPrincipal user, IStringLocalizer<ContentEditorController> stringLocalizer = null)
		{
			PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var prop in propInfos)
			{
				if (!prop.CustomAttributes.Any()) continue;

				var attr = prop.GetCustomAttributes<EditorConfigAttribute>();
				if (!attr.Any()) continue;

				var editorAttribute = attr.First();
				if (editorAttribute != null && !prop.PropertyType.IsPrimitive && !complexExcludedTypes.Contains(prop.PropertyType.Name.ToLower()) && !prop.PropertyType.IsArray && !editorAttribute.HasAdditionalFields && !editorAttribute.HideInEditor)
				{
					var name = string.IsNullOrEmpty(parentPath) ? FirstCharToLowerCase(prop.Name) : $"{parentPath}.{FirstCharToLowerCase(prop.Name)}";
					var value = prop.GetValue(model);
					if (value == null)
					{
						value = Activator.CreateInstance(prop.PropertyType);
					}
					if (model == null)
					{
						model = Activator.CreateInstance(type);
					}
					//CreateEditorFieldDefinitions(prop.PropertyType, fields, name, prop.GetValue(model), user);
					CreateEditorFieldDefinitions(value.GetType(), fields, name, value, user);
				}
				else if (editorAttribute != null && !editorAttribute.HideInEditor)
				{
					if (!string.IsNullOrEmpty(editorAttribute.Authorize))
					{
						var hasOneRole = false;
						var roles = editorAttribute.Authorize.Split(',');
						foreach (var role in roles)
						{
							if (user.IsInRole(role))
							{
								hasOneRole = true;
							}
						}
						if (!hasOneRole) continue;
					}
					if (model == null)
					{
						model = Activator.CreateInstance(type);
					}
					var value = prop.GetValue(model);
					if (value == null)
					{
						value = "";
					}
					var fd = new EditorFieldDefinition
					{
						FieldName = FirstCharToLowerCase(prop.Name),
						FieldPath = string.IsNullOrEmpty(parentPath) ? FirstCharToLowerCase(prop.Name) : $"{parentPath}.{FirstCharToLowerCase(prop.Name)}",
						FieldType = string.IsNullOrEmpty(editorAttribute.InputType) ? prop.PropertyType.Name.ToLower() : editorAttribute.InputType,
						FieldValue = value,
						Tab = editorAttribute.Tab != null ? Translation.Translate(editorAttribute.Tab) : editorAttribute.Tab,
						Section = stringLocalizer != null ? Translation.Translate(editorAttribute.Section) : editorAttribute.Section,
						Sort = editorAttribute.Sort,
						Readonly = editorAttribute.ReadOnly,
						Validation = null,
						SelectOptions = null,
						AdditionalFieldInfos = null,
						ChangeHint = editorAttribute.ChangeHint,
						//DisplayName = editorAttribute.DisplayName,
						DisplayName = editorAttribute.DisplayName != null ? Translation.Translate(editorAttribute.DisplayName) : Translation.Translate(prop.Name),
						HasAdditionalFields = editorAttribute.HasAdditionalFields,
						FieldConfig = editorAttribute.FieldConfig
					};
					if (editorAttribute is RadioEditorConfigAttribute)
					{
						fd.SelectType = "radio";
						fd.SelectOptions = GetSelectOptions(((RadioEditorConfigAttribute)editorAttribute).Source);
						fd.GroupLabel = Translation.Translate(((RadioEditorConfigAttribute)editorAttribute).GroupLabel);
					}
					fields.Add(fd);
				}
			}
		}

		private static string FirstCharToLowerCase(string str)
		{
			if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
				return str;

			return char.ToLower(str[0]) + str.Substring(1);
		}

		private static LayoutConfigurationModel[] GetSelectOptions(string source)
		{
			var splitted = source.Split(",");
			var ty = Type.GetType(splitted[1].Trim());
			FieldInfo fieldInfo = ty.GetField(splitted[0]);
			if (fieldInfo.FieldType.Name != "LayoutConfigurationModel[]")
			{
				return new LayoutConfigurationModel[] { };
			}
			LayoutConfigurationModel[] options = (LayoutConfigurationModel[])fieldInfo.GetValue(null);

			foreach (var obj in options)
			{
				obj.Name = Translation.Translate(obj.Name);
			}
			return options;
		}
	}
}
