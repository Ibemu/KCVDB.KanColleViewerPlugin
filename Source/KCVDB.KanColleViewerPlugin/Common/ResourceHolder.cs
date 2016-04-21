using System.Globalization;
using System.Resources;
using KCVDB.KanColleViewerPlugin.Properties;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin
{
	sealed class ResourceHolder : NotificationObject
	{
		#region Singleton
		static ResourceHolder instance_;
		public static ResourceHolder Instance => instance_ ?? (instance_ = new ResourceHolder());
		#endregion // Singleton

		public Resources Resources { get; } = new Resources();
		public ResourceManager ResourceManager => Resources.ResourceManager;

		public CultureInfo Culture
		{
			get
			{
				return Resources.Culture;
			}
			set
			{
				if (Resources.Culture != value) {
					Resources.Culture = value;
					RaisePropertyChanged();
					RaisePropertyChanged(nameof(Resources));
				}
			}
		}

		private ResourceHolder()
		{ }

		public string GetString(string resourceId)
		{
			return Culture != null
				? ResourceManager.GetString(resourceId, Culture)
				: ResourceManager.GetString(resourceId);
		}
	}
}
