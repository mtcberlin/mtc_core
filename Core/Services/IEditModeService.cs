using System.Collections.Generic;
using MtcMvcCore.Core.Models;

namespace MtcMvcCore.Core.Services
{
	public interface IEditModeService
	{
		void TrySaveChanges(List<ContentChangeSet> changes);

	}
}
