using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GroupCheck.Server
{
	public class Check : IEntity
	{
		public int GroupID { get; set; }
		public int CheckID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		public IDictionary<int, int> Creditors { get; set; }
		public IDictionary<int, int> Debitors { get; set; }

		public bool Confirmed { get; set; }
		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
		public bool Deleted { get; set; }
	}

	public class NewCheck
	{
		public int GroupID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		public IDictionary<int, int> Creditors { get; set; }
		public IDictionary<int, int> Debitors { get; set; }
	}

	public class UpdateCheck
	{
		public int GroupID { get; set; }
		public int CheckID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		public IDictionary<int, int> Creditors { get; set; }
		public IDictionary<int, int> Debitors { get; set; }
	}
}
