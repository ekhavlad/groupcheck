using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using GroupCheck.WebApi;
using GroupCheck.UnitTests;
using GroupCheck.ServerProxy;

namespace GroupCheck.UnitTests.WebApi
{
	[TestFixture]
	public class Groups : _Base
	{
		[Test, Order(0)]
		public void CRUD()
		{
			var members = new List<CreateGroupMemberRequest>()
			{
				new CreateGroupMemberRequest() { AccountID = api.AccountID, Name = "author" },
				new CreateGroupMemberRequest() { AccountID = null, Name = "additional user" }
			};

			var newEmptyGroup = new CreateGroupRequest()
			{
				Name = "empty group",
				Members = members
			};

			var group = api.Groups.Create(newEmptyGroup);
			api.Groups.Confirm(group.ID);
			api.Groups.Update(new UpdateGroupRequest() { ID = group.ID, Name = "updated name" });
			api.Groups.Exit(group.ID);
		}
	}
}
