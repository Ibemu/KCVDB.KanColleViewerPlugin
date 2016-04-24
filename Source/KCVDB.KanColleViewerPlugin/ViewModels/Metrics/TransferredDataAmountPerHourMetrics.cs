using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Telemetry;
using KCVDB.KanColleViewerPlugin.Utilities;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	class TransferredDataAmountPerHourMetrics : MetricsBase
	{
		static TimeSpan DueTime { get; } = TimeSpan.FromSeconds(10);
		static TimeSpan Period { get; } = TimeSpan.FromSeconds(5);
		static TimeSpan WindowSpan { get; } = TimeSpan.FromMilliseconds(100);

		LinkedList<Tuple<long, int>> History { get; } = new LinkedList<Tuple<long, int>>();
		object ListLock { get; } = new object();

		bool isListUpdated_;
		long cachedSize_;

		public TransferredDataAmountPerHourMetrics(IKCVDBClient client)
			: base("MetricsTitleDataAmountPerHour")
		{
			Subscriptions.Add(Observable.FromEventPattern<ApiDataSentEventArgs>(client, nameof(client.ApiDataSent))
				.Select(x => x.EventArgs)
				.Buffer(WindowSpan)
				.Subscribe(list => {
					try {
						var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
						lock (ListLock) {
							foreach (var e in list) {
								History.AddLast(new Tuple<long, int>(now, e.SentApiData.PayloadByteCount));
								isListUpdated_ = true;
							}
						}
					}
					catch (Exception ex) {
						if (ex.IsCritical()) { throw; }
						TelemetryClient.TrackException("Failed to add transferred api info to the list.", ex);
					}
				}));

			Subscriptions.Add(Observable.Timer(DueTime, Period)
				.Select(x => {
					CleanupAndUpdate(DateTimeOffset.Now);
					return x;
				})
				.SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.Normal)
				.Subscribe(_ => {
					try {
						UpdateValueText();
					}
					catch (Exception ex) {
						if (ex.IsCritical()) { throw; }
						TelemetryClient.TrackException("Failed to update transferred data mount per hour metrics.", ex);
					}
				}));

			UpdateValueText();
		}

		public void CleanupAndUpdate(DateTimeOffset now)
		{
			var cutLine = now.AddHours(-1).ToUnixTimeMilliseconds();
			lock (ListLock) {
				var length = History.Count;
				for (int i = 0; i < length; i++) {
					var item = History.First();
					if (item.Item1 < cutLine) {
						History.RemoveFirst();
					}
					else {
						break;
					}
				}

				isListUpdated_ |= length != History.Count;
			}

			if (isListUpdated_) {
				cachedSize_ = History.Any() ? History.Sum(x => x.Item2) : 0;
			}
		}

		protected override string ConvertValueToString()
		{
			return MetricsUtil.FormatSize(cachedSize_, 3);
		}
	}
}
