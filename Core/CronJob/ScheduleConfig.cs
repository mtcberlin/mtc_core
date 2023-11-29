using System;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.CronJob
{
	public class ScheduleConfig<T> : IScheduleConfig<T>
	{
		public string CronExpression { get; set; }
		public TimeZoneInfo TimeZoneInfo { get; set; }
	}

}
