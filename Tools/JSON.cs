using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GroupCheck.Tools
{
	public static class JSON
	{
		public static string Serialize(object input)
		{
			return JsonSerializer.Serialize(input);
		}
		public static T Deserialize<T>(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var result = JsonSerializer.Deserialize<T>(input);
			return result;
		}
	}
}
