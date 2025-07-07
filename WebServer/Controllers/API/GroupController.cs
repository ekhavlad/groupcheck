using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GroupCheck.WebApi;
using GroupCheck.Server;

namespace GroupCheck.WebServer
{
	public class GroupController : BaseApiController, IGroupAPI
	{
		[HttpPost]
		public GroupResponse Create([FromBody] CreateGroupRequest request)
		{
			var newGroup = ParseNewGroup(request);
			var group = _server.Groups.Create(newGroup);
			var response = GetGroupResponse(group);
			return response;
		}

		[HttpGet("Group/Confirm/{groupID}")]
		public GroupResponse Confirm(int groupID)
		{
			var group = _server.Groups.Confirm(groupID);
			var response = GetGroupResponse(group);
			return response;
		}

		[HttpPost]
		public GroupResponse Update([FromBody] UpdateGroupRequest request)
		{
			var updateGroup = new UpdateGroup()
			{
				ID = request.ID,
				Name = request.Name
			};
			var group = _server.Groups.Update(updateGroup);
			var response = GetGroupResponse(group);
			return response;
		}

		[HttpDelete("Group/Exit/{groupID}")]
		public void Exit(int groupID)
		{
			_server.Groups.Exit(groupID);
		}

		#region ctor
		public GroupController(IServer server) : base(server) { }
		#endregion

		internal static NewGroup ParseNewGroup(CreateGroupRequest group)
		{
			group.Members = (group.Members ?? new List<CreateGroupMemberRequest>()).Where(m => m != null);
			return new NewGroup()
			{
				Name = group.Name,
				Members = group.Members.Select(m =>
					new NewGroupMember()
					{
						Name = m.Name,
						AccountID = m.AccountID
					})
			};
		}
		internal static GroupResponse GetGroupResponse(Group group)
		{
			group.Members = (group.Members ?? new List<Member>()).Where(m => m != null);
			return new GroupResponse()
			{
				ID = group.ID,
				Name = group.Name,
				Created = group.Created,
				CreatedByID = group.CreatedByID,
				Updated = group.Updated,
				UpdatedByID = group.UpdatedByID,
				Revision = group.Revision,
				Members = group.Members.Select(m => MemberController.GetMemberResponse(m))
			};
		}
	}
}
