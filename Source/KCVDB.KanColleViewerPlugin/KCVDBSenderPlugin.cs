using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Grabacr07.KanColleViewer.Composition;
using KCVDB.KanColleViewerPlugin.Models.Database;
using KCVDB.KanColleViewerPlugin.Models.Updating;
using KCVDB.KanColleViewerPlugin.Properties;
using KCVDB.KanColleViewerPlugin.Telemetry;
using KCVDB.KanColleViewerPlugin.Utilities;
using KCVDB.KanColleViewerPlugin.ViewModels;
using KCVDB.KanColleViewerPlugin.Views;
using Microsoft.ApplicationInsights;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[Export(typeof(ILocalizable))]
	[ExportMetadata("Title", "KCVDBデータ送信プラグイン")]
	[ExportMetadata("Description", "KCVDBにデータを送信するプラグインです。")]
	[ExportMetadata("Version", Constants.ApplicationVersion)]
	[ExportMetadata("Author", "艦これ検証部")]
	[ExportMetadata("Guid", "005EE84C-80B7-4523-A6F9-5D58D97D27C2")]
	public sealed class KCVDBSenderPlugin : IPlugin, ITool, ILocalizable, IDisposable
	{
		TelemetryClient TelemetryClient { get; } = TelemetryService.Instance.Client;
		ApiSender apiSender_;
		ToolViewViewModel viewModel_;

		public async void Initialize()
		{
			TaihaToolkit.RegisterComponents(WPFComponent.Instance);

			TelemetryClient.TrackEvent("PluginLoaded");

			// Obtain default app culture
			try {
				ResourceHolder.Instance.Culture = LocalizationUtil.GetCurrentAppCulture();
				UpdateChineseCulture();
			}
			catch (Exception ex) {
				TelemetryClient.TrackException("Failed to get default app culture.", ex);
			}

			// Initialize KCVDB client
			try {
				var sessionId = Guid.NewGuid().ToString();
				apiSender_ = new ApiSender(sessionId);
				viewModel_ = new ToolViewViewModel(
					apiSender_.KcvdbClient,
					sessionId,
					new WPFDispatcher(Dispatcher.CurrentDispatcher));
			}
			catch (Exception ex) {
				TelemetryClient.TrackException("Failed to initialize KCVDB client.", ex);
			}

			Settings.Default.PropertyChanged += Settings_PropertyChanged;

			TelemetryClient.TrackEvent("PluginInitialized");
			
			try {
				await CheckForUpdate();
			}
			catch (Exception ex) {
				TelemetryClient.TrackException("Failed to check the latest version.", ex);
				if (ex.IsCritical()) { throw; }
			}
		}

		async Task CheckForUpdate()
		{
			var versionInfoUrl = ResourceHolder.Instance.GetString("VersionInfoUrl");
			var latestVersion = await new LatestVersionInfoRetriever(new Uri(versionInfoUrl))
				.GetLatestVersionInfoAsync();

			if(latestVersion.IsNewerThan(PluginInfo.Instance.CurrentVersionString)) {
				var caption = "提督業も忙しい！ - " +  ResourceHolder.Instance.GetString("PluginName");
				var message = string.Format(
					latestVersion.IsEmergency ? "{0}\n{1}" : "{1}",
					ResourceHolder.Instance.GetString("NewerVersionAvailableMessageEmergencyPrefix"),
					ResourceHolder.Instance.GetString("NewerVersionAvailableMessage"));

				var app = LocalizationUtil.GetApplication();
				var ret = app?.MainWindow != null
					? MessageBox.Show(app.MainWindow, message, caption, MessageBoxButton.YesNo, MessageBoxImage.Information)
					: MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Information);

				if (ret == MessageBoxResult.Yes) {
					var url = ResourceHolder.Instance.GetString("KcvdbPluginWebUrl");
					WebUtil.OpenUri(new Uri(url));
				}
			}
		}

		private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Settings.Default.ShowTraditionalChinese)) {
				UpdateChineseCulture();
			}
		}

		void UpdateChineseCulture()
		{
			if (ResourceHolder.Instance.Culture?.TwoLetterISOLanguageName == "zh") {
				try {
					ResourceHolder.Instance.Culture = Settings.Default.ShowTraditionalChinese
						? new CultureInfo("zh-TW")
						: new CultureInfo("zh-CN");
				}
				catch (Exception ex) {
					TelemetryClient.TrackException("Failed to update chinese culture", ex);
				}
			}
		}

		public string Name => Resources.ToolPluginName;

		public object View
		{
			get
			{
				return new ToolViewControl {
					DataContext = viewModel_
				};
			}
		}

		public void ChangeCulture(string cultureName)
		{
			TelemetryClient.TrackEvent("CultureChanged", new {
				CultureName = cultureName
			});

			if (cultureName == null) {
				ResourceHolder.Instance.Culture = null;
			}
			else {
				ResourceHolder.Instance.Culture = CultureInfo.GetCultureInfo(cultureName);
			}

			UpdateChineseCulture();
		}
		
		#region IDisposable Member

		bool isDisposed_ = false;
		public void Dispose()
		{
			if (isDisposed_) { return; }
			
			this.TelemetryClient.TrackEvent("PluginDisposed.");
			this.TelemetryClient.Flush();

			apiSender_?.Dispose();
			viewModel_?.Dispose();
			Settings.Default.PropertyChanged -= Settings_PropertyChanged;

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}