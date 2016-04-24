using KCVDB.KanColleViewerPlugin.Properties;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace KCVDB.KanColleViewerPlugin.Telemetry
{
	sealed class TelemetryService
	{
		#region Singleton
		static TelemetryService instance_;
		public static TelemetryService Instance => instance_ ?? (instance_ = new TelemetryService());
		#endregion // Singleton

		public TelemetryClient Client { get; }

		private TelemetryService()
		{
			if (Settings.Default.StopSendingTelemetry) {
				TelemetryConfiguration.Active.DisableTelemetry = true;
			}

			TelemetryConfiguration.Active.InstrumentationKey = Constants.ApplicationInsightInstrumentationKey;
			TelemetryConfiguration.Active.TelemetryInitializers.Add(new TelemetryInitializer());

			Client = new TelemetryClient();
		}
	}
}
