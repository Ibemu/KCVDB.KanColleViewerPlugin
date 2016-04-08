using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using KCVDB.KanColleViewerPlugin.Models.Database;

namespace KCVDB.KanColleViewerPlugin
{
	[Export(typeof(IPlugin))]
	[ExportMetadata("Title", "KCVDB")]
	[ExportMetadata("Description", "")]
	[ExportMetadata("Version", "0.0.0.1")]
	[ExportMetadata("Author", "KanColle Verification Team")]
	[ExportMetadata("Guid", "005EE84C-80B7-4523-A6F9-5D58D97D27C2")]
	[Export(typeof(ITool))]
	public sealed class KCVDBSenderPlugin : IPlugin, ITool, IDisposable
	{
		CompositeDisposable Disposables { get; } = new CompositeDisposable();
		ApiSender apiSender_;

		public void Initialize()
		{
			Disposables.Add(KanColleClient.Current.Proxy.SessionSource
				.Where(x => x.Request.PathAndQuery == "/netgame/social/-/gadgets/=/app_id=854854/")
				.Subscribe(x => CreateNewSender()));
		}
		
		public void Cleanup()
		{
			Disposables.Dispose();

			apiSender_?.Dispose();
			apiSender_ = null;
		}

		void CreateNewSender()
		{
			if (apiSender_ != null) {
				var senderToBeDisposed = apiSender_;
				senderToBeDisposed.ReadyToBeDisposed += (_, __) => {
					senderToBeDisposed.Dispose();
				};
				senderToBeDisposed.PrepareShutdown();

				apiSender_ = null;
			}

			apiSender_ = new ApiSender();
		}


		#region IDisposable メンバ
		bool isDisposed_ = false;

		public string Name => "KCVDB";

		public object View
		{
			get
			{
				return "unko";
			}
		}

		public void Dispose()
		{
			if (isDisposed_) { return; }

			Cleanup();

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}