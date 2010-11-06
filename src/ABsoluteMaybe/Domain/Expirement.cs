using System;
using System.Collections.Generic;
using System.Linq;

namespace ABsoluteMaybe.Domain
{
	public class Expirement
	{
		public Expirement(string name, string conversionKeyword, DateTime dateCreated, DateTime? dateEnded,
		                  IEnumerable<ParticipationRecord> participants)
		{
			Name = name;
			ConversionKeyword = conversionKeyword;
			DateCreated = dateCreated;
			DateEnded = dateEnded;
			Participants = participants;
		}

		public string Name { get; private set; }
		public string ConversionKeyword { get; private set; }
		public DateTime DateCreated { get; private set; }
		public DateTime? DateEnded { get; private set; }
		public IEnumerable<ParticipationRecord> Participants { get; private set; }

		private IEnumerable<Option> _options;
		public IEnumerable<Option> Options
		{
			get
			{
				return _options ?? (_options = Participants
				                               	.GroupBy(p => p.AssignedOption)
				                               	.Select(g => new Option(g.Key,
				                               	                        g.Count(),
				                               	                        g.Count(p => p.HasConverted))));
			}
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