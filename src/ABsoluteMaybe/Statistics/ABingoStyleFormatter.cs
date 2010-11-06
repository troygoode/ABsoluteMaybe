using System.Collections.Generic;
using System.Text;

namespace ABsoluteMaybe.Statistics
{
	public class ABingoStyleFormatter
	{
		private static readonly Dictionary<double, string> Descriptions = new Dictionary<double, string>
		                                                                  	{
		                                                                  		{0.9, "fairly confident"},
		                                                                  		{0.95, "confident"},
		                                                                  		{0.99, "very confident"},
		                                                                  		{0.999, "extremely confident"}
		                                                                  	};

		private readonly ABsoluteMaybeStatistics _statistics;

		public ABingoStyleFormatter(ABsoluteMaybeStatistics statistics)
		{
			_statistics = statistics;
		}

		public override string ToString()
		{
			var retval = new StringBuilder();
			ABsoluteMaybeStatistics.ABsoluteMaybeStatisticsResult result;
			try
			{
				result = _statistics.Execute();
			}
			catch (ABsoluteMaybeException e)
			{
				return e.Message;
			}

			if (result.InsufficientSampleSize)
				retval.Append("Take these results with a grain of salt since your samples are so small: ");

			retval.AppendFormat(
				@"
				The best alternative you have is: [{0}], which had 
				{1} conversions from {2} participants 
				({3}).  The other alternative was [{4}], 
				which had {5} conversions from {6} participants 
				({7}).  ",
				result.BestOption.Name,
				result.BestOption.Conversions,
				result.BestOption.Participants,
				result.BestOption.ConversionRate.ToString("#0.##%"),
				result.WorstOption.Name,
				result.WorstOption.Conversions,
				result.WorstOption.Participants,
				result.WorstOption.ConversionRate.ToString("#0.##%")
				);

			if (result.ConfidenceLevel == 0)
				retval.Append("However, this difference is not statistically significant.");
			else
				retval.AppendFormat(
					@"
					This difference is <b>{0} likely to be statistically significant</b>, which means you can be 
					{1} that it is the result of your alternatives actually mattering, rather than 
					being due to random chance.  However, this statistical test can't measure how likely the currently 
					observed magnitude of the difference is to be accurate or not.  It only says ""better"", not ""better 
					by so much"".  ",
					result.ConfidenceLevel.ToString("#0.##%"),
					Descriptions[result.ConfidenceLevel]
					);

			return retval.ToString();
		}
	}
}