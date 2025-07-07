using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupCheck.Tools.Extensions.RuntimeExt
{
	public static class RuntimeExtensions
	{
		/// <summary>
		/// Creates object of the same type and copy all public properties.
		/// </summary>
		public static T CreateCopy<T>(this T source) where T : new()
		{
			return Runtime.CreateCopy(source);
		}

		/// <summary>
		/// Creates new object and fill all the corresponded properties, that exist in source object.
		/// </summary>
		/// <typeparam name="T">Type of object to be created.</typeparam>
		/// <param name="original">Source object</param>
		/// <param name="properties">The list of propertiesto be  copied. Is Null, copy all properties for the destination object.</param>
		/// <returns></returns>
		public static T ConvertTo<T>(this object original, IEnumerable<string> properties = null) where T : new()
		{
			return Runtime.ConvertTo<T>(original, properties);
		}

		public static Dictionary<string, Func<object, object>> GetPropertyGetters(this Type type)
		{
			return Runtime.GetPropertyGetters(type);
		}

		public static Dictionary<string, Action<object, object>> GetPropertySetters(this Type type)
		{
			return Runtime.GetPropertySetters(type);
		}
	}
}
