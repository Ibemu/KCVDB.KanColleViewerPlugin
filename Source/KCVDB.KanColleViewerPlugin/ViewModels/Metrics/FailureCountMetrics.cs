using System;
using System.Linq;
using System.Reactive.Linq;
using KCVDB.Client;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	class FailureCountMetrics : MetricsBase
	{
		static TimeSpan WindowSpan { get; } = TimeSpan.FromMilliseconds(100);

		long count_;
		
		public FailureCountMetrics(IKCVDBClient client)
			: base("MetricsTitleFailureCount")
		{
			Subscriptions.Add(Observable.FromEventPattern<SendingErrorEventArgs>(client, nameof(client.SendingError))
				.Select(x => x.EventArgs)
				.Buffer(WindowSpan)
				.SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.Normal)
				.Subscribe(list => {
					count_ += list.Count;
					UpdateValueText();
				}));

			UpdateValueText();
		}

		protected override string ConvertValueToString()
		{
			return count_.ToString();
		}
	}
}
