using System;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Telemetry;
using KCVDB.KanColleViewerPlugin.Utilities;
using Microsoft.ApplicationInsights;
using Nekoxy;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.Models.Database
{
	internal sealed class ApiSender : IDisposable
	{
		static string DateHeaderKey { get; } = "date";

		TelemetryClient TelemetryClient { get; } = TelemetryService.Instance.Client;
		CompositeDisposable Disposable { get; } = new CompositeDisposable();
		public IKCVDBClient KcvdbClient { get; }

		bool shouldStopSending_;
		
		public ApiSender(string sessionId)
		{
			Disposable.Add(KanColleClient.Current.Proxy.ApiSessionSource
				.Subscribe(OnSession));

			KcvdbClient = KCVDBClientService.Instance.CreateClient(Constants.KCVDB.AgentId, sessionId);
			Disposable.Add(KcvdbClient);

			KcvdbClient.FatalError += KcvdbClient_FatalError;
			KcvdbClient.InternalError += KcvdbClient_InternalError;
			KcvdbClient.SendingError += KcvdbClient_SendingError;
		}

		private void KcvdbClient_SendingError(object sender, SendingErrorEventArgs e)
		{
			try {
				TelemetryClient.TrackException("Sending error catched.", e.Exception, new {
					ErrorMessage = e.Message,
					Reason = e.Reason,
				});
			}
			catch (Exception ex) {
				if (ex.IsCritical()) { throw; }
			}
		}

		private void KcvdbClient_InternalError(object sender, InternalErrorEventArgs e)
		{
			try {
				TelemetryClient.TrackException("KCVDB internal error catched", e.Exception, new {
					ErrorMessage = e.Message,
				});
			}
			catch (Exception ex) {
				if (ex.IsCritical()) { throw; }
			}
		}

		private void KcvdbClient_FatalError(object sender, FatalErrorEventArgs e)
		{
			try {
				shouldStopSending_ = true;

				TelemetryClient.TrackException("KCVDB Fatal error catched", e.Exception, new {
					ErrorMessage = e.Message,
				});
			}
			catch (Exception ex) {
				if (ex.IsCritical()) { throw; }
			}
		}

		void OnSession(Session session)
		{
			try {
				if (shouldStopSending_) { return; }
				
				var requestUri = Uri.IsWellFormedUriString(session.Request.RequestLine.URI, UriKind.Absolute)
				? new Uri(session.Request.RequestLine.URI, UriKind.Absolute)
				: new Uri(string.Format("http://{0}{1}", session.Request.Headers.Host, session.Request.PathAndQuery), UriKind.Absolute);
				var statusCode = session.Response.StatusLine.StatusCode;
				var requestBody = ApiStringUtil.RemoveTokenFromRequestBody(session.Request.BodyAsString);
				var responseBody = session.Response.BodyAsString;
				string httpDate;
				session.Response.Headers.Headers.TryGetValue(DateHeaderKey, out httpDate);

				KcvdbClient.SendRequestDataAsync(
					requestUri,
					statusCode,
					requestBody,
					responseBody,
					httpDate);
			}
			catch (Exception ex) {
				TelemetryClient.TrackException("Failed to enqueue session data", ex);
				if (ex.IsCritical()) { throw; }
			}
		}

		#region IDisposable メンバ
		bool isDisposed_;
		public void Dispose()
		{
			if (isDisposed_) { return; }
			Disposable.Dispose();
			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
