using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	public enum ValidationCode
	{
		Common,

		AccountEmptyUID,

		AccountEmptyEmail,
		AccountEmptyPassword,
		AccountInvalidEmail,
		AccountInvalidPhone,
		AccountInvalidPassword,

		GroupEmptyName,

		YouMustBeMemberOfGroup,
		DuplicateAccounts,

		MemberEmptyName,
	}
}
