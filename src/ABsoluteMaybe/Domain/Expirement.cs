using System;
using System.Collections.Generic;

namespace ABsoluteMaybe.Domain
{
	public class Expirement
	{
		public string Name { get; set; }
		public string ConversionKeyword { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateEnded { get; set; }

		public IEnumerable<ParticipationRecord> Participants { get; set; }
	}
}