using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GroupCheck.WebApi;
using GroupCheck.Server;

namespace GroupCheck.WebServer
{
	public class CheckController : BaseApiController, ICheckAPI
	{
		[HttpPost]
		public CheckResponse Create([FromBody] CreateCheckRequest request)
		{
			var newCheck = new NewCheck()
			{
				GroupID = request.GroupID,
				Description = request.Description,
				DateAndTime = request.DateAndTime,
				Debitors = DeserializeMembers(request.Debitors),
				Creditors = DeserializeMembers(request.Creditors)
			};
			var check = _server.Checks.Create(newCheck);
			var response = GetCheckResponse(check);
			return response;
		}

		[HttpGet("Check/Confirm/{groupID}/{checkID}")]
		public CheckResponse Confirm(int groupID, int checkID)
		{
			var check = _server.Checks.Confirm(groupID, checkID);
			var response = GetCheckResponse(check);
			return response;
		}

		[HttpPost]
		public CheckResponse Update([FromBody] UpdateCheckRequest request)
		{
			var updateCheck = new UpdateCheck()
			{
				CheckID = request.CheckID,
				DateAndTime = request.DateAndTime,
				Description = request.Description,
				Debitors = DeserializeMembers(request.Debitors),
				Creditors = DeserializeMembers(request.Creditors)
			};
			var check = _server.Checks.Update(updateCheck);
			var response = GetCheckResponse(check);
			return response;
		}

		[HttpDelete("Check/Delete/{groupID}/{checkID}")]
		public void Delete(int groupID, int checkID)
		{
			_server.Checks.Delete(groupID, checkID);
		}

		#region ctor
		public CheckController(IServer server) : base(server) { }
		#endregion

		private static CheckResponse GetCheckResponse(Check check)
		{
			return new CheckResponse()
			{
				CheckID = check.CheckID,
				GroupID = check.GroupID,
				DateAndTime = check.DateAndTime,
				Description = check.Description,
				Creditors = SerializeMembers(check.Creditors),
				Debitors = SerializeMembers(check.Debitors),
				Created = check.Created,
				CreatedByID = check.CreatedByID,
				Updated = check.Updated,
				UpdatedByID = check.UpdatedByID,
				Revision = check.Revision,
			};
		}

		private static string SerializeMembers(IDictionary<int,int> members)
		{
			return members == null ? string.Empty : string.Join(",", members.Select(m => $"{m.Key}:{m.Value}"));
		}
		private static IDictionary<int, int> DeserializeMembers(string members)
		{
			return members == null ?
				new Dictionary<int,int>() :
				members.Split(',').ToDictionary(
							x => int.Parse(x.Split(':')[0]),
							x => int.Parse(x.Split(':')[1])); 
			
		}
	}
}
