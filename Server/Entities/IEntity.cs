using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GroupCheck.Server
{
	public interface IEntity
	{
		bool Confirmed { get; set; }
		DateTime Created { get; set; }
		int CreatedByID { get; set; }
		DateTime Updated { get; set; }
		int UpdatedByID { get; set; }
		long Revision { get; set; }
		bool Deleted { get; set; }
	}
}
