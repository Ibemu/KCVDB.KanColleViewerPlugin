using System;
using System.Linq;
using System.Reactive.Linq;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Telemetry;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	class SuccessCountMetrics : MetricsBase
	{
		static TimeSpan WindowSpan { get; } = TimeSpan.FromMilliseconds(100);

		long count_;

		public SuccessCountMetrics(IKCVDBClient client)
			: base("MetricsTitleSuccessCount")
		{
			Subscriptions.Add(Observable.FromEventPattern<ApiDataSentEventArgs>(client, nameof(client.ApiDataSent))
				.Select(x => x.EventArgs)
				.Buffer(WindowSpan)
				.SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.Normal)
				.Subscribe(list => {
					try {
						count_ += list.Count;
						UpdateValueText();
					}
					catch (Exception ex) {
						if (ex.IsCritical()) { throw; }
						TelemetryClient.TrackException("Failed to update success count metrics.", ex);
					}
				}));

			UpdateValueText();
		}

		protected override string ConvertValueToString()
		{
			return count_.ToString();
		}
	}
}
