using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.ServerProxy
{
	[Serializable]
	public class AuthenticationRequiredException : Exception
	{
		public AuthenticationRequiredException() : base("AuthenticationRequiredException") { }
		public AuthenticationRequiredException(string message) : base(message) { }
		public AuthenticationRequiredException(string message, Exception inner) : base(message, inner) { }
		protected AuthenticationRequiredException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
