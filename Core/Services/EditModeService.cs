using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Services;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.Controllers
{
	public class EditModeService: IEditModeService
	{
		private readonly ILogger<EditModeService> _logger;

		public EditModeService(ILogger<EditModeService> logger)
		{
			_logger = logger;
		}
		
		public void TrySaveChanges(List<ContentChangeSet> changes)
		{
			foreach (var contentChangeSet in changes)
			{
				var filePath = WebUtility.UrlDecode(contentChangeSet.FilePath);
				if (!string.IsNullOrEmpty(filePath))
				{
					var doc = LoadXmlDocument(filePath);
					var nodes = doc.GetElementsByTagName(contentChangeSet.FieldName.ToLower());
					for (var i = 0; i < nodes.Count; i++)
					{
						if (IsSameValue(nodes[i].InnerXml, contentChangeSet.OldValue))
						{
							nodes[i].InnerXml = WebUtility.UrlDecode(contentChangeSet.Value);
						}
					}

					SaveToFile(doc, filePath);
				}
			}
		}

		private void SaveToFile(XmlDocument doc, string filePath)
		{
			if (filePath.Contains(".edited") || !Settings.EnableFileVersions)
			{
				doc.Save(filePath);
			}
			else
			{
				var cleanFilePath = filePath.Replace(".xml", string.Empty);
				doc.Save($"{cleanFilePath}.edited.xml");
			}
		}

		private bool IsSameValue(string fileValue, string oldValue)
		{
			if (WebUtility.UrlDecode(oldValue) == "[No text in field]" && string.IsNullOrEmpty(fileValue))
			{
				return true;
			}
			return fileValue == WebUtility.UrlDecode(oldValue);
		}

		private XmlDocument LoadXmlDocument(string filePath)
		{
			try
			{
				var doc = new XmlDocument();
				doc.Load(WebUtility.UrlDecode(filePath));
				return doc;
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Error while loading {filePath}");
			}

			return null;
		}
	}
}
