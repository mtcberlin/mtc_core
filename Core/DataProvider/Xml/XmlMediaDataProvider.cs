
// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;
using MtcMvcCore.Core.Models;
using MtcMvcCore.Core.Models.Media;

namespace MtcMvcCore.Core.DataProvider.Xml
{

	public class XmlMediaDataProvider : IMediaDataProvider
	{
		private IXmlDataProvider _dataProvider;

		public XmlMediaDataProvider(IXmlDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
		}

		public bool CreateFolderModel(Guid parentId)
		{
			throw new NotImplementedException();
		}

		public bool CreateVideoModel(Guid parentId)
		{
			throw new NotImplementedException();
		}

		public bool CreateImageModel(Guid parentId)
		{
			throw new NotImplementedException();
		}

		public CoreMediaBase GetAssetById(Guid id)
		{
			throw new NotImplementedException();
		}

		public CoreMediaFolder GetMediaRootFolder()
		{
			throw new NotImplementedException();
		}

		public List<CoreMediaBase> GetSubItems(Guid parentId, string type)
		{
			throw new NotImplementedException();
		}

		public bool UpdateAsset(CoreMediaBase asset)
		{
			throw new NotImplementedException();
		}

		public bool Delete(Guid assetId)
		{
			throw new NotImplementedException();
		}
	}

}
