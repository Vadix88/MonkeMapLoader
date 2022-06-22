using ComputerInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace VmodMonkeMapLoader.Helpers
{
	public static class NumberFormatUtils
	{
		public static string FormatCount(int count)
		{
			return (count >= 1000 ? $"{count / 1000}k" : count.ToString()).PadLeft(4);
		}

		public static string FormatSize(int size, string color = null)
		{
			float mb = size / 1024f / 1024f;
			// Format as 123.1MB
			if (color != null)
			{
				return new StringBuilder().AppendClr($"{mb:F1}".PadLeft(7), color).Append("MB").ToString();
			} else
			{
				return $"{mb:F1}MB".PadLeft(7);
			}
		}
	}
}
