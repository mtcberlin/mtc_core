using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace MtcMvcCore.Core.CronJob
{
	public class SampleCron : Services.CronJob
	{
		private readonly ILogger<SampleCron> _logger;

		public SampleCron(IScheduleConfig<SampleCron> config, ILogger<SampleCron> logger)
			: base(config.CronExpression, config.TimeZoneInfo)
		{
			_logger = logger;
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("CronJob starts.");
			return base.StartAsync(cancellationToken);
		}

		protected override Task DoWork(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob is working.");
			return Task.CompletedTask;
		}

		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("CronJob is stopping.");
			return base.StopAsync(cancellationToken);
		}
    }
}
