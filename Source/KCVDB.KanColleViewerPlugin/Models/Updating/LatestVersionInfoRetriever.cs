using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace KCVDB.KanColleViewerPlugin.Models.Updating
{
	public class LatestVersionInfoRetriever
	{
		Uri VersionInfoUri { get; }

		public LatestVersionInfoRetriever(Uri versionInfoUri)
		{
			if (versionInfoUri == null) { throw new ArgumentNullException(nameof(versionInfoUri)); }
			VersionInfoUri = versionInfoUri;
		}

		public async Task<LatestVersionInfo> GetLatestVersionInfoAsync()
		{
			var req = (HttpWebRequest)HttpWebRequest.Create(VersionInfoUri);
			req.Method = "GET";

			using (var res = (HttpWebResponse)(await req.GetResponseAsync())) {
				var serializer = new DataContractJsonSerializer(typeof(LatestVersionInfo));
				return (LatestVersionInfo)serializer.ReadObject(res.GetResponseStream());
			}
		}
	}
}
