using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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


		public static Application GetApplication()
		{
			var applicationType = AppDomain.CurrentDomain.GetAssemblies()
				.FirstOrDefault(x => x.GetName().Name == "KanColleViewer")
				?.GetTypes()
				?.FirstOrDefault(x => x.FullName == "Grabacr07.KanColleViewer.Application");

			var instanceProperty = applicationType?.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
			return instanceProperty?.GetValue(applicationType) as Application;
		}

		public static IDisposable StartMisakura()
		{
			return Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1))
				.Subscribe(_ => {
					var application = GetApplication();
					application.Dispatcher.BeginInvoke((Action)(() => {
						foreach (var window in application.Windows.Cast<Window>().ToArray()) {
							window.Title = MisakuraConverter.Convert(window.Title);
							window
								.Descendants()
								.ToList()
								.ForEach(x => Convert(x));
						}
					}));
				});
		}

		static Dictionary<object, bool> MisakuraedControls = new Dictionary<object, bool>();

		public static void Convert(object content)
		{
			if (MisakuraedControls.ContainsKey(content)) {
				return;
			}

			if (content is TextBlock) {
				var textBlock = (TextBlock)content;
				if (textBlock.Inlines.Any()) {
					foreach (var inline in textBlock.Inlines.ToArray()) {
						Convert(inline);
					}
				}
				else {
					textBlock.Text = MisakuraConverter.Convert(textBlock.Text);
				}
			}
			else if (content is Run) {
				var run = (Run)content;
				run.Text = MisakuraConverter.Convert(run.Text);
			}
			else if (content is Hyperlink) {
				var hyperlink = (Hyperlink)content;
				foreach (var inline in hyperlink.Inlines.ToArray()) {
					Convert(inline);
				}
			}
			else {
				return;
			}

			MisakuraedControls[content] = true;
		}
	}
}
