using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.WebApi
{
	public interface IGroupAPI
	{
		GroupResponse Create(CreateGroupRequest request);
		GroupResponse Confirm(int groupID);
		GroupResponse Update(UpdateGroupRequest request);
		void Exit(int groupID);
	}

	public class GroupResponse
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public IEnumerable<MemberResponse> Members { get; set; }

		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
	}

	public class CreateGroupRequest
	{
		public string Name { get; set; }
		public IEnumerable<CreateGroupMemberRequest> Members { get; set; }
	}
	public class CreateGroupMemberRequest
	{
		public int? AccountID { get; set; }
		public string Name { get; set; }
	}

	public class UpdateGroupRequest
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}
}
