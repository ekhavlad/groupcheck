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
	public partial class GroupCheckAdapter : ICheckAPI
	{
		CheckResponse ICheckAPI.Create(CreateCheckRequest request)
		{
			var created = Post<CheckResponse>("Check/Create", request);
			return created;
		}
		CheckResponse ICheckAPI.Confirm(int groupID, int id)
		{
			var confirmed = Get<CheckResponse>($"Check/Confirm/{groupID}/{id}");
			return confirmed;
		}
		CheckResponse ICheckAPI.Update(UpdateCheckRequest request)
		{
			var updated = Post<CheckResponse>("Check/Update", request);
			return updated;
		}
		void ICheckAPI.Delete(int groupID, int id)
		{
			Delete($"Check/Delete/{groupID}/{id}");
		}
	}
}
