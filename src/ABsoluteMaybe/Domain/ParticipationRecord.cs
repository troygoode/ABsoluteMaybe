using System;

namespace ABsoluteMaybe.Domain
{
	public class ParticipationRecord
	{
		public ParticipationRecord(string userIdentifier, string assignedOption, bool hasConverted, DateTime? dateConverted)
		{
			UserIdentifier = userIdentifier;
			AssignedOption = assignedOption;
			HasConverted = hasConverted;
			DateConverted = dateConverted;
		}

		public string UserIdentifier { get; private set; }
		public string AssignedOption { get; private set; }
		public bool HasConverted { get; private set; }
		public DateTime? DateConverted { get; private set; }
	}
}