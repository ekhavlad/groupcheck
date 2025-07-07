using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupCheck.Server;
using GroupCheck.Tools;
using GroupCheck.Tools.Extensions.RuntimeExt;

namespace GroupCheck.Server
{
	public partial class Core
	{
		Account IServerAccounts.FindOrCreate(AuthScheme schemeID, string uid)
		{
			if (string.IsNullOrEmpty(uid))
				throw new ValidationException(ValidationCode.AccountEmptyUID);

			switch (schemeID)
			{
				case AuthScheme.Phone:
					if (uid.Length != 11 || uid.Any(letter => !char.IsDigit(letter)))
						throw new ValidationException(ValidationCode.AccountInvalidPhone);
					break;

				default:
					throw new ArgumentException($"Unsupported AuthScheme: {schemeID}", nameof(schemeID));
			}

			var account = storage.Accounts.FindOrCreate(schemeID, uid);

			RequireExistance(account);

			return account;
		}
		Account IServerAccounts.Get(int id)
		{
			RequireAuthentication();
			var account = storage.Accounts.Get(id);
			RequireExistance(account);
			return account;
		}
		Account IServerAccounts.GetCurrent()
		{
			RequireAuthentication();
			return CurrentAccount.CreateCopy();
		}
		Account IServerAccounts.Update(UpdateAccount updateAccount)
		{
			RequireNotNull(updateAccount);
			RequireAuthentication();

			_currentAccount.Name = updateAccount.Name;
			_currentAccount.Updated = Now();

			_currentAccount = storage.Accounts.Update(_currentAccount);
			return CurrentAccount.CreateCopy();
		}
		void IServerAccounts.Delete()
		{
			RequireAuthentication();

			_currentAccount.Deleted = true;
			_currentAccount.Updated = Now();
			storage.Accounts.Update(_currentAccount);

			SetCurrentAccount(ANONYMOUS);
		}

		Account IServerAccounts.Register(NewAccount newAccount)
		{
			RequireNotNull(newAccount);

			if (string.IsNullOrEmpty(newAccount.Email))
				throw new ValidationException(ValidationCode.AccountEmptyEmail);

			if (string.IsNullOrEmpty(newAccount.Password))
				throw new ValidationException(ValidationCode.AccountEmptyPassword);

			if (!Email.TryParse(newAccount.Email, out string parsedEmail, out string parsedName))
				throw new ValidationException(ValidationCode.AccountInvalidEmail);

			newAccount.Name = newAccount.Name ?? parsedName ?? string.Empty;
			newAccount.Email = parsedEmail;

			var existing = storage.Accounts.FindByEmail(newAccount.Email);
			if (existing != null)
				throw new AlreadyExistsException();

			var salt = Password.Generate(16);
			var hash = Hash.Calculate(newAccount.Password + salt + PASSWORD_SECRET_SALT);

			var account = new Account()
			{
				Email = newAccount.Email,
				Name = newAccount.Name,
				Salt = salt,
				Password = hash,

				Created = Now(),
				Updated = Now(),
			};

			account = storage.Accounts.Create(account);
			return account;
		}
		Account IServerAccounts.FindByEmail(string email)
		{
			RequireAuthentication();

			if (string.IsNullOrEmpty(email))
				throw new ValidationException(ValidationCode.AccountEmptyEmail);

			if (!Email.TryParse(email, out string parsedEmail, out string parsedName))
				throw new ValidationException(ValidationCode.AccountInvalidEmail);

			return storage.Accounts.FindByEmail(email);
		}
		Account IServerAccounts.ValidateEmail(string email, string password)
		{
			if (string.IsNullOrEmpty(email))
				throw new ValidationException(ValidationCode.AccountEmptyEmail);

			var account = storage.Accounts.FindByEmail(email);

			if (account == null)
				throw new NotFoundException();

			if (account.Password.Trim() != string.Empty && account.Password != Hash.Calculate(password + account.Salt + PASSWORD_SECRET_SALT))
				throw new ValidationException(ValidationCode.AccountInvalidPassword);

			return account;
		}
		Account IServerAccounts.ChangePassword(string password)
		{
			RequireAuthentication();

			var account = this.Accounts.GetCurrent();
			if (account == null) return null;

			account.Salt = Password.Generate(16);
			account.Password = Hash.Calculate(password + account.Salt + PASSWORD_SECRET_SALT);
			account.Updated = Now();
			account = storage.Accounts.Update(account);

			return account;
		}

		#region password salt
		private const string PASSWORD_SECRET_SALT = "3qM8Za4HzEOygASmk+78Pj0n4AEw/AzibEaCg2tjRns=";
		#endregion
	}
}
