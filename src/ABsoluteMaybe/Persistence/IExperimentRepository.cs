using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExperimentRepository
	{
		IEnumerable<Experiment> FindAllExperiments();

		void CreateExperiment(string experimentName, IEnumerable<string> options);
		void CreateExperiment(string experimentName, string conversionKeyword, IEnumerable<string> options);

		ParticipationRecord GetOrCreateParticipationRecord(string experimentName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string conversionKeyword,
		             string userId);

		void EndExperiment(string experimentName, string finalOption);
	}
}