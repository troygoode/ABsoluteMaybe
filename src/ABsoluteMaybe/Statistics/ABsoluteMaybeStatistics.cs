using System.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Statistics
{
	public class ABsoluteMaybeStatistics
	{
		private readonly Experiment _experiment;

		public ABsoluteMaybeStatistics(Experiment experiment)
		{
			_experiment = experiment;
		}

		public ABsoluteMaybeStatisticsResult Execute()
		{
			var zscore = new ZScore(_experiment.Options);
			var pvalue = new PValue(zscore.Execute()).Execute();

			var insufficientSampleSize = _experiment.Options.Any(o => o.Participants < 10);

			var best = _experiment.Options.OrderByDescending(o => o.ConversionRate).ThenByDescending(o => o.Conversions).First();
			var worst = _experiment.Options.OrderByDescending(o => o.ConversionRate).ThenByDescending(o => o.Conversions).Last();

			return new ABsoluteMaybeStatisticsResult(insufficientSampleSize, 1 - pvalue, best, worst);
		}

		#region Nested type: ABsoluteMaybeStatisticsResult

		public class ABsoluteMaybeStatisticsResult
		{
			public ABsoluteMaybeStatisticsResult(bool insufficientSampleSize,
			                                     double confidenceLevel,
			                                     Experiment.Option bestOption,
			                                     Experiment.Option worstOption)
			{
				InsufficientSampleSize = insufficientSampleSize;
				ConfidenceLevel = confidenceLevel;
				BestOption = bestOption;
				WorstOption = worstOption;
			}

			public bool InsufficientSampleSize { get; private set; }
			public double ConfidenceLevel { get; private set; }
			public Experiment.Option BestOption { get; private set; }
			public Experiment.Option WorstOption { get; private set; }
		}

		#endregion
	}
}