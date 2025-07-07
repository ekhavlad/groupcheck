using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using GroupCheck.Tools.Extensions.RuntimeExt;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorage, IServerStorageAccounts, IServerStorageGroups, IServerStorageMembers, IServerStorageChecks
	{
		private static Dictionary<string, HashSet<string>> _sqlParameters = new Dictionary<string, HashSet<string>>();
		private static HashSet<string> GetRequestParameters(string sql)
		{
			if (!_sqlParameters.ContainsKey(sql))
			{
				lock (_sqlParameters)
				{
					if (!_sqlParameters.ContainsKey(sql))
					{
						_sqlParameters[sql] = new HashSet<string>();
						var isParam = false;
						var tmpParam = "";
						for (int i = 0; i < sql.Length; i++)
						{
							if (!isParam && sql[i] == '@') isParam = true;
							else if (isParam && char.IsLetterOrDigit(sql[i])) tmpParam += sql[i];
							else if (isParam) { _sqlParameters[sql].Add(tmpParam); tmpParam = ""; isParam = false; }
						}
						if (tmpParam != "") _sqlParameters[sql].Add(tmpParam);
					}
				}
			}

			return _sqlParameters[sql];
		}

		private static SqlCommand PrepareSQL(string sql, object model)
		{
			var requestParameters = GetRequestParameters(sql);
			var cmd = new SqlCommand(sql);

			if (!requestParameters.Any())
				return cmd;

			if (requestParameters.Count == 1)
			{
				cmd.Parameters.AddWithValue("@" + requestParameters.Single(), model == null ? DBNull.Value : model);
				return cmd;
			}

			if (model == null)
				throw new Exception("An empty model is given for preparing SQL request with parameters.");

			var getters = model.GetType().GetPropertyGetters();
			foreach (var par in requestParameters)
			{
				object val;
				if (getters.ContainsKey("DB_" + par))
					val = getters["DB_" + par](model);
				else
					val = getters[par](model);
				cmd.Parameters.AddWithValue("@" + par, val == null ? DBNull.Value : val);
			}

			return cmd;
		}
		private static T ParseRow<T>(DataRow row) where T : class, new()
		{
			if (row == null) return (T)null;

			var setters = typeof(T).GetPropertySetters();
			var result = new T();
			foreach (var p in setters.Where(p => row.Table.Columns.Contains(p.Key)))
			{
				var value = (row[p.Key] == DBNull.Value) ? null : row[p.Key];
				if (p.Key == "Revision")
					p.Value(result, ParseRevision((byte[])value));
				else if (setters.Keys.Contains("DB_" + p.Key))
					setters["DB_" + p.Key](result, value);
				else
					p.Value(result, value);
			}

			return result;
		}

		private static long ParseRevision(byte[] timestamp)
		{
			if (timestamp == null || timestamp.Length == 0) return 0;
			return BitConverter.ToInt64(timestamp.Reverse().ToArray(), 0);
		}
	}
}
