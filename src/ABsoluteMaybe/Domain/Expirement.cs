using System;
using System.Collections.Generic;

namespace ABsoluteMaybe.Domain
{
	public class Expirement
	{
		public string Name { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateEnded { get; set; }
		public IEnumerable<Option> Options { get; set; }

		#region Nested type: Option

		public class Option
		{
			public string Value { get; set; }
			public int Participants { get; set; }
			public int Conversions { get; set; }
		}

		#endregion
	}
}