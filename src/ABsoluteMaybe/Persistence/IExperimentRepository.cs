using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExperimentRepository
	{
		IEnumerable<Experiment> FindAllExperiments();

		void CreateExperiment(string experimentName);
		void CreateExperiment(string experimentName, string conversionKeyword);

		ParticipationRecord GetOrCreateParticipationRecord(string experimentName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string conversionKeyword,
		             string userId);
	}
}