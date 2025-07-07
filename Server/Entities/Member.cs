using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GroupCheck.Server
{
	public class Member : IEntity
	{
		public int GroupID { get; set; }
		public int MemberID { get; set; }
		public int? AccountID { get; set; }
		public string Name { get; set; }

		public bool Confirmed { get; set; }
		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
		public bool Deleted { get; set; }
	}

	public class NewMember
	{
		public int GroupID { get; set; }
		public int? AccountID { get; set; }
		public string Name { get; set; }
	}

	public class UpdateMember
	{
		public int GroupID { get; set; }
		public int MemberID { get; set; }
		public string Name { get; set; }
	}
}
