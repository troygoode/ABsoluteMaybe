using System;
using System.Linq;
using ABsoluteMaybe.Domain;
using NUnit.Framework;
using Should;

namespace AbsoluteMaybe.Tests.Domain
{
	[TestFixture]
	public class ExperimentTests
	{
		[Test]
		public void OptionsIncludesOptionValuesThatHaventBeenUsedByAnyParticipants()
		{
			//arrange
			var participants = new[]
			                   	{
			                   		new ParticipationRecord("user1", "OPTION_1", false, null),
									new ParticipationRecord("user2", "OPTION_1", false, null),
			                   	};
			var exp = new Experiment(null, null, null, DateTime.Now, null, participants, new[] { "OPTION_1", "OPTION_2" });

			//act
			var options = exp.Options;

			//assert
			options.Count().ShouldEqual(2);
		}

		[Test]
		public void OptionsAreGroupedProperly()
		{
			//arrange
			var participants = new[]
			                   	{
			                   		new ParticipationRecord("user1", "OPTION_1", false, null),
									new ParticipationRecord("user2", "OPTION_1", false, null),
									new ParticipationRecord("user1", "OPTION_2", false, null),
									new ParticipationRecord("user2", "OPTION_2", false, null),
			                   	};
			var exp = new Experiment(null, null, null, DateTime.Now, null, participants, new[] { "OPTION_1", "OPTION_2" });

			//act
			var options = exp.Options;

			//assert
			options.Count().ShouldEqual(2);
		}

		[Test]
		public void OptionParticipantsAreCountedProperly()
		{
			//arrange
			var participants = new[]
			                   	{
			                   		new ParticipationRecord("user1", "OPTION_1", false, null),
									new ParticipationRecord("user2", "OPTION_1", false, null),
									new ParticipationRecord("user1", "OPTION_2", false, null),
									new ParticipationRecord("user2", "OPTION_2", false, null),
									new ParticipationRecord("user3", "OPTION_2", false, null),
			                   	};
			var exp = new Experiment(null, null, null, DateTime.Now, null, participants, new[] { "OPTION_1", "OPTION_2" });

			//act
			var options = exp.Options;

			//assert
			options.ElementAt(0).Participants.ShouldEqual(2);
			options.ElementAt(1).Participants.ShouldEqual(3);
		}

		[Test]
		public void OptionConversionsAreCountedProperly()
		{
			//arrange
			var participants = new[]
			                   	{
			                   		new ParticipationRecord("user1", "OPTION_1", false, null),
									new ParticipationRecord("user2", "OPTION_1", false, null),
									new ParticipationRecord("user1", "OPTION_2", false, null),
									new ParticipationRecord("user2", "OPTION_2", true, DateTime.Now),
									new ParticipationRecord("user3", "OPTION_2", false, null),
			                   	};
			var exp = new Experiment(null, null, null, DateTime.Now, null, participants, new[] { "OPTION_1", "OPTION_2" });

			//act
			var options = exp.Options;

			//assert
			options.ElementAt(0).Conversions.ShouldEqual(0);
			options.ElementAt(1).Conversions.ShouldEqual(1);
		}

		[Test]
		public void OptionConversionRateIsCountedProperly()
		{
			//arrange
			var participants = new[]
			                   	{
			                   		new ParticipationRecord("user1", "OPTION_1", false, null),
									new ParticipationRecord("user2", "OPTION_1", true, DateTime.Now),
									new ParticipationRecord("user3", "OPTION_1", false, null),
									new ParticipationRecord("user4", "OPTION_1", true, DateTime.Now)
			                   	};
			var exp = new Experiment(null, null, null, DateTime.Now, null, participants, new[] { "OPTION_1", "OPTION_2" });

			//act
			var options = exp.Options;

			//assert
			options.ElementAt(0).ConversionRate.ShouldEqual(.5);
		}
	}
}