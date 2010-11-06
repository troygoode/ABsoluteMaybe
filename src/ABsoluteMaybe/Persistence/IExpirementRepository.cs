using System;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExpirementRepository
	{
		void CreateExpirement(string expirementName);
		void CreateExpirement(string expirementName, string conversionKeyword);

		ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string conversionKeyword,
		             string userId);
	}
}