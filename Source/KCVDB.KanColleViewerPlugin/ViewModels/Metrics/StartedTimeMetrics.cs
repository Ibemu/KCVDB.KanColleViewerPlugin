using System;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	class StartedTimeMetrics : MetricsBase
	{
		DateTimeOffset StartedAt { get; }

		public StartedTimeMetrics(DateTimeOffset startedAt)
			: base("MetricsTitleStartedAt")
		{
			StartedAt = startedAt;
			UpdateValueText();
		}

		protected override string ConvertValueToString()
		{
			var localDateTime = StartedAt.LocalDateTime;
			return string.Format("{0} {1}",
				localDateTime.ToShortDateString(),
				localDateTime.ToLongTimeString());
		}
	}
}
