using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCVDB.KanColleViewerPlugin.ViewModels
{
	interface IMetrics : INotifyPropertyChanged
	{
		string Title { get; }
		string ValueText { get; }
	}
}
