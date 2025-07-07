using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.WebApi
{
	public interface ICheckAPI
	{
		CheckResponse Create(CreateCheckRequest request);
		CheckResponse Confirm(int groupID, int checkID);
		CheckResponse Update(UpdateCheckRequest request);
		void Delete(int groupID, int checkID);
	}

	public class CheckResponse
	{
		public int GroupID { get; set; }
		public int CheckID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Debitors { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Creditors { get; set; }
		public DateTime Created { get; set; }
		public int CreatedByID { get; set; }
		public DateTime Updated { get; set; }
		public int UpdatedByID { get; set; }
		public long Revision { get; set; }
	}

	public class CreateCheckRequest
	{
		public int GroupID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Debitors { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Creditors { get; set; }
	}

	public class UpdateCheckRequest
	{
		public int GroupID { get; set; }
		public int CheckID { get; set; }
		public DateTimeOffset DateAndTime { get; set; }
		public string Description { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Debitors { get; set; }
		/// <summary>
		/// Format: MemberID:Summ,MemberID:Summ...
		/// </summary>
		public string Creditors { get; set; }
	}
}
