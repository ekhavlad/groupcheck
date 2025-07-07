using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using GroupCheck.Server;
using GroupCheck.WebApi;
using System.Net;
using GroupCheck.WebServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GroupCheck.WebServer.Filters
{
	public class ManagedExceptionFilter : Attribute, IExceptionFilter
	{
		private readonly ILogger _logger;
		public ManagedExceptionFilter(ILogger logger)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			var exception = context.Exception;

			_logger.Info(exception);

			context.ExceptionHandled = true;

			if (exception is AuthenticationRequiredException)
			{
				context.Result = CreateResult(StatusCodes.Status401Unauthorized);
			}
			else if (exception is AccessDeniedException)
			{
				context.Result = CreateResult(StatusCodes.Status403Forbidden);
			}
			else if (exception is AccountBlockedException)
			{
				context.Result = CreateResult(StatusCodes.Status403Forbidden, ResponseCode.ACCOUNT_BANNED);
			}
			else if (exception is NotFoundException)
			{
				context.Result = CreateResult(StatusCodes.Status404NotFound);
			}
			else if (exception is DeletedException)
			{
				context.Result = CreateResult(StatusCodes.Status410Gone);
			}
			else if (exception is AlreadyExistsException)
			{
				context.Result = CreateResult(StatusCodes.Status422UnprocessableEntity, ResponseCode.ALREADY_EXISTS);
			}
			else if (exception is ValidationException)
			{
				context.Result = CreateResult(StatusCodes.Status422UnprocessableEntity, GetValidationErrorCode((ValidationException)exception));
				_logger.Warning(exception);
			}
			else if (exception is APIManagedException)
			{
				context.Result = CreateResult(StatusCodes.Status422UnprocessableEntity, exception.Message);
				_logger.Warning(exception);
			}
			else
			{
				context.Exception = new Exception(context.ActionDescriptor.DisplayName, context.Exception);
				context.ExceptionHandled = false;
			}
		}

		private IActionResult CreateResult(int statusCode, string message = null)
		{
			return new ContentResult()
			{
				StatusCode = statusCode,
				ContentType = System.Net.Mime.MediaTypeNames.Text.Plain,
				Content = message
			};
		}

		private static readonly Dictionary<ValidationCode, string> validationErrors = new Dictionary<ValidationCode, string>()
		{
			{ ValidationCode.Common, ResponseCode.UNDEFINED_ERROR },

			{ ValidationCode.AccountEmptyUID, ResponseCode.EMPTY_UID },

			{ ValidationCode.AccountEmptyEmail, ResponseCode.EMPTY_EMAIL },
			{ ValidationCode.AccountEmptyPassword, ResponseCode.EMPTY_PASSWORD },
			{ ValidationCode.AccountInvalidEmail, ResponseCode.INVALID_EMAIL },
			{ ValidationCode.AccountInvalidPhone, ResponseCode.INVALID_PHONE },
			{ ValidationCode.AccountInvalidPassword, ResponseCode.INVALID_PASSWORD },

			{ ValidationCode.GroupEmptyName, ResponseCode.EMPTY_GROUP_NAME },
			{ ValidationCode.MemberEmptyName, ResponseCode.EMPTY_MEMBER_NAME },

			{ ValidationCode.DuplicateAccounts, ResponseCode.DUPLICATE_ACCOUNTS },
			{ ValidationCode.YouMustBeMemberOfGroup, ResponseCode.YOU_MUST_BE_MEMBER_OF_GROUP },
		};

		private static string GetValidationErrorCode(ValidationException exception)
		{
			return GetValidationErrorCode(exception.ValidationCode);
		}

		private static string GetValidationErrorCode(ValidationCode serverValidationCode)
		{
			if (!validationErrors.ContainsKey(serverValidationCode))
				return ResponseCode.UNDEFINED_ERROR;

			return validationErrors[serverValidationCode];
		}
	}
}
