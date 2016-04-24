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

		public string CurrentVersionString => string.Format("{0}.{1}.{2}", CurrentVersion.Major, CurrentVersion.Minor, CurrentVersion.Build);
	}
}
