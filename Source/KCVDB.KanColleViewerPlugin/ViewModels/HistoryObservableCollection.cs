using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCVDB.KanColleViewerPlugin.ViewModels
{
	class HistoryObservableCollection<TItem> : ObservableCollection<TItem>
	{
		public int MaxItemsCount { get; } = 7;

		public void AddItemsToTop(IEnumerable<TItem> items)
		{
			CheckReentrancy();

			foreach (var item in items) {
				Items.Insert(0, item);
			}

			var numItemsToRemove = Items.Count - MaxItemsCount;
			for (int i = 0; i < numItemsToRemove; i++) {
				var index = Items.Count - i - 1;
				Items.RemoveAt(index);
			}

			OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
		}
	}
}
