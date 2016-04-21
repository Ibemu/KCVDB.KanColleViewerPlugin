using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KCVDB.KanColleViewerPlugin.Utilities
{
	public static class DependencyObjectExtensions
	{
		static IEnumerable<DependencyObject> Children(this DependencyObject obj)
		{
			if (obj == null) { throw new ArgumentNullException(nameof(obj)); }

			var count = VisualTreeHelper.GetChildrenCount(obj);
			if (count == 0) {
				yield break;
			}

			for (int i = 0; i < count; i++) {
				var child = VisualTreeHelper.GetChild(obj, i);
				if (child != null) {
					yield return child;
				}
			}
		}
		
		public static IEnumerable<DependencyObject> Descendants(this DependencyObject obj)
		{
			if (obj == null) { throw new ArgumentNullException(nameof(obj)); }

			foreach (var child in obj.Children()) {
				yield return child;
				foreach (var grandChild in child.Descendants()) {
					yield return grandChild;
				}
			}
		}
	}
}
