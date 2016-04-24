using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Threading;
using Grabacr07.KanColleViewer.Composition;
using KCVDB.KanColleViewerPlugin.Models.Database;
using KCVDB.KanColleViewerPlugin.Properties;
using KCVDB.KanColleViewerPlugin.Utilities;
using KCVDB.KanColleViewerPlugin.ViewModels;
using KCVDB.KanColleViewerPlugin.Views;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[Export(typeof(ILocalizable))]
	[ExportMetadata("Title", "KCVDBデータ送信プラグイン")]
	[ExportMetadata("Description", "KCVDBにデータを送信するプラグインです。")]
	[ExportMetadata("Version", "1.0.0")]
	[ExportMetadata("Author", "艦これ検証部")]
	[ExportMetadata("Guid", "005EE84C-80B7-4523-A6F9-5D58D97D27C2")]
	public sealed class KCVDBSenderPlugin : IPlugin, ITool, ILocalizable, IDisposable
	{
		ApiSender apiSender_;
		ToolViewViewModel viewModel_;

		public void Initialize()
		{
			ResourceHolder.Instance.Culture = LocalizationUtil.GetCurrentAppCulture();
			UpdateChineseCulture();

			var sessionId = Guid.NewGuid().ToString();
			apiSender_ = new ApiSender(sessionId);
			viewModel_ = new ToolViewViewModel(
				apiSender_.Client,
				sessionId,
				new WPFDispatcher(Dispatcher.CurrentDispatcher));

			Settings.Default.PropertyChanged += Settings_PropertyChanged;
		}

		private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			UpdateChineseCulture();
		}

		void UpdateChineseCulture()
		{
			if (ResourceHolder.Instance.Culture?.TwoLetterISOLanguageName == "zh") {
				ResourceHolder.Instance.Culture = Settings.Default.ShowTraditionalChinese
					? new CultureInfo("zh-TW")
					: new CultureInfo("zh-CN");
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

			apiSender_?.Dispose();
			viewModel_?.Dispose();
			Settings.Default.PropertyChanged -= Settings_PropertyChanged;

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}