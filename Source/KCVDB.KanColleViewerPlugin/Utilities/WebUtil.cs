using System;
using System.Diagnostics;

namespace KCVDB.KanColleViewerPlugin.Utilities
{
	static class WebUtil
	{
		public static void OpenUri(Uri uri)
		{
			Process.Start(uri.ToString());
		}
	}
}
