using System.Linq;
using System.Net;

namespace KCVDB.KanColleViewerPlugin.Utilities
{
	static class ApiStringUtil
	{
		public static string RequestStringTokenParameterKey { get; } = "api_token";

		public static string RemoveTokenFromRequestBody(string urlEncodedStr)
			=> string.Join(
				"&",
				urlEncodedStr
				.Split('&')
				.Select(token => token
					.Split('=')
					.Select(x => WebUtility.UrlDecode(x))
					.ToArray())
				.Where(x => x.Length >= 2 && x[0] != RequestStringTokenParameterKey)
				.Select(x => x
					.Select(y => WebUtility.HtmlEncode(y))
					.Select(y => y.Replace("_", "%5F"))
					.ToArray())
				.Select(x => x[0] + "=" + x[1])
			);
	}
}
