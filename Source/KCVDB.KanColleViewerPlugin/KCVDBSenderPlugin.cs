using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
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
		ApiSender apiSender_;

		public void Initialize()
		{
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

			apiSender_?.Dispose();

			isDisposed_ = true;
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}