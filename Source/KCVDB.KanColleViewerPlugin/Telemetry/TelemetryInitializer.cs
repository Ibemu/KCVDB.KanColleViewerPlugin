using System;
using System.Linq;
using System.Management;
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
			telemetry.Context.Device.Language = ResourceHolder.Instance.Culture?.Name ?? "auto";
			try {
				telemetry.Context.User.Id = new ManagementClass("win32_processor")
					.GetInstances()
					.Cast<ManagementObject>()
					.Select(x => x.Properties["processorID"]?.Value?.ToString())
					.FirstOrDefault(x => !string.IsNullOrEmpty(x));
			}
			catch {
				telemetry.Context.User.Id = "Unknown";
			}
		}
	}
}
