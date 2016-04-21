using System;
using System.Reflection;

namespace KCVDB.KanColleViewerPlugin
{
	sealed class PluginInfo
	{
		#region Singleton

		static PluginInfo instance_;
		public static PluginInfo Instance => instance_ ?? (instance_ = new PluginInfo());

		#endregion // Singleton

		private PluginInfo()
		{ }

		public Version CurrentVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version;
	}
}
