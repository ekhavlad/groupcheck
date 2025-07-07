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
		Group IServerGroups.Create(NewGroup newGroup)
		{
			RequireAuthentication();
			RequireAccessRight(AccessRights.CreateGroup);

			if (string.IsNullOrEmpty(newGroup?.Name))
				throw new ValidationException(ValidationCode.GroupEmptyName);

			if (newGroup.Members == null || !newGroup.Members.Any(m => m.AccountID == CurrentAccount.ID))
				throw new ValidationException(ValidationCode.YouMustBeMemberOfGroup);

			if (newGroup.Members.Where(m => m.AccountID != null).GroupBy(m => m.AccountID).Any(_ => _.Count() > 1))
				throw new ValidationException(ValidationCode.DuplicateAccounts);

			var now = Now();
			var group = new Group()
			{
				Name = newGroup.Name,
				Created = now,
				CreatedByID = CurrentAccount.ID,
				Updated = now,
				UpdatedByID = CurrentAccount.ID,

				Members = newGroup.Members.Select(m =>
					new Member()
					{
						AccountID = m.AccountID,
						Name = m.Name,
						Confirmed = true, // it will be enough to confirm only Group itself in the future
						Created = now,
						CreatedByID = CurrentAccount.ID,
						Updated = now,
						UpdatedByID = CurrentAccount.ID,
					})
			};

			group = storage.Groups.Create(group);
			return group;
		}

		Group IServerGroups.Confirm(int groupID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			RequireAccessRight(group, AccessRights.ConfirmGroup);

			if (group.Confirmed)
				return group;

			group.Confirmed = true;
			group.Updated = Now();

			group = storage.Groups.Update(group);
			return group;
		}

		Group IServerGroups.Get(int groupID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			RequireAccessRight(group, AccessRights.GetGroup);
			return group;
		}

		Group IServerGroups.Update(UpdateGroup updateGroup)
		{
			RequireNotNull(updateGroup);
			RequireAuthentication();
			var group = storage.Groups.Get(updateGroup.ID);
			RequireAccessRight(group, AccessRights.UpdateGroup);

			if (string.IsNullOrEmpty(updateGroup.Name))
				throw new ValidationException(ValidationCode.GroupEmptyName);

			if (group.Name == updateGroup.Name)
				return group;

			group.Name = updateGroup.Name;

			group.Updated = Now();
			group.UpdatedByID = CurrentAccount.ID;

			group = storage.Groups.Update(group);
			return group;
		}

		void IServerGroups.Exit(int groupID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			RequireAccessRight(group, AccessRights.GetGroup);

			var member = group.Members.SingleOrDefault(m => m.AccountID == CurrentAccount.ID);
			if (member == null || member.Deleted)
				return;

			member.Deleted = true;
			member.Updated = Now();
			storage.Members.Update(member);

			if (!group.Members.Any(m => !m.Deleted))
			{
				group.Deleted = true;
				group.Updated = Now();
				storage.Groups.Update(group);
			}
		}
	}
}
