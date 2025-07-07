using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	[Serializable]
	public class ValidationException : Exception
	{
		public ValidationCode ValidationCode { get; set; }
		public ValidationException() : base("ValidationException") { }
		public ValidationException(ValidationCode validationCode) : base(validationCode.ToString()) { ValidationCode = validationCode; }
		public ValidationException(ValidationCode validationCode, Exception inner) : base(validationCode.ToString(), inner) { ValidationCode = validationCode; }
		protected ValidationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
