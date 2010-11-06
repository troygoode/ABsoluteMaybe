using System;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExpirementRepository
	{
		void CreateExpirement(string expirementName);

		ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string expirementName,
		             string userId);
	}
}