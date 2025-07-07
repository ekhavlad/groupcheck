using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Tools
{
	public static class Email
	{
		public static bool TryParse(string emailaddress, out string email, out string name)
		{
			try
			{
				var m = new System.Net.Mail.MailAddress(emailaddress);
				email = m.Address;
				name = m.DisplayName;
				return true;
			}
			catch (FormatException)
			{
				email = name = null;
				return false;
			}
		}
	}
}
