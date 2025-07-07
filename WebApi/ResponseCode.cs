using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.WebApi
{
	public static class ResponseCode
	{
		public const string ACCOUNT_BANNED = "ACCOUNT_BANNED";

		public const string UNDEFINED_ERROR = "UNDEFINED_ERROR";
		public const string INVALID_REQUEST_DATA = "INVALID_REQUEST_DATA";
		public const string ALREADY_EXISTS = "ALREADY_EXISTS";
		public const string EMPTY_UID = "EMPTY_UID";
		public const string EMPTY_EMAIL = "EMPTY_EMAIL";
		public const string EMPTY_PASSWORD = "EMPTY_PASSWORD";
		public const string INVALID_EMAIL = "INVALID_EMAIL";
		public const string INVALID_PHONE = "INVALID_PHONE";
		public const string INVALID_PASSWORD = "INVALID_PASSWORD";
		public const string EMPTY_GROUP_NAME = "EMPTY_GROUP_NAME";
		public const string EMPTY_MEMBER_NAME = "EMPTY_MEMBER_NAME";

		public const string AUTH_PHONE_INVALID_CODE = "AUTH_PHONE_INVALID_CODE";
		public const string AUTH_PHONE_WAIT_FOR_RECREATE = "AUTH_PHONE_WAIT_FOR_RECREATE";

		public const string DUPLICATE_ACCOUNTS = "DUPLICATE_ACCOUNTS";
		public const string YOU_MUST_BE_MEMBER_OF_GROUP = "YOU_MUST_BE_MEMBER_OF_GROUP";

	}
}
