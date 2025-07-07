using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;
using GroupCheck.Tools;
using GroupCheck.Tools.Extensions;

namespace GroupCheck.Server
{
	public partial class Core
	{
		Member IServerMembers.Create(NewMember newMember)
		{
			RequireNotNull(newMember);
			RequireAuthentication();
			var group = storage.Groups.Get(newMember.GroupID);
			RequireAccessRight(group, AccessRights.CreateMember);

			if (string.IsNullOrWhiteSpace(newMember.Name))
				throw new ValidationException(ValidationCode.MemberEmptyName);

			if (newMember.AccountID != null && group.Members.Any(m => m.AccountID == newMember.AccountID))
			{
				return group.Members.Single(m => m.AccountID == newMember.AccountID);
			}

			var member = new Member()
			{
				GroupID = newMember.GroupID,
				AccountID = newMember.AccountID,
				Name = newMember.Name,

				Created = Now(),
				CreatedByID = CurrentAccount.ID,
				Updated = Now(),
				UpdatedByID = CurrentAccount.ID,
			};

			member = storage.Members.Create(member);
			return member;
		}

		Member IServerMembers.Confirm(int groupID, int memberID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var member = group?.Members.SingleOrDefault(m => m.MemberID == memberID);
			RequireAccessRight(group, member, AccessRights.ConfirmMember);

			if (member.Confirmed)
				return member;

			member.Confirmed = true;
			member.Updated = Now();

			member = storage.Members.Update(member);
			return member;
		}

		Member IServerMembers.Get(int groupID, int memberID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var member = group?.Members.SingleOrDefault(m => m.MemberID == memberID);
			RequireAccessRight(group, member, AccessRights.GetMember);
			return member;
		}

		Member IServerMembers.Update(UpdateMember updateMember)
		{
			RequireNotNull(updateMember);
			RequireAuthentication();
			var group = storage.Groups.Get(updateMember.GroupID);
			var member = group?.Members.SingleOrDefault(m => m.MemberID == updateMember.MemberID);
			RequireAccessRight(group, member, AccessRights.UpdateMember);

			if (string.IsNullOrWhiteSpace(updateMember.Name))
				throw new ValidationException(ValidationCode.MemberEmptyName);

			if (member.Name == updateMember.Name)
				return member;

			member.Name = updateMember.Name;
			member.Updated = Now();
			member.UpdatedByID = CurrentAccount.ID;

			member = storage.Members.Update(member);
			return member;
		}

		void IServerMembers.Delete(int groupID, int memberID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var member = group?.Members.SingleOrDefault(m => m.MemberID == memberID);
			RequireAccessRight(group, member, AccessRights.DeleteMember);

			member.Deleted = true;
			member.Updated = Now();
			storage.Members.Update(member);
		}
	}
}
