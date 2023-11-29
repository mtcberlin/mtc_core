using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.CronJob
{
	public static class ScheduledServiceExtensions
	{
		public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : Services.CronJob
		{
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
			}
			var config = new ScheduleConfig<T>();
			options.Invoke(config);
			if (string.IsNullOrWhiteSpace(config.CronExpression))
			{
				throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
			}

			services.AddSingleton<IScheduleConfig<T>>(config);
			services.AddHostedService<T>();
			return services;
		}
	}
}
