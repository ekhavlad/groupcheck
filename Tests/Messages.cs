using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.Server;
using GroupCheck.ServerStorage;
using GroupCheck.Tools;
using GroupCheck.Tools.Extensions;
using GroupCheck.UnitTests;

namespace GroupCheck.UnitTests
{
	public class Messages
	{
		public const string NOT_CREATED = "NOT CREATED";
		public const string NOT_FOUND = "NOT FOUND";
		public const string NOT_CONFIRMED = "NOT CONFIRMED";
		public const string NOT_UPDATED = "NOT UPDATED";
		public const string NOT_RESTORED = "NOT RESTORED";
		public const string NOT_DELETED = "NOT DELETED";
	}
}
