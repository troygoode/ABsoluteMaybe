using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExpirementRepository
	{
		IEnumerable<Expirement> FindAllExpirements();

		void CreateExpirement(string expirementName);
		void CreateExpirement(string expirementName, string conversionKeyword);

		ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string conversionKeyword,
		             string userId);
	}
}