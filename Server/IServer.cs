using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;

namespace GroupCheck.Server
{
	public interface IServer
	{
		Account CurrentAccount { get; }
		void SetCurrentAccount(int accountID);
		void WorkAsAnonymous();
		bool IsAnonymous { get; }

		IServerAccounts Accounts { get; }
		IServerGroups Groups { get; }
		IServerMembers Members { get; }
		IServerChecks Checks { get; }
	}

	public interface IServerAccounts
	{
		Account FindOrCreate(AuthScheme schemeID, string uid);
		Account GetCurrent();
		Account Get(int id);
		Account Update(UpdateAccount updateAccount);
		void Delete();

		Account Register(NewAccount newAccount);
		Account FindByEmail(string email);
		Account ValidateEmail(string email, string password);
		Account ChangePassword(string password);
	}

	public interface IServerGroups
	{
		Group Create(NewGroup newGroup);
		Group Confirm(int groupID);
		Group Get(int groupID);
		Group Update(UpdateGroup updateGroup);
		void Exit(int groupID);
	}

	public interface IServerMembers
	{
		Member Create(NewMember newMember);
		Member Confirm(int groupID, int memberID);
		Member Get(int groupID, int memberID);
		Member Update(UpdateMember updateMember);
		void Delete(int groupID, int memberID);
	}

	public interface IServerChecks
	{
		Check Create(NewCheck newCheck);
		Check Confirm(int groupID, int checkID);
		Check Get(int groupID, int checkID);
		Check Update(UpdateCheck updateCheck);
		void Delete(int groupID, int checkID);
	}

	public enum AuthScheme : byte
	{
		Phone,
	}
}
