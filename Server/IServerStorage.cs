using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;

namespace GroupCheck.ServerStorage
{
	public partial interface IServerStorage
	{
		IServerStorageAccounts Accounts { get; }
		IServerStorageGroups Groups { get; }
		IServerStorageMembers Members { get; }
		IServerStorageChecks Checks { get; }
	}
	public partial interface IServerStorageAccounts
	{
		Account FindOrCreate(AuthScheme schemeID, string uid);
		Account Create(Account account);
		Account Get(int accountID);
		Account FindByEmail(string email);
		Account Update(Account account);
		void Delete(int accountID);
	}
	public partial interface IServerStorageGroups
	{
		Group Create(Group group);
		Group Get(int groupID);
		IEnumerable<Group> GetGroupsByAccount(int accountID, long minRevision, int batchSize);
		Group Update(Group group);
		void Delete(int groupID);
	}
	public partial interface IServerStorageMembers
	{
		Member Create(Member member);
		Member Get(int groupID, int memberID);
		IEnumerable<Member> GetMembersByGroup(int groupID, long minRevision, int batchSize);
		Member Update(Member member);
		void Delete(int groupID, int memberID);
	}
	public partial interface IServerStorageChecks
	{
		Check Create(Check check);
		Check Get(int groupID, int checkID);
		IEnumerable<Check> GetChecksByGroup(int groupID, long minRevision, int batchSize);
		Check Update(Check check);
		void Delete(int groupID, int checkID);
	}
}
