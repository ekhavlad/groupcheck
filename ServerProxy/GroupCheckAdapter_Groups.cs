using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using GroupCheck.WebApi;
using GroupCheck.Tools.Extensions;

namespace GroupCheck.ServerProxy
{
	public partial class GroupCheckAdapter : IGroupAPI
	{
		GroupResponse IGroupAPI.Create(CreateGroupRequest request)
		{
			var created = Post<GroupResponse>("Group/Create", request);
			return created;
		}
		GroupResponse IGroupAPI.Confirm(int id)
		{
			var confirmed = Get<GroupResponse>($"Group/Confirm/{id}");
			return confirmed;
		}
		GroupResponse IGroupAPI.Update(UpdateGroupRequest request)
		{
			var updated = Post<GroupResponse>($"Group/Update", request);
			return updated;
		}
		void IGroupAPI.Exit(int id)
		{
			Delete($"Group/Exit/{id}");
		}
	}
}
