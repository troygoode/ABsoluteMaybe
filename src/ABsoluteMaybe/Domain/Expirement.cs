using System;
using System.Collections.Generic;

namespace ABsoluteMaybe.Domain
{
	public class Expirement
	{
		public Expirement(string name, string conversionKeyword, DateTime dateCreated, DateTime? dateEnded, IEnumerable<ParticipationRecord> participants)
		{
			Name = name;
			ConversionKeyword = conversionKeyword;
			DateCreated = dateCreated;
			DateEnded = dateEnded;
			Participants = participants;
		}

		public string Name { get; private set; }
		public string ConversionKeyword { get; private set; }
		public DateTime DateCreated { get; private set; }
		public DateTime? DateEnded { get; private set; }
		public IEnumerable<ParticipationRecord> Participants { get; private set; }
	}
}