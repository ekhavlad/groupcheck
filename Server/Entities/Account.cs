using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	public class Account
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Salt { get; set; }
		public string Password { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public long Revision { get; set; }
		public bool Deleted { get; set; }
		public DateTime? BlockedUntil { get; set; }
	}

	public class NewAccount
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class UpdateAccount
	{
		public string Name { get; set; }
	}
}
