using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorage, IServerStorageAccounts, IServerStorageGroups, IServerStorageMembers, IServerStorageChecks
	{
		private readonly string _connectionString;
		private readonly string _host;
		private readonly string _user;
		private readonly string _pass;
		private readonly string _db;

		public MSSQLStorage(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new Exception("Connection string cannot be empty.");

			var pars = connectionString.Split(';').ToDictionary(p => p.Split('=')[0].Trim().ToLower(), p => { var tmp = p.Split('='); return tmp.Length > 1 ? tmp[1] : null; });
			pars.TryGetValue("data source", out _host);
			pars.TryGetValue("user id", out _user);
			pars.TryGetValue("password", out _pass);
			pars.TryGetValue("initial catalog", out _db);
			_connectionString = connectionString;
		}
		public MSSQLStorage(string host, string user, string pass, string db)
		{
			_host = host;
			_user = user;
			_pass = pass;
			_db = db;
			_connectionString = $"data source={_host};initial catalog={_db};persist security info=True;user id={_user};password={_pass};";
		}

		public IServerStorageAccounts Accounts { get { return this; } }
		public IServerStorageGroups Groups { get { return this; } }
		public IServerStorageMembers Members { get { return this; } }
		public IServerStorageChecks Checks { get { return this; } }

		private SqlConnection GetConnection()
		{
			return new SqlConnection(_connectionString);
		}

		public int Execute(SqlCommand cmd)
		{
			using (SqlConnection conn = GetConnection())
			{
				cmd.Connection = conn;
				using (cmd)
				{
					conn.Open();
					var rowsAffected = cmd.ExecuteNonQuery();
					return rowsAffected;
				}
			}
		}
		protected int Execute(string sql)
		{
			var cmd = new SqlCommand(sql);
			return Execute(cmd);
		}
		protected int Execute(string sql, object model)
		{
			var cmd = PrepareSQL(sql, model);
			return Execute(cmd);
		}

		public DataRow GetDataRow(SqlCommand cmd)
		{
			if (cmd.Connection == null)
			{
				using (SqlConnection conn = GetConnection())
				{
					cmd.Connection = conn;
					conn.Open();
					using (cmd)
					{
						using (var adapter = new SqlDataAdapter(cmd))
						{
							var table = new DataTable();
							adapter.Fill(table);
							if (table.Rows.Count == 0) return null;
							return table.Rows[0];
						}
					}
				}
			}
			else
			{
				using (cmd)
				{
					using (var adapter = new SqlDataAdapter(cmd))
					{
						var table = new DataTable();
						adapter.Fill(table);
						if (table.Rows.Count == 0) return null;
						return table.Rows[0];
					}
				}
			}
		}
		public DataTable GetTable(SqlCommand cmd)
		{
			if (cmd.Connection == null)
			{
				using (SqlConnection conn = GetConnection())
				{
					cmd.Connection = conn;
					conn.Open();
					using (cmd)
					{
						using (var adapter = new SqlDataAdapter(cmd))
						{
							var table = new DataTable();
							adapter.Fill(table);
							return table;
						}
					}
				}
			}
			else
			{
				using (cmd)
				{
					using (var adapter = new SqlDataAdapter(cmd))
					{
						var table = new DataTable();
						adapter.Fill(table);
						return table;
					}
				}
			}
		}
		public DataSet GetDataSet(SqlCommand cmd)
		{
			if (cmd.Connection == null)
			{
				using (SqlConnection conn = GetConnection())
				{
					cmd.Connection = conn;
					conn.Open();
					using (cmd)
					{
						using (var adapter = new SqlDataAdapter(cmd))
						{
							var ds = new DataSet();
							adapter.Fill(ds);
							return ds;
						}
					}
				}
			}
			else
			{
				using (cmd)
				{
					using (var adapter = new SqlDataAdapter(cmd))
					{
						var ds = new DataSet();
						adapter.Fill(ds);
						return ds;
					}
				}
			}
		}

		protected T GetItem<T>(SqlCommand cmd) where T : class, new()
		{
			var row = GetDataRow(cmd);
			var result = ParseRow<T>(row);
			return result;
		}
		protected T GetItem<T>(string sql, object model = null) where T : class, new()
		{
			var cmd = PrepareSQL(sql, model);
			return GetItem<T>(cmd);
		}
		protected List<T> GetList<T>(SqlCommand cmd) where T : class, new()
		{
			var table = GetTable(cmd);
			var result = new List<T>();
			foreach (DataRow row in table.Rows)
			{
				var item = ParseRow<T>(row);
				result.Add(item);
			}
			return result;
		}
		protected List<T> GetList<T>(string sql, object model = null) where T : class, new()
		{
			var cmd = PrepareSQL(sql, model);
			return GetList<T>(cmd);
		}
	}
}
