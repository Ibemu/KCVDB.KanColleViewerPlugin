using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.ViewModels.Metrics;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels
{
	class ToolViewViewModel : ViewModelBase, IDisposable
	{
		int MaxHistoryCount { get; } = 20;
		IKCVDBClient Client { get; }
		CompositeDisposable Subscriptions { get; } = new CompositeDisposable();

		public ToolViewViewModel(
			IKCVDBClient client,
			string sessionId,
			IDispatcher dispatcher)
			: base(dispatcher)
		{
			if (client == null) { throw new ArgumentNullException(nameof(client)); }
			if (sessionId == null) { throw new ArgumentNullException(nameof(sessionId)); }
			Client = client;
			SessionId = sessionId;

			// Register metrics
			CustomMetrics.Add(new StartedTimeMetrics(DateTimeOffset.Now));
			CustomMetrics.Add(new TransferredApiCountMetrics(Client));
			CustomMetrics.Add(new SuccessCountMetrics(Client));
			CustomMetrics.Add(new FailureCountMetrics(Client));
			CustomMetrics.Add(new TransferredDataAmountMetrics(Client));
			CustomMetrics.Add(new TransferredDataAmountPerHourMetrics(Client));


			// Register a listener to update history
			Observable.FromEventPattern<ApiDataSentEventArgs>(Client, nameof(Client.ApiDataSent))
				.Select(x => x.EventArgs)
				.Select(x => {
					var now = DateTimeOffset.Now;
					return x.ApiData.Select(apiData => new HistoryItem {
						Time = now,
						Body = apiData.RequestUri.PathAndQuery,
						Success = true,
					});
				})
				.SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.Normal)
				.Subscribe(historyItems => {
					HistoryItems = new ObservableCollection<HistoryItem>(
						historyItems.Reverse().Concat(HistoryItems).Take(MaxHistoryCount));
				});
		}

		#region Bindings

		#region HistoryItems
		ObservableCollection<HistoryItem> historyItems_ = new ObservableCollection<HistoryItem>();
		public ObservableCollection<HistoryItem> HistoryItems
		{
			get
			{
				return historyItems_;
			}
			set
			{
				SetValue(ref historyItems_, value);
			}
		}
		#endregion

		#region CustomMetrics
		ObservableCollection<IMetrics> metrics_ = new ObservableCollection<IMetrics>();
		public ObservableCollection<IMetrics> CustomMetrics
		{
			get
			{
				return metrics_;
			}
			set
			{
				SetValue(ref metrics_, value);
			}
		}
		#endregion

		#region SessionId
		public string SessionId
		{
			get
			{
				return GetValue<string>();
			}
			set
			{
				SetValue(value);
			}
		}
		#endregion

		#endregion // Bindings

		
		#region IDisposable メンバ
		bool isDisposed_ = false;
		virtual protected void Dispose(bool disposing)
		{
		}

		public void Dispose()
		{
			if (isDisposed_) { return; }

			Subscriptions.Dispose();

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
