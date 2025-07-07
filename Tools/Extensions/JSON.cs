using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools.Extensions.JSONExt
{
	public static class JSONExtension
	{
		public static string ToJSON(this object input)
		{
			return JSON.Serialize(input);
		}
		public static T ParseJSON<T>(this string input)
		{
			return JSON.Deserialize<T>(input);
		}
	}
}
