using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace KCVDB.KanColleViewerPlugin.Telemetry
{
	class TelemetryInitializer : ITelemetryInitializer
	{
		string SessionId { get; } = Guid.NewGuid().ToString();
		string Version { get; } = PluginInfo.Instance.CurrentVersion.ToString();
		
		public void Initialize(ITelemetry telemetry)
		{
			telemetry.Context.Component.Version = Version;
			telemetry.Context.Session.Id = SessionId;
			telemetry.Context.Device.OperatingSystem = string.Format("{0} {1}",
				Environment.OSVersion.VersionString,
				Environment.Is64BitOperatingSystem ? "x64" : "x86");
		}
	}
}
