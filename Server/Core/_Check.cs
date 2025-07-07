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
		Check IServerChecks.Create(NewCheck newCheck)
		{
			RequireNotNull(newCheck);
			RequireAuthentication();
			var group = storage.Groups.Get(newCheck.GroupID);
			RequireAccessRight(group, AccessRights.CreateCheck);
			
			var check = new Check()
			{
				GroupID = newCheck.GroupID,
				DateAndTime = newCheck.DateAndTime,
				Description = newCheck.Description,
				Creditors = newCheck.Creditors,
				Debitors = newCheck.Debitors,

				Created = Now(),
				CreatedByID = CurrentAccount.ID,
				Updated = Now(),
				UpdatedByID = CurrentAccount.ID,
			};
			check = storage.Checks.Create(check);

			return check;
		}

		Check IServerChecks.Confirm(int groupID, int checkID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var check = storage.Checks.Get(groupID, checkID);
			RequireAccessRight(group, check, AccessRights.ConfirmCheck);

			if (check.CreatedByID != CurrentAccount.ID)
				throw new AccessDeniedException();

			if (check.Confirmed)
				return check;

			check.Confirmed = true;
			check.Updated = Now();

			check = storage.Checks.Update(check);
			return check;
		}

		Check IServerChecks.Get(int groupID, int checkID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var check = storage.Checks.Get(groupID, checkID);
			RequireAccessRight(group, check, AccessRights.GetCheck);
			return check;
		}

		Check IServerChecks.Update(UpdateCheck updateCheck)
		{
			RequireNotNull(updateCheck);
			RequireAuthentication();
			var group = storage.Groups.Get(updateCheck.GroupID);
			var check = storage.Checks.Get(updateCheck.GroupID, updateCheck.CheckID);
			RequireAccessRight(group, check, AccessRights.UpdateCheck);

			check.DateAndTime = updateCheck.DateAndTime;
			check.Description = updateCheck.Description;
			check.Debitors = updateCheck.Debitors;
			check.Creditors = updateCheck.Creditors;

			check.Updated = Now();
			check.UpdatedByID = CurrentAccount.ID;

			check = storage.Checks.Update(check);
			return check;
		}

		void IServerChecks.Delete(int groupID, int checkID)
		{
			RequireAuthentication();
			var group = storage.Groups.Get(groupID);
			var check = storage.Checks.Get(groupID, checkID);
			RequireAccessRight(group, check, AccessRights.DeleteCheck);

			check.Deleted = true;
			check.Updated = Now();
			storage.Checks.Update(check);
		}
	}
}
