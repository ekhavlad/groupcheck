using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GroupCheck.Server
{
	public class Group : IEntity
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public IEnumerable<Member> Members { get; set; }

		public bool Confirmed { get; set; }
		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
		public bool Deleted { get; set; }
	}

	public class NewGroup
	{
		public string Name { get; set; }
		public IEnumerable<NewGroupMember> Members { get; set; }
	}

	public class NewGroupMember
	{
		public int? AccountID { get; set; }
		public string Name { get; set; }
	}

	public class UpdateGroup
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}
}
