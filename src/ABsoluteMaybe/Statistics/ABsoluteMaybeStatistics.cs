using System.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Statistics
{
	public class ABsoluteMaybeStatistics
	{
		private readonly Expirement _expirement;

		public ABsoluteMaybeStatistics(Expirement expirement)
		{
			_expirement = expirement;
		}

		public ABsoluteMaybeStatisticsResult Execute()
		{
			var zscore = new ZScore(_expirement.Options);
			var pvalue = new PValue(zscore.Execute()).Execute();

			var insufficientSampleSize = _expirement.Options.Any(o => o.Participants < 10);

			var best = _expirement.Options.OrderByDescending(o => o.ConversionRate).First();
			var worst = _expirement.Options.OrderByDescending(o => o.ConversionRate).Last();

			return new ABsoluteMaybeStatisticsResult(insufficientSampleSize, 1 - pvalue, best, worst);
		}

		#region Nested type: ABsoluteMaybeStatisticsResult

		public class ABsoluteMaybeStatisticsResult
		{
			public ABsoluteMaybeStatisticsResult(bool insufficientSampleSize,
			                                     double confidenceLevel,
			                                     Expirement.Option bestOption,
			                                     Expirement.Option worstOption)
			{
				InsufficientSampleSize = insufficientSampleSize;
				ConfidenceLevel = confidenceLevel;
				BestOption = bestOption;
				WorstOption = worstOption;
			}

			public bool InsufficientSampleSize { get; private set; }
			public double ConfidenceLevel { get; private set; }
			public Expirement.Option BestOption { get; private set; }
			public Expirement.Option WorstOption { get; private set; }
		}

		#endregion
	}
}