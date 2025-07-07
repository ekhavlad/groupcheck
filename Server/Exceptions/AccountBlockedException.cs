using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	[Serializable]
	public class AccountBlockedException : Exception
	{
		public DateTime BlockedUntil { get; set; }

		public AccountBlockedException(DateTime blockedUntil)
			: base("AccountBlockedException")
		{
			BlockedUntil = blockedUntil;
		}
	}
}
