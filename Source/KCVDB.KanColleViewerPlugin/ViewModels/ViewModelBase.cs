using KCVDB.KanColleViewerPlugin.Telemetry;
using Microsoft.ApplicationInsights;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels
{
	class ViewModelBase : NotificationObjectWithPropertyBag
	{
		protected TelemetryClient TelemetryClient => TelemetryService.Instance.Client;

		public ViewModelBase(IDispatcher dispatcher)
			: base(dispatcher)
		{ }
	}
}
