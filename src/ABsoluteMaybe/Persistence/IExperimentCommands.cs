﻿using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExperimentCommands
	{
		ExperimentSummary GetOrCreateExperiment(string experimentName, IEnumerable<string> options);
		ExperimentSummary GetOrCreateExperiment(string experimentName, string conversionKeyword, IEnumerable<string> options);

		ParticipationRecord GetOrCreateParticipationRecord(string experimentName,
		                                                   Func<string> chooseAnOptionForUser,
		                                                   string userId);

		void Convert(string conversionKeyword,
		             string userId);

		void EndExperiment(string experimentName, string alwaysUseOption);
	}
}