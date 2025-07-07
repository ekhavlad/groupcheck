using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	[Serializable]
	public class AccessDeniedException : Exception
	{
		public AccessDeniedException() : base("AccessDeniedException") { }
		public AccessDeniedException(string message) : base(message) { }
		public AccessDeniedException(string message, Exception inner) : base(message, inner) { }
		protected AccessDeniedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
