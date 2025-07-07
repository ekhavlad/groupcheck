using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GroupCheck.WebApi;
using GroupCheck.Server;

namespace GroupCheck.WebServer
{
	public class MemberController : BaseApiController, IMemberAPI
	{
		[HttpPost]
		public MemberResponse Create([FromBody] CreateMemberRequest request)
		{
			var newMember = new NewMember()
			{
				GroupID = request.GroupID,
				Name = request.Name,
				AccountID = request.AccountID
			};
			var member = _server.Members.Create(newMember);
			var response = GetMemberResponse(member);
			return response;
		}

		[HttpGet("Member/Confirm/{groupID}/{memberID}")]
		public MemberResponse Confirm(int groupID, int memberID)
		{
			var member = _server.Members.Confirm(groupID, memberID);
			var response = GetMemberResponse(member);
			return response;
		}

		[HttpPost]
		public MemberResponse Update([FromBody] UpdateMemberRequest request)
		{
			var updateMember = new UpdateMember()
			{
				GroupID = request.GroupID,
				MemberID = request.MemberID,
				Name = request.Name
			};
			var member = _server.Members.Update(updateMember);
			var response = GetMemberResponse(member);
			return response;
		}

		[HttpDelete("Member/Delete/{groupID}/{memberID}")]
		public void Delete(int groupID, int memberID)
		{
			_server.Members.Delete(groupID, memberID);
		}

		#region ctor
		public MemberController(IServer server) : base(server) { }
		#endregion

		internal static MemberResponse GetMemberResponse(Member member)
		{
			return new MemberResponse()
			{
				MemberID = member.MemberID,
				GroupID = member.GroupID,
				Name = member.Name,
				AccountID = member.AccountID,
				Created = member.Created,
				CreatedByID = member.CreatedByID,
				Updated = member.Updated,
				UpdatedByID = member.UpdatedByID,
				Revision = member.Revision,
			};
		}
	}
}
