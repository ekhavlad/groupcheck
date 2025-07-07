using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.Server;

namespace GroupCheck.UnitTests.Server
{
	public class ServerOnTest : IDisposable
	{
		public readonly IServer Server;
		private readonly ServerStorage.IStorageOnTest storage;
		public ServerOnTest(ServerStorage.IStorageOnTest storage)
		{
			this.storage = storage;
			Server = new Core(storage.Storage);
		}
		public void Dispose()
		{
			storage.Dispose();
		}
	}

	public class EmptyServerOnTest : ServerOnTest
	{
		public EmptyServerOnTest() : base(new ServerStorage.EmptyStorageOnTest()) { }
	}
}
