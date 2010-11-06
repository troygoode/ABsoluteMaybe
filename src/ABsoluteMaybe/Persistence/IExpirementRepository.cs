using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExpirementRepository
	{
		Expirement GetOrCreateExpirement(string expirementName,
		                                 IEnumerable<string> options);

		ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string expirementName,
		             string userId);
	}
}