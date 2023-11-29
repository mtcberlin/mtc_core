using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using MtcMvcCore.Core;
using MtcMvcCore.Core.DataProvider;
using MtcMvcCore.Core.DataProvider.Xml;
using MtcMvcCore.Core.Models.PageModels;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Areas.Admin.Pages.ContentPackages.Services
{
	public class ContentPackagesService : IContentPackagesService
	{
		private readonly ILogger<ContentPackagesService> _logger;
		private const string packageInfoFilename = "info.xml";
		private const string packagesPath = "Data/ContentPackages";
		private const string packageSavePath = packagesPath + "/{0}.zip";
		private const string packagePagesPath = "pages/{0}.xml";
		private const string packageObjectPath = "object/{0}.xml";
		private const string packageVersionsPath = "versions/{0}.xml";
		private const string packageLanguagesPath = "languages/{0}.xml";

		public Dictionary<string, string> _types = new Dictionary<string, string>();

		private readonly IXmlDataProvider _xmlDataProvider;
		private readonly IPageDataProvider _pageDataProvider;
		private readonly IComponentDataProvider _componentDataProvider;

		public ContentPackagesService(ILogger<ContentPackagesService> logger, IXmlDataProvider xmlDataProvider, IPageDataProvider pageDataProvider, IComponentDataProvider componentDataProvider)
		{
			_logger = logger;
			_xmlDataProvider = xmlDataProvider;
			_pageDataProvider = pageDataProvider;
			_componentDataProvider = componentDataProvider;
		}

		public bool Create(Guid pageId, string packageName, bool withSubpages)
		{
			var files = new List<InMemoryFile>();

			AddFilesForPages(pageId, files, withSubpages);
			AddPackageInformation(pageId, files);

			var zip = GetZipArchive(files);

			if (!Directory.Exists("Data/ContentPackages"))
			{
				Directory.CreateDirectory("Data/ContentPackages");
			}

			System.IO.File.WriteAllBytes(string.Format(packageSavePath, packageName), zip);

			return true;
		}

		public bool Install(string filePath)
		{
			XmlDocument xmlDoc = new XmlDocument();
			using (FileStream zipToOpen = new FileStream(filePath, FileMode.Open))
			{
				using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
				{
					var infoXml = archive.GetEntry(packageInfoFilename).Open();
					xmlDoc.Load(infoXml);
					var version = xmlDoc.DocumentElement.SelectSingleNode("//*[@id='version']").Value;
					var firstPage = xmlDoc.DocumentElement.SelectSingleNode("//*[@id='firstelement']").Value;
					var pageTypes = xmlDoc.DocumentElement.SelectSingleNode("//*[@id='pageTypes']").ChildNodes;
					foreach (XmlNode x in pageTypes)
					{
						_types.Add(x.Attributes["id"].Value, x.Attributes["type"].Value);
					}
					foreach (var entry in archive.Entries)
					{
						if (entry.FullName.StartsWith("pages/"))
						{
							SavePage(entry, firstPage);
						} else if(entry.FullName.StartsWith("object/")) {
							SaveComponent(entry);
						}
					}
				}
			}

			return true;
		}

		private void SavePage(ZipArchiveEntry version, string firstPage)
		{
			string id = version.Name.Replace(".xml", string.Empty);
			Type t = Type.GetType(_types[id]);
			XmlSerializer ser = new XmlSerializer(Type.GetType(_types[id]));
			var content = ser.Deserialize(version.Open());
			_pageDataProvider.SavePages(content, id == firstPage);
		}

		private void SaveComponent(ZipArchiveEntry component)
		{
			string id = component.Name.Replace(".xml", string.Empty);
			Type t = Type.GetType(_types[id]);
			XmlSerializer ser = new XmlSerializer(Type.GetType(_types[id]));
			var content = ser.Deserialize(component.Open());
			_componentDataProvider.SaveModel(content, Guid.Parse(id));
		}

		private void AddPackageInformation(Guid pageId, List<InMemoryFile> files)
		{
			XmlDocument doc = new XmlDocument();

			XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement root = doc.DocumentElement;
			doc.InsertBefore(declaration, root);

			XmlElement rootNode = doc.CreateElement("root");
			doc.AppendChild(rootNode);

			XmlElement packageVersion = doc.CreateElement("node");
			packageVersion.SetAttribute("id", "version");
			XmlText versionText = doc.CreateTextNode("1.0.0");
			packageVersion.AppendChild(versionText);

			XmlElement firstElement = doc.CreateElement("node");
			firstElement.SetAttribute("id", "firstelement");
			XmlText firstText = doc.CreateTextNode(pageId.ToString());
			firstElement.AppendChild(firstText);

			XmlElement types = doc.CreateElement("node");
			types.SetAttribute("id", "pageTypes");
			foreach (var type in _types)
			{
				XmlElement typ = doc.CreateElement("node");
				typ.SetAttribute("id", type.Key);
				typ.SetAttribute("type", type.Value);
				types.AppendChild(typ);
			}

			rootNode.AppendChild(packageVersion);
			rootNode.AppendChild(firstElement);
			rootNode.AppendChild(types);

			MemoryStream memStream = new MemoryStream();
			doc.Save(memStream);

			files.Add(new InMemoryFile
			{
				FileName = packageInfoFilename,
				Content = memStream.ToArray()
			});
		}

		private void AddFilesForPages(Guid pageId, List<InMemoryFile> files, bool withSubpages)
		{
			var pages = _pageDataProvider.GetPages(pageId, withSubpages);

			foreach (var page in pages)
			{
				if (!_types.ContainsKey(pageId.ToString()))
				{
					_types.Add(page.Id.ToString(), page.GetType().FullName);
				}

				files.Add(new InMemoryFile
				{
					FileName = string.Format(packagePagesPath, page.Id),
					Content = _xmlDataProvider.AsMemoryStream(page).ToArray()
				});

				AddFilesForComponents(page, files);
			}
		}

		private void AddFilesForComponents(BasePage page, List<InMemoryFile> files)
		{
			if (page.Placeholders != null)
			{
				foreach (var placeholder in page.Placeholders)
				{
					AddComponentsFromPlaceholder(placeholder, files);
				}
			}
		}

		private void AddComponentsFromPlaceholder(Placeholder placeholder, List<InMemoryFile> files)
		{
			foreach (var component in placeholder.Components)
			{
				var componentConfig = SiteConfiguration.GetComponentConfig(component.Name);
				var type = Type.GetType(componentConfig.EditorModel);
				var datasource = _componentDataProvider.GetDatasource(component.Datasource, type.Name);
				_types.Add(component.Datasource, componentConfig.EditorModel);
				files.Add(new InMemoryFile
				{
					FileName = string.Format(packageObjectPath, component.Datasource),
					Content = _xmlDataProvider.AsMemoryStream(datasource).ToArray()
				});
			}
		}

		public static byte[] GetZipArchive(List<InMemoryFile> files)
		{
			byte[] archiveFile;
			using (var archiveStream = new MemoryStream())
			{
				using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
				{
					foreach (var file in files)
					{
						var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
						using (var zipStream = zipArchiveEntry.Open())
							zipStream.Write(file.Content, 0, file.Content.Length);
					}
				}

				archiveFile = archiveStream.ToArray();
			}

			return archiveFile;
		}

		public List<string> GetCurrentPackages()
		{
			var result = new List<string>();
			if (Directory.Exists(packagesPath))
			{
				foreach (var file in Directory.GetFiles(packagesPath))
				{
					result.Add(Path.GetFileNameWithoutExtension(file));
				}
			}

			return result;

		}

		public bool DeletePackage(string name)
		{
			if (Directory.Exists(packagesPath))
			{
				foreach (var file in Directory.GetFiles(packagesPath))
				{
					if (Path.GetFileNameWithoutExtension(file) == name)
					{
						File.Delete(file);
						return true;
					}
				}
			}
			return false;
		}

		public byte[] GetPackageAsByteArray(string name)
		{
			if (Directory.Exists(packagesPath))
			{
				foreach (var file in Directory.GetFiles(packagesPath))
				{
					if (Path.GetFileNameWithoutExtension(file) == name)
					{
						return System.IO.File.ReadAllBytes(file);
					}
				}
			}
			return null;
		}

		public class InMemoryFile
		{
			public string FileName { get; set; }
			public byte[] Content { get; set; }
		}
	}
}

