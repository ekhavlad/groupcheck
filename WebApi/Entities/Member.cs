using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.WebApi
{
	public interface IMemberAPI
	{
		MemberResponse Create(CreateMemberRequest request);
		MemberResponse Confirm(int groupID, int memberID);
		MemberResponse Update(UpdateMemberRequest request);
		void Delete(int groupID, int memberID);
	}

	public class MemberResponse
	{
		public int GroupID { get; set; }
		public int MemberID { get; set; }
		public string Name { get; set; }
		public int? AccountID { get; set; }

		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
	}

	public class CreateMemberRequest
	{
		public int GroupID { get; set; }
		public string Name { get; set; }
		public int? AccountID { get; set; }
	}

	public class UpdateMemberRequest
	{
		public int GroupID { get; set; }
		public int MemberID { get; set; }
		public string Name { get; set; }
		public int? AccountID { get; set; }
	}
}
