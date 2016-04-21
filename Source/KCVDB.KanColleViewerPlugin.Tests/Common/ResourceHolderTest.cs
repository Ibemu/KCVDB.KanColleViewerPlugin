using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KCVDB.KanColleViewerPlugin.Tests.Common
{
	[TestClass]
	public class ResourceHolderTest
	{
		[TestMethod]
		public void GetStringTest()
		{
			var expectedEnglishString = "KCVDB Data Sending Plugin";
			var expectedJapaneseString = "KCVDBデータ送信プラグイン";

			ResourceHolder.Instance.Culture = new CultureInfo("en-us");
			var actual = ResourceHolder.Instance.GetString("PluginName");
			Assert.AreEqual(expectedEnglishString, actual);

			ResourceHolder.Instance.Culture = new CultureInfo("ja-jp");
			var actualJapaneseString = ResourceHolder.Instance.GetString("PluginName");
			Assert.AreEqual(expectedJapaneseString, actualJapaneseString);
		}
	}
}
