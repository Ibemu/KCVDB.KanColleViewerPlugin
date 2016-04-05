using System;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using KCVDB.Client;
using KCVDB.KanColleViewerPlugin.Utilities;
using Nekoxy;

namespace KCVDB.KanColleViewerPlugin.Models.Database
{
	internal sealed class ApiSender : IDisposable
	{
		static string DateHeaderKey { get; } = "date";

		IKCVDBClient client_;
		
		CompositeDisposable Disposable { get; } = new CompositeDisposable();


		public ApiSender()
		{
			HttpProxy.AfterReadRequestHeaders += HttpProxy_AfterReadRequestHeaders;
			HttpProxy.AfterReadResponseHeaders += HttpProxy_AfterReadResponseHeaders;
			Disposable.Add(KanColleClient.Current.Proxy.ApiSessionSource
				.Subscribe(OnSession));

			client_ = KCVDBClientService.Instance.CreateClient(Constants.KCVDB.AgentId);
			Disposable.Add(client_);
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

				client_.SendRequestDataAsync(
					requestUri,
					statusCode,
					requestBody,
					responseBody,
					httpDate);
			}
			catch (Exception ex) {
				Console.WriteLine(ex);
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
