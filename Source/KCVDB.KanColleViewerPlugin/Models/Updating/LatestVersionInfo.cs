using System;
using System.Runtime.Serialization;

namespace KCVDB.KanColleViewerPlugin.Models.Updating
{
	[DataContract]
	public class LatestVersionInfo
	{
		[DataMember(IsRequired = false)]
		public bool IsEmergency { get; set; }

		[DataMember(Name = "name", IsRequired = true)]
		public string VersionString { get; set; }

		public bool IsNewerThan(string currentVersion)
		{
			var latestTokens = VersionString.Trim().Split('.');
			var currentTokens = currentVersion.Trim().Split('.');

			for (int i = 0; i < latestTokens.Length; i++) {
				var latest = int.Parse(latestTokens[i]);
				var current = int.Parse(currentTokens[i]);
				if (latest > current) {
					return true;
				}
			}

			return false;
		}
	}
}
