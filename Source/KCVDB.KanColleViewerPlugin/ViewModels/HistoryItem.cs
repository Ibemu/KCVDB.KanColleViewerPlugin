using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Studiotaiha.Toolkit;

namespace KCVDB.KanColleViewerPlugin.ViewModels
{
	class HistoryItem : BindableBase
	{
		#region TemeSent
		public DateTimeOffset Time
		{
			get
			{
				return GetValue<DateTimeOffset>();
			}
			set
			{
				SetValue(value);
			}
		}
		#endregion
		
		#region Success
		public bool Success
		{
			get
			{
				return GetValue<bool>();
			}
			set
			{
				SetValue(value);
			}
		}
		#endregion

		#region Body
		public string Body
		{
			get
			{
				return GetValue<string>();
			}
			set
			{
				SetValue(value);
			}
		}
		#endregion
	}
}
