using System;
using System.Collections.Generic;
using System.Linq;

namespace ABsoluteMaybe.Domain
{
	public class Experiment
	{
		private readonly IEnumerable<string> _allPossibleOptionValues;
		private IEnumerable<Option> _options;

		public Experiment(string name,
		                  string conversionKeyword,
		                  string alwaysUseOption,
		                  DateTime dateStarted,
		                  DateTime? dateEnded,
		                  IEnumerable<ParticipationRecord> participants,
		                  IEnumerable<string> allPossibleOptionValues)
		{
			_allPossibleOptionValues = allPossibleOptionValues;
			Name = name;
			ConversionKeyword = conversionKeyword;
			AlwaysUseOption = alwaysUseOption;
			DateStarted = dateStarted;
			DateEnded = dateEnded;
			Participants = participants;
		}

		public string Name { get; private set; }
		public string ConversionKeyword { get; private set; }
		public string AlwaysUseOption { get; private set; }
		public DateTime DateStarted { get; private set; }
		public DateTime? DateEnded { get; private set; }
		public IEnumerable<ParticipationRecord> Participants { get; private set; }

		public IEnumerable<Option> Options
		{
			get
			{
				if (_options != null)
					return _options;

				var options = Participants
					.GroupBy(p => p.AssignedOption)
					.Select(g => new Option(g.Key,
					                        g.Count(),
					                        g.Count(p => p.HasConverted)))
					.ToList();
				var unincludedOptionValues = _allPossibleOptionValues.Where(pov => !options.Select(o => o.Name).Contains(pov));
				options.AddRange(unincludedOptionValues.Select(uov => new Option(uov, 0, 0)));

				return _options = options;
			}
		}

		public int TotalParticipants
		{
			get { return _options.Select(o => o.Participants).Sum(); }
		}

		public int TotalConversions
		{
			get { return _options.Select(o => o.Conversions).Sum(); }
		}

		#region Nested type: Option

		public class Option
		{
			internal Option(string name, int participants, int conversions)
			{
				Name = name;
				Participants = participants;
				Conversions = conversions;
			}

			public string Name { get; private set; }
			public int Participants { get; private set; }
			public int Conversions { get; private set; }

			public double ConversionRate
			{
				get { return (double) Conversions/Participants; }
			}
		}

		#endregion
	}
}