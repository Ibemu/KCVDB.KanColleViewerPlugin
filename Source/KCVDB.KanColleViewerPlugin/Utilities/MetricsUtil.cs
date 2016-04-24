using System;

namespace KCVDB.KanColleViewerPlugin.Utilities
{
	static class MetricsUtil
	{
		static double Base { get; } = 10.0;
		static double Exponent { get; } = 3.0;
		static string[] Units { get; } = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "YB" };

		public static string FormatSize(long value, int rounding)
		{

			var lastIndex = 0;
			var lastDenom = 1.0;
			for (int i = 0; i < Units.Length; i++) {
				var exponent = Exponent * i;
				var denom = Math.Pow(Base, exponent);

				if (denom > value) { break; }

				lastIndex = i;
				lastDenom = denom;
			}

			var devidedSize = value / lastDenom;
			var format = lastDenom != 1.0
				? $"{{0:F{rounding}}} {Units[lastIndex]}"
				: $"{{0}} {Units[lastIndex]}";
			return string.Format(format, devidedSize);
		}
	}
}
