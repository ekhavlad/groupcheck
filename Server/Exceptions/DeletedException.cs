using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.Server
{
	[Serializable]
	public class DeletedException : Exception
	{
		public DeletedException() : base("DeletedException") { }
		public DeletedException(string message) : base(message) { }
		public DeletedException(string message, Exception inner) : base(message, inner) { }
		protected DeletedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
