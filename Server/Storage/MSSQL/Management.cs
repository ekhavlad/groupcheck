using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace GroupCheck.ServerStorage
{
	public partial class MSSQLStorage : IServerStorage
	{
		public void SysCreateBackup(string file)
		{
			SysCreateBackup(_db, file);
		}
		public void SysCreateBackup(string database, string file)
		{
			Execute($"BACKUP DATABASE [{database}] TO DISK = '{file}' WITH COMPRESSION");
		}
		public void SysRestoreBackup(string file, string path)
		{
			SysRestoreBackup(_db, file, path);
		}
		public void SysRestoreBackup(string database, string file, string path)
		{
			if (!System.IO.File.Exists(file))
				throw new System.IO.FileNotFoundException(file);

			path = path.TrimEnd('/', '\\') + "\\";
			var master = new MSSQLStorage(_host, _user, _pass, "master");
			var cmd = new SqlCommand($"RESTORE FILELISTONLY FROM DISK = '{file}' WITH FILE = 1");
			var files = master.GetTable(cmd).Rows.Cast<DataRow>()
				.ToDictionary(r => (string)r["LogicalName"], r => System.IO.Path.GetFileName((string)r["PhysicalName"]));

			var n = 0;
			for (var i = 0; i < files.Values.Min(k => k.Length) && files.Values.All(s => s[i] == files.Values.First()[i]); i++)
			{
				n++;
			}
			foreach (var k in files.Keys.ToList())
			{
				files[k] = path + database + files[k].Substring(n);
			}

			master.KillSessions(database);
			var restore = $@"RESTORE DATABASE [{database}] FROM DISK = '{file}' WITH REPLACE, " + string.Join(",", files.Select(f => $"MOVE '{f.Key}' TO '{f.Value}'"));
			master.Execute(restore);
		}
		public void SysCreateSnapshot(string snapshot, string filepath)
		{
			SysCreateSnapshot(_db, snapshot, filepath);
		}
		public void SysCreateSnapshot(string database, string snapshot, string filepath)
		{
			filepath = filepath.TrimEnd('/', '\\');
			var sql = $@"
				DECLARE @FileSql varchar(3000) = '', @SnapSql nvarchar(4000)
				SELECT @FileSql = @FileSql +
					CASE WHEN @FileSql <> '' THEN + ',' ELSE '' END + '(NAME = ''' + mf.name + ''', FILENAME = ''{filepath}\' + mf.name + '_{snapshot}.ss'')'
				FROM sys.master_files AS mf
					INNER JOIN sys.databases AS db ON db.database_id = mf.database_id
				WHERE db.state = 0 AND mf.type = 0 AND db.[name] = '{database}'

				SET @SnapSql = 'CREATE DATABASE [{database}_{snapshot}] ON ' + @FileSql + ' AS SNAPSHOT OF [{database}];'

				EXEC sp_executesql @stmt = @SnapSql";
			Execute(sql);
		}
		public void SysRestoreSnapshot(string snapshot)
		{
			SysRestoreSnapshot(_db, snapshot);
		}
		public void SysRestoreSnapshot(string database, string snapshot)
		{
			var master = new MSSQLStorage(_host, _user, _pass, "master");
			master.KillSessions(database);

			var restore = $@"RESTORE DATABASE [{database}] from DATABASE_SNAPSHOT = '{database}_{snapshot}'";
			master.Execute(restore);
		}
		public void SysDeleteSnapshot(string snapshot)
		{
			SysDeleteSnapshot(_db, snapshot);
		}
		public void SysDeleteSnapshot(string database, string snapshot)
		{
			var master = new MSSQLStorage(_host, _user, _pass, "master");
			master.KillSessions($"{database}_{snapshot}");
			master.Execute($"DROP DATABASE [{database}_{snapshot}]");
		}

		public void SysCreateDatabase(string database)
		{
			var create = $@"CREATE DATABASE [{database}]";
			Execute(create);
		}
		public void SysDropDatabase(string database)
		{
			var master = new MSSQLStorage(_host, _user, _pass, "master");
			master.KillSessions(database);
			var drop = $@"DROP DATABASE [{database}]";
			master.Execute(drop);
		}

		private void KillSessions(string database)
		{
			var kill = $@"
				DECLARE @kill varchar(8000) = '';
				SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'
					FROM sys.dm_exec_sessions
					WHERE database_id = db_id('{database}');
				EXEC(@kill);";
			Execute(kill);
		}
	}
}
