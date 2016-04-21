using System;
using System.Linq;
using System.Reactive.Linq;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Utilities;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	class TransferredDataAmountMetrics : MetricsBase
	{
		static TimeSpan WindowSpan { get; } = TimeSpan.FromMilliseconds(100);

		long bytesSent_;

		public TransferredDataAmountMetrics(IKCVDBClient client)
			: base("MetricsTitleTotalDataAmountSent")
		{
			Subscriptions.Add(Observable.FromEventPattern<ApiDataSentEventArgs>(client, nameof(client.ApiDataSent))
				.Select(x => x.EventArgs)
				.Buffer(WindowSpan)
				.SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.Normal)
				.Subscribe(list => {
					bytesSent_ += list.Sum(e => e.SentApiData.PayloadByteCount);
					UpdateValueText();
				}));

			UpdateValueText();
		}

		protected override string ConvertValueToString()
		{
			return MetricsUtil.FormatSize(bytesSent_, 3);
		}
	}
}
