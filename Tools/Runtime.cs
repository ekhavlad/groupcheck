using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace GroupCheck.Tools
{
	public static class Runtime
	{
		/// <summary>
		/// Returns the full name (with namespace) of the currently executing method.
		/// </summary>
		/// <returns></returns>
		public static string GetMethodFullName()
		{
			var st = new System.Diagnostics.StackTrace();
			var sf = st.GetFrame(1);
			var method = sf.GetMethod();
			var result = method.DeclaringType.FullName + "." + method.Name;
			return result;
		}

		/// <summary>
		/// Creates object of the same type and copy all public properties.
		/// </summary>
		public static T CreateCopy<T>(T source) where T : new()
		{
			if (source == null)
				return default(T);
			var getters = GetPropertyGetters(typeof(T));
			var setters = GetPropertySetters(typeof(T));

			var result = new T();
			foreach (var s in setters.Where(_ => getters.ContainsKey(_.Key)))
			{
				s.Value(result, getters[s.Key](source));
			}
			return result;
		}

		/// <summary>
		/// Creates new object and fill all the corresponded properties, that exist in source object.
		/// </summary>
		/// <typeparam name="T">Type of object to be created.</typeparam>
		/// <param name="original">Source object</param>
		/// <param name="properties">The list of propertiesto be  copied. Is Null, copy all properties for the destination object.</param>
		/// <returns></returns>
		public static T ConvertTo<T>(object source, IEnumerable<string> properties = null) where T : new()
		{
			var getters = GetPropertyGetters(source.GetType());
			var setters = GetPropertySetters(typeof(T));

			var result = new T();
			foreach (var s in setters.Where(_ => (properties == null || properties.Contains(_.Key)) && getters.ContainsKey(_.Key)))
			{
				s.Value(result, getters[s.Key](source));
			}

			return result;
		}


		private static Dictionary<Type, Dictionary<string, Func<object, object>>> _getters = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
		public static Dictionary<string, Func<object, object>> GetPropertyGetters(Type type)
		{
			if (!_getters.ContainsKey(type))
			{
				lock (_getters)
				{
					if (!_getters.ContainsKey(type))
					{
						var currentGetters = new Dictionary<string, Func<object, object>>();
						foreach (var prop in type.GetProperties().Where(_ => _.CanRead))
						{
							currentGetters[prop.Name] = BuildGetAccessor(prop);
						}
						_getters[type] = currentGetters;
					}
				}
			}

			return _getters[type];
		}

		private static Dictionary<Type, Dictionary<string, Action<object, object>>> _setters = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
		public static Dictionary<string, Action<object, object>> GetPropertySetters(Type type)
		{
			if (!_setters.ContainsKey(type))
			{
				lock (_setters)
				{
					if (!_setters.ContainsKey(type))
					{
						var currentSetters = new Dictionary<string, Action<object, object>>();
						foreach (var prop in type.GetProperties().Where(_ => _.CanWrite))
						{
							currentSetters[prop.Name] = BuildSetAccessor(prop);
						}
						_setters[type] = currentSetters;
					}
				}
			}

			return _setters[type];
		}

		private static Func<object, object> BuildGetAccessor(PropertyInfo prop)
		{
			var obj = Expression.Parameter(typeof(object), "obj");
			var castedObj = prop.DeclaringType.IsValueType ? Expression.Convert(obj, prop.DeclaringType) : Expression.TypeAs(obj, prop.DeclaringType);

			Func<object, object> GetDelegate =
				Expression.Lambda<Func<object, object>>(
					Expression.TypeAs(Expression.Call(castedObj, prop.GetGetMethod()), typeof(object)),
					obj)
				.Compile();

			return GetDelegate;
		}
		private static Action<object, object> BuildSetAccessor(PropertyInfo prop)
		{
			var method = prop.GetSetMethod();
			var valType = method.GetParameters().First().ParameterType;

			var obj = Expression.Parameter(typeof(object), "obj");
			var castedObj = method.DeclaringType.IsValueType ? Expression.Convert(obj, method.DeclaringType) : Expression.TypeAs(obj, method.DeclaringType);

			var val = Expression.Parameter(typeof(object), "val");
			var castedVal = valType.IsValueType ? Expression.Convert(val, valType) : Expression.TypeAs(val, valType);

			var setDelegate = Expression.Lambda<Action<object, object>>(
					Expression.Call(castedObj, prop.GetSetMethod(), castedVal),
					obj, val)
				.Compile();

			return setDelegate;
		}
	}
}
