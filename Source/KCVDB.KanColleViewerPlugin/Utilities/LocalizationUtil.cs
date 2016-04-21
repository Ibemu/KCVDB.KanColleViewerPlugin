using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace KCVDB.KanColleViewerPlugin.Utilities
{
	static class LocalizationUtil
	{
		public static CultureInfo GetCurrentAppCulture()
		{
			var resourceServiceType = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(x => x.GetName().Name == "KanColleViewer")
				?.GetTypes()
				?.FirstOrDefault(x => x.FullName == "Grabacr07.KanColleViewer.Models.ResourceService");

			var currentProperty = resourceServiceType?.GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
			var resourceService = currentProperty?.GetValue(resourceServiceType);
			
			var resourcesProperty = resourceServiceType?.GetProperty("Resources");
			var resources = resourcesProperty?.GetValue(resourceService);

			var cultureProperty = resources?.GetType()?.GetProperty("Culture");
			return cultureProperty?.GetValue(resources) as CultureInfo;
		}
	}
}
