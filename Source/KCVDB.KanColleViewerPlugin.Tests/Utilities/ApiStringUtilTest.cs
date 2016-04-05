using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCVDB.KanColleViewerPlugin.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KCVDB.KanColleViewerPlugin.Tests.Utilities
{
	[TestClass]
	public class ApiStringUtilTest
	{
		[TestMethod]
		public void RemoveTokenFromRequestBodyTest()
		{
			var input = "api%5Ftoken=b4206b74b7563ec684336f054582605bb3af409b&api%5Fverno=1";
			var expected = "api%5Fverno=1";

			var actual = ApiStringUtil.RemoveTokenFromRequestBody(input);
			Assert.AreEqual(expected, actual);
		}
	}
}
