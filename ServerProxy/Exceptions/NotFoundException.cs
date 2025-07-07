using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.ServerProxy
{
	[Serializable]
	public class NotFoundException : Exception
	{
		public NotFoundException() : base("NotFoundException") { }
		public NotFoundException(string message) : base(message) { }
		public NotFoundException(string message, Exception inner) : base(message, inner) { }
		protected NotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
