using System;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.CronJob
{
	public interface IScheduleConfig<T>
	{
		string CronExpression { get; set; }
		TimeZoneInfo TimeZoneInfo { get; set; }
	}

}
