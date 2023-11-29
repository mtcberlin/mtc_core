
// ReSharper disable once CheckNamespace
using System;
using System.Collections.Generic;

namespace MtcMvcCore.Areas.Admin.Pages.ContentPackages.Services
{
	public interface IContentPackagesService
	{
		public bool Create(Guid pageId, string name, bool withSubpages);
		public bool Install(string name);
		public List<string> GetCurrentPackages();
		
		bool DeletePackage(string name);
		byte[] GetPackageAsByteArray(string name);
	}
}
