using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;
using GroupCheck.ServerStorage;
using System.Threading;

namespace GroupCheck.Server
{
	public partial class Core
	{
		[Flags]
		public enum AccessRights
		{
			GetGroup = 1,
			CreateGroup = 2,
			ConfirmGroup = 4,
			UpdateGroup = 8,

			GetMember = 16,
			CreateMember = 32,
			ConfirmMember = 64,
			UpdateMember = 128,
			DeleteMember = 256,

			GetCheck = 512,
			CreateCheck = 1024,
			ConfirmCheck = 2048,
			UpdateCheck = 4096,
			DeleteCheck = 8192
		}

		private AccessRights GetDefaultAccessRights()
		{
			return AccessRights.CreateGroup;
		}
		private AccessRights GetAccessRights(Group group)
		{
			var rights = (AccessRights)GetDefaultAccessRights();

			if (group.CreatedByID == CurrentAccount.ID) rights |= AccessRights.ConfirmGroup;
			if (!group.Confirmed)
				return rights;

			if (!group.Members.Any(m => m.AccountID == CurrentAccount.ID && !m.Deleted))
				return rights;

			rights |= AccessRights.GetGroup;
			rights |= AccessRights.UpdateGroup;
			rights |= AccessRights.CreateMember;
			rights |= AccessRights.CreateCheck;

			return rights;
		}
		/// <summary>
		/// Validates the user rights. Throws <see cref="AccessDeniedException"/> in case of unsufficient rights.
		/// </summary>
		private void RequireAccessRight(AccessRights right)
		{
			if (!GetDefaultAccessRights().HasFlag(right))
				throw new AccessDeniedException();
		}
		/// <summary>
		/// Validates the user rights for the group.
		/// Throws <see cref="NotFoundException"/> in case of NULL value,
		/// <see cref="DeletedException"/> in case of deleted value,
		/// <see cref="AccessDeniedException"/> in case of unsufficient rights.
		/// </summary>
		private void RequireAccessRight(Group group, AccessRights right)
		{
			RequireExistance(group);
			var rights = GetAccessRights(group);
			if (!rights.HasFlag(right))
				throw new AccessDeniedException();
		}
		/// <summary>
		/// Validates the user rights for the group and member.
		/// Throws <see cref="NotFoundException"/> in case of NULL value,
		/// <see cref="DeletedException"/> in case of deleted value,
		/// <see cref="AccessDeniedException"/> in case of unsufficient rights.
		/// </summary>
		private void RequireAccessRight(Group group, Member member, AccessRights right)
		{
			RequireExistance(group);
			RequireExistance(member);
			var rights = GetAccessRights(group);

			if (member.CreatedByID == CurrentAccount.ID || rights.HasFlag(AccessRights.CreateMember))
				rights |= AccessRights.ConfirmMember;

			if (member.Confirmed)
			{
				rights |= AccessRights.UpdateMember;
				rights |= AccessRights.DeleteMember;
				rights |= AccessRights.GetMember;
			}

			if (!rights.HasFlag(right))
				throw new AccessDeniedException();
		}
		/// <summary>
		/// Validates the user rights for the group and check.
		/// Throws <see cref="NotFoundException"/> in case of NULL value,
		/// <see cref="DeletedException"/> in case of deleted value,
		/// <see cref="AccessDeniedException"/> in case of unsufficient rights.
		/// </summary>
		private void RequireAccessRight(Group group, Check check, AccessRights right)
		{
			RequireExistance(group);
			RequireExistance(check);
			var rights = GetAccessRights(group);

			if (check.CreatedByID == CurrentAccount.ID) rights |= AccessRights.ConfirmCheck;

			if (check.Confirmed)
			{
				rights |= AccessRights.UpdateCheck;
				rights |= AccessRights.DeleteCheck;
				rights |= AccessRights.GetCheck;
			}

			if (!rights.HasFlag(right))
				throw new AccessDeniedException();
		}
	}
}
