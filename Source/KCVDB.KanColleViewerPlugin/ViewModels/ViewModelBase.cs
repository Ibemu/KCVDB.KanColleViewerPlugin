using System;
using KCVDB.KanColleViewerPlugin.Telemetry;
using KCVDB.KanColleViewerPlugin.Utilities;
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

		#region Commands

		#region OpenUrlCommand
		DelegateCommand openUrlCommand_ = null;
		public DelegateCommand OpenUrlCommand
		{
			get
			{
				return openUrlCommand_ ?? (openUrlCommand_ = new DelegateCommand {
					ExecuteHandler = param => {
						try {
							var uri = param is Uri
								? (Uri)param
								: param is string
									? new Uri((string)param)
									: null;
							if (uri != null) {
								WebUtil.OpenUri(uri);
							}
						}
						catch (Exception ex) {
							TelemetryClient.TrackException("Failed to open URI", ex, new {
								Param = param,
							});
						}			
					}
				});
			}
		}
		#endregion

		#endregion // Commands
	}
}
