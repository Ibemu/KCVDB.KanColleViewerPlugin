using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using KCVDB.KanColleViewerPlugin.Telemetry;
using Microsoft.ApplicationInsights;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels.Metrics
{
	abstract class MetricsBase : NotificationObject, IMetrics, IDisposable
	{
		protected TelemetryClient TelemetryClient => TelemetryService.Instance.Client;
		protected CompositeDisposable Subscriptions { get; } = new CompositeDisposable();

		public string TitleResourceName { get; }

		public MetricsBase(string titleResourceName)
		{
			if (titleResourceName == null) { throw new ArgumentNullException(nameof(titleResourceName)); }
			TitleResourceName = titleResourceName;

			Subscriptions.Add(Observable.FromEventPattern<PropertyChangedEventArgs>(ResourceHolder.Instance, nameof(ResourceHolder.PropertyChanged))
				.Where(x => x.EventArgs.PropertyName == nameof(ResourceHolder.Resources))
				.SubscribeOnDispatcher()
				.Subscribe(_ => {
					try {
						UpdateTitle();
					}
					catch (Exception ex) {
						TelemetryClient.TrackException("Failed to update the title", ex, new {
							TypeName = this.GetType().FullName
						});
					}
				}));

			UpdateTitle();
		}

		#region Title
		string title_;
		public string Title
		{
			get
			{
				return title_;
			}
			private set
			{
				SetValue(ref title_, value);
			}
		}
		#endregion

		#region ValueText
		string value_;
		public string ValueText
		{
			get
			{
				return value_;
			}
			private set
			{
				SetValue(ref value_, value);
			}
		}
		#endregion

		protected abstract string ConvertValueToString();

		protected void UpdateValueText()
		{
			this.ValueText = ConvertValueToString();
		}

		protected void UpdateTitle()
		{
			try {
				this.Title = ResourceHolder.Instance.GetString(TitleResourceName);
			}
			catch (Exception ex) {
				TelemetryClient.TrackException("Failed to update the title", ex, new {
					TypeName = this.GetType().FullName
				});
			}
		}

		#region IDisposable メンバ
		bool isDisposed_ = false;
		virtual protected void Dispose(bool disposing)
		{
			if (isDisposed_) { return; }
			if (disposing) {
				Subscriptions.Dispose();
			}
			isDisposed_ = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
