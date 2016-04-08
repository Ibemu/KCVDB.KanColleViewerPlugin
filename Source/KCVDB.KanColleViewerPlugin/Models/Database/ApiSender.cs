using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Grabacr07.KanColleWrapper;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Utilities;
using Nekoxy;

namespace KCVDB.KanColleViewerPlugin.Models.Database
{
	internal sealed class ApiSender : IDisposable
	{
		static string DateHeaderKey { get; } = "date";

		CompositeDisposable Subscriptions { get; } = new CompositeDisposable();
		IKCVDBClient Client { get; }

		int queueLength_;
		bool isShuttingDown_;

		public ApiSender()
		{
			HttpProxy.AfterReadRequestHeaders += HttpProxy_AfterReadRequestHeaders;
			HttpProxy.AfterReadResponseHeaders += HttpProxy_AfterReadResponseHeaders;
			Subscriptions.Add(KanColleClient.Current.Proxy.ApiSessionSource
				.Subscribe(OnSession));

			Client = KCVDBClientService.Instance.CreateClient(Constants.KCVDB.AgentId);

			Client.ApiDataSent += Client_ApiDataSent;
		}

		private void Client_ApiDataSent(object sender, ApiDataSentEventArgs e)
		{
			queueLength_--;
			if (isShuttingDown_ && queueLength_ == 0) {
				this.ReadyToBeDisposed?.Invoke(this, EventArgs.Empty);
			}
		}

		private void HttpProxy_AfterReadResponseHeaders(HttpResponse obj)
		{
			Console.WriteLine(obj);
		}

		private void HttpProxy_AfterReadRequestHeaders(HttpRequest obj)
		{
			Console.WriteLine(obj);
		}

		void OnSession(Session session)
		{
			try {
				var requestUri = Uri.IsWellFormedUriString(session.Request.RequestLine.URI, UriKind.Absolute)
				? new Uri(session.Request.RequestLine.URI, UriKind.Absolute)
				: new Uri(string.Format("http://{0}{1}", session.Request.Headers.Host, session.Request.PathAndQuery), UriKind.Absolute);
				var statusCode = session.Response.StatusLine.StatusCode;
				var requestBody = ApiStringUtil.RemoveTokenFromRequestBody(session.Request.BodyAsString);
				var responseBody = session.Response.BodyAsString;
				string httpDate;
				session.Response.Headers.Headers.TryGetValue(DateHeaderKey, out httpDate);

				Client.SendRequestDataAsync(
					requestUri,
					statusCode,
					requestBody,
					responseBody,
					httpDate);
				queueLength_++;
			}
			catch (Exception ex) {
				Console.WriteLine(ex);
			}
		}

		public void PrepareShutdown()
		{
			isShuttingDown_ = true;

			Subscriptions.Dispose();
			if (queueLength_ == 0) {
				this.ReadyToBeDisposed?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler ReadyToBeDisposed;

		#region IDisposable メンバ
		bool isDisposed_;
		public void Dispose()
		{
			if (isDisposed_) { return; }

			if (!Subscriptions.IsDisposed) {
				Subscriptions.Dispose();
			}
			Client.Dispose();

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
