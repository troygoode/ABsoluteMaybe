using System.Linq;
using NUnit.Framework;
using Should;

namespace ABsoluteMaybe.Tests.Persistence
{
	[TestFixture]
	public class XmlExperimentQueriesTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_commands = new XmlCommandsStub();
		}

		#endregion

		private XmlCommandsStub _commands;

		[Test]
		public void FindAllExperimentsReturnsAllExperiments()
		{
			//arrange
			_commands.Reset();
			// - experiment one
			_commands.GetOrCreateExperiment("Experiment1", new[] { "Experiment One", "Bar" });
			_commands.GetOrCreateParticipationRecord("Experiment1", () => "Experiment One", "User 1");
			// - experiment two
			_commands.GetOrCreateExperiment("Experiment2", new[] { "Foo", "Experiment Two" });
			_commands.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", "User 1");
			_commands.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", "User 2");
			// - experiment three
			_commands.GetOrCreateExperiment("Experiment3", new[] { "Foo", "Bar" });
			var queries = new XmlQueriesStub(_commands.SavedXml);

			//act
			var result = queries.FindAllExperiments();

			//assert
			result.Count().ShouldEqual(3);
			result.ElementAt(0).Participants.Count().ShouldEqual(1);
			result.ElementAt(1).Participants.Count().ShouldEqual(2);
			result.ElementAt(2).Participants.Count().ShouldEqual(0);
		}
	}
}