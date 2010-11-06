using System;
using System.Collections.Generic;
using ABsoluteMaybe.Domain;
using ABsoluteMaybe.Statistics;
using NUnit.Framework;

namespace AbsoluteMaybe.Tests.Statistics
{
	[TestFixture]
	public class ABsoluteMaybeStatisticsTests
	{
		[Test]
		public void ShouldWork()
		{
			//arrange
			var participants = CreateRandomParticipationRecords(100, 200);
			var exp = new Expirement(null, null, DateTime.Now, null, participants);

			//act
			var stats = new ABsoluteMaybeStatistics(exp);
			var formatted = new ABingoStyleFormatter(stats);

			//assert
			var tostring = formatted.ToString();
			var foo = "bar";
		}

		private static IEnumerable<ParticipationRecord> CreateRandomParticipationRecords(int min, int max)
		{
			var rnd = new Random();
			for (var i = 0; i < rnd.Next(min, max); i++)
			{
				var uid = i.ToString();
				var option = (rnd.Next(2) == 0).ToString();
				var converted = rnd.Next(2) == 0;
				yield return new ParticipationRecord(uid, option, converted, converted ? DateTime.Now : (DateTime?)null);
			}
		}
	}
}