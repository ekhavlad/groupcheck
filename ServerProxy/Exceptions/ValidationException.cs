﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupCheck.ServerProxy
{
	[Serializable]
	public class ValidationException : Exception
	{
		public ValidationException() : base("ValidationException") { }
		public ValidationException(string message) : base(message) { }
		public ValidationException(string message, Exception inner) : base(message, inner) { }
		protected ValidationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
