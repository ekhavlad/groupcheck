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
		/// <summary>
		/// Throws <see cref="AuthenticationRequiredException"/> if server is working with anonimous account now.
		/// </summary>
		private void RequireAuthentication()
		{
			if (IsAnonymous)
				throw new AuthenticationRequiredException();
		}

		/// <summary>
		/// Throws <see cref="ArgumentNullException"/> in case of NULL value.
		/// </summary>
		private void RequireNotNull(object value)
		{
			if (value == null)
				throw new ArgumentNullException();
		}

		/// <summary>
		/// Throws <see cref="NotFoundException"/> in case of NULL value or <see cref="DeletedException"/> in case of deleted value.
		/// </summary>
		private void RequireExistance(IEntity entity)
		{
			if (entity == null)
				throw new NotFoundException();

			if (entity.Deleted)
				throw new DeletedException();
		}

		/// <summary>
		/// Throws <see cref="NotFoundException"/> in case of NULL or not confirmed value or <see cref="DeletedException"/> in case of deleted value.
		/// </summary>
		private void RequireExistanceAndConfirmed<T>(IEntity entity)
		{
			if (entity == null || !entity.Confirmed)
				throw new NotFoundException(typeof(T).Name);

			if (entity.Deleted)
				throw new DeletedException(typeof(T).Name);
		}

		/// <summary>
		/// Throws <see cref="NotFoundException"/> in case of NULL value or <see cref="DeletedException"/> in case of deleted value.
		/// </summary>
		private void RequireExistance(Account account)
		{
			if (account == null)
				throw new NotFoundException(typeof(Account).Name);

			if (account.Deleted)
				throw new DeletedException(typeof(Account).Name);
		}
	}
}
