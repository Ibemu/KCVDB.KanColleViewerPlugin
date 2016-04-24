using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace KCVDB.KanColleViewerPlugin.Telemetry
{
	static class TelemetryClientExtensions
	{
		public static void TrackException(
			this TelemetryClient client,
			string message,
			Exception exception)
		{
			client.TrackException<object>(message, exception, null);
		}

		public static void TrackException<TProperties>(
			this TelemetryClient client,
			string message,
			Exception exception,
			TProperties property)
			where TProperties : class
		{
			var telemetry = new ExceptionTelemetry(exception);
			telemetry.Properties["Message"] = message;

			if (property != null) {
				var properties = property.GetType().GetProperties();
				foreach (var prop in properties) {
					var name = prop.Name;
					var value = prop.GetValue(property);

					telemetry.Properties[name] = value?.ToString() ?? "null";
				}
			}

			client.TrackException(telemetry);
		}

		public static void TrackEvent<TProperties>(
			this TelemetryClient client,
			string eventName,
			TProperties property)
			where TProperties : class
		{
			var telemetry = new EventTelemetry(eventName);

			if (property != null) {
				var properties = property.GetType().GetProperties();
				foreach (var prop in properties) {
					var name = prop.Name;
					var value = prop.GetValue(property);

					telemetry.Properties[name] = value?.ToString() ?? "null";
				}
			}

			client.TrackEvent(telemetry);
		}
	}
}
