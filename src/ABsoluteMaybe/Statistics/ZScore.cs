using System;
using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Statistics
{
	public class ZScore
	{
		private readonly IEnumerable<Expirement.Option> _options;

		public ZScore(IEnumerable<Expirement.Option> options)
		{
			_options = options;
		}

		public double Execute()
		{
			if (_options.Count() != 2)
				throw new Exception("Sorry, can't currently automatically calculate statistics for A/B tests with > 2 alternatives.");

			var option1 = _options.ElementAt(0);
			var option2 = _options.ElementAt(1);

			if (option1.Participants == 0 || option2.Participants == 0)
				throw new Exception("Can't calculate the z score if either of the alternatives lacks participants.");

			var numerator = option1.ConversionRate - option2.ConversionRate;
			var frac1 = option1.ConversionRate * (1 - option1.ConversionRate) / option1.Participants;
			var frac2 = option2.ConversionRate * (1 - option2.ConversionRate) / option2.Participants;

			return numerator/Math.Pow((frac1 + frac2), 0.5);
		}
	}
}