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
	public partial class GroupCheckAdapter : IMemberAPI
	{
		MemberResponse IMemberAPI.Create(CreateMemberRequest request)
		{
			var created = Post<MemberResponse>("Member/Create", request);
			return created;
		}
		MemberResponse IMemberAPI.Confirm(int groupID, int memberID)
		{
			var confirmed = Get<MemberResponse>($"Member/Confirm/{groupID}/{memberID}");
			return confirmed;
		}
		MemberResponse IMemberAPI.Update(UpdateMemberRequest request)
		{
			var updated = Post<MemberResponse>("Member/Update", request);
			return updated;
		}
		void IMemberAPI.Delete(int groupID, int id)
		{
			Delete($"Member/Delete/{groupID}/{id}");
		}
	}
}
