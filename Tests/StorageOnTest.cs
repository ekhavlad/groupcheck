using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.ServerStorage;

namespace GroupCheck.UnitTests.ServerStorage
{
	public interface IStorageOnTest : IDisposable
	{
		IServerStorage Storage { get; }
	}

	public class StorageOnTest : IStorageOnTest
	{
		protected const string host = ".";
		protected const string user = "sa";
		protected const string pass = "Aw34esz";

		protected const string backups = "D:\\_Backups\\gc";
		protected const string sqlWorkingDir = "C:\\SQL\\gc";

		public readonly IServerStorage _storage;
		public IServerStorage Storage { get { return _storage; } }
		private readonly string snapshot;
		private readonly bool fromBackup;
		private readonly string activeDb;

		public StorageOnTest(string db)
		{
			fromBackup = false;
			activeDb = db;
			_storage = new MSSQLStorage(host, user, pass, activeDb);
			snapshot = Guid.NewGuid().ToString();
			((MSSQLStorage)Storage).SysCreateSnapshot(snapshot, sqlWorkingDir);
		}
		public StorageOnTest(string backupFile, string db)
		{
			fromBackup = true;
			activeDb = db;
			var master = new MSSQLStorage(host, user, pass, "master");

			if (!System.IO.File.Exists(backupFile))
				backupFile = backups + "\\" + backupFile;

			master.SysRestoreBackup(activeDb, backupFile, sqlWorkingDir);

			_storage = new MSSQLStorage(host, user, pass, activeDb);
		}

		public void CreateBackUp(string backupFile)
		{
			((MSSQLStorage)Storage).SysCreateBackup(backupFile);
		}

		public void Dispose()
		{
			if (fromBackup)
			{
				((MSSQLStorage)Storage).SysDropDatabase(activeDb);
			}
			else
			{
				((MSSQLStorage)Storage).SysRestoreSnapshot(snapshot);
				((MSSQLStorage)Storage).SysDeleteSnapshot(snapshot);
			}
		}
	}

	public class EmptyStorageOnTest : IStorageOnTest
	{
		protected const string host = ".";
		protected const string user = "sa";
		protected const string pass = "Aw34esz";

		protected const string backups = "D:\\_Backups\\gc";
		protected const string sqlWorkingDir = "C:\\SQL\\gc";

		public readonly IServerStorage _storage;
		public IServerStorage Storage { get { return _storage; } }
		private readonly string activeDb;

		public EmptyStorageOnTest()
		{
			activeDb = Guid.NewGuid().ToString();

			var master = new MSSQLStorage(host, user, pass, "master");
			master.SysCreateDatabase(activeDb);

			var created = new MSSQLStorage(host, user, pass, activeDb);
			var createSQL = System.IO.File.ReadAllText("../../../../Server/Storage/MSSQL/database.sql");
			foreach (var statement in createSQL.Split("\nGO").Where(_ => !string.IsNullOrWhiteSpace(_)))
			{
				var cmd = new System.Data.SqlClient.SqlCommand(statement);
				created.Execute(cmd);
			}

			_storage = created;
		}

		public void Dispose()
		{
			((MSSQLStorage)Storage).SysDropDatabase(activeDb);
		}
	}
}
