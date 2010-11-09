using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Should;

namespace ABsoluteMaybe.Tests.Persistence
{
	[TestFixture]
	public class XmlExperimentCommandsTests
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
		public void ConvertMarksOnlyExperimentsWithMatchingNameOrConversionKeyword()
		{
			//arrange
			_commands.Reset();
			const string userId = "USER_123";
			// - experiment one
			_commands.GetOrCreateExperiment("CORRECT_CONVERSION_KEYWORD", new[] { "Experiment One", "Bar" });
			_commands.GetOrCreateParticipationRecord("CORRECT_CONVERSION_KEYWORD", () => "Experiment One", userId);
			// - experiment two
			_commands.GetOrCreateExperiment("Experiment2", new[] { "Foo", "Experiment Two" });
			_commands.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", userId);
			// - experiment three
			_commands.GetOrCreateExperiment("Experiment3", "CORRECT_CONVERSION_KEYWORD", new[] { "Experiment Three", "Bar" });
			_commands.GetOrCreateParticipationRecord("Experiment3", () => "Experiment Three", userId);

			//act
			_commands.Convert("CORRECT_CONVERSION_KEYWORD", userId);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var experiments = xml.Root.Elements("Experiment");
			experiments.Count().ShouldEqual(3);

			var records = experiments.Aggregate(new List<XElement>(),
			                                    (list, exp) =>
			                                    	{
			                                    		var participants =
			                                    			exp.Element("Participants").Elements("Participant").Where(
			                                    				x =>
			                                    				x.Attribute("HasConverted") != null &&
			                                    				bool.Parse(x.Attribute("HasConverted").Value));
														list.AddRange(participants);
			                                    		return list;
			                                    	},
			                                    list => list);
			records.Count().ShouldEqual(2);
		}

		[Test]
		public void ConvertMarksParticipantAsHavingConverted()
		{
			//arrange
			var timestamp = new DateTime(2000, 1, 1);
			_commands.Reset();
			_commands.UtcNowFactory = () => timestamp;
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });
			_commands.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//act
			_commands.Convert(experimentName, userId);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();

			var convertAtt = record.Attribute("HasConverted");
			convertAtt.ShouldNotBeNull();
			bool.Parse(convertAtt.Value).ShouldBeTrue();

			var dateConvertAtt = record.Attribute("DateConverted");
			dateConvertAtt.ShouldNotBeNull();
			DateTime.Parse(dateConvertAtt.Value).ShouldEqual(timestamp);
		}

		[Test]
		public void ConvertMarksParticipantsInRecordsWithConversionKeywordDifferentThanExperimentName()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKeyword = "CONVERT_ON_ME";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, convKeyword, new[] { "Foo", "Bar" });
			_commands.GetOrCreateParticipationRecord(experimentName, () => "Foo", userId);

			//act
			_commands.Convert(convKeyword, userId);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();

			var convertAtt = record.Attribute("HasConverted");
			convertAtt.ShouldNotBeNull();
			bool.Parse(convertAtt.Value).ShouldBeTrue();
		}

		[Test]
		public void ConvertOnlyUpdatesParticipantRecordIfParticipantHasntYetConverted()
		{
			//arrange
			var firstTimestamp = new DateTime(2000, 1, 1);
			var secondTimestamp = new DateTime(2001, 1, 1);
			_commands.Reset();
			_commands.UtcNowFactory = () => firstTimestamp;
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, new[]{ "Foo", "Bar" });
			_commands.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);
			_commands.Convert(experimentName, userId);

			//act
			_commands.UtcNowFactory = () => secondTimestamp;
			_commands.Convert(experimentName, userId);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();

			var convertAtt = record.Attribute("HasConverted");
			convertAtt.ShouldNotBeNull();
			bool.Parse(convertAtt.Value).ShouldBeTrue();

			var dateConvertAtt = record.Attribute("DateConverted");
			dateConvertAtt.ShouldNotBeNull();
			DateTime.Parse(dateConvertAtt.Value).ShouldEqual(firstTimestamp);
		}

		[Test]
		public void CreateExperimentDoesNotRecordTimeThatExperimentEnded()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";

			//act
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			exp.Attribute("Ended").ShouldBeNull();
		}

		[Test]
		public void CreateExperimentDoesntOverwriteExperimentIfItAlreadyExists()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Existing Experiment";
			const string convKeyOld = "CONVERT_ON_ME";
			_commands.GetOrCreateExperiment(experimentName, convKeyOld, new[] { "Foo", "Bar" });
			_commands.GetOrCreateParticipationRecord(experimentName, () => "Foo", "USER_1");

			//act
			_commands.GetOrCreateExperiment(experimentName, "CONV_KEY_NEW", new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var experiments = xml.Root.Elements("Experiment");
			experiments.Count().ShouldEqual(1);

			var exp = experiments.Single();

			var convKeyAtt = exp.Attribute("ConversionKeyword");
			convKeyAtt.ShouldNotBeNull();
			convKeyAtt.Value.ShouldEqual(convKeyOld);

			var records = exp.Element("Participants").Elements("Participant");
			records.Count().ShouldEqual(1);
		}

		[Test]
		public void CreateExperimentDoesntStoreConversionKeywordWhenItIsSameAsExperimentName()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKey = experimentName;

			//act
			_commands.GetOrCreateExperiment(experimentName, convKey, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();
			exp.Attribute("ConversionKeyword").ShouldBeNull();
		}

		[Test]
		public void CreateExperimentRecordsTimeOfExperimentCreation()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			var timestamp = new DateTime(2000, 1, 1);
			_commands.UtcNowFactory = () => timestamp;

			//act
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var created = exp.Attribute("Started");
			created.ShouldNotBeNull();

			var createdDate = DateTime.Parse(created.Value);
			createdDate.ShouldEqual(timestamp);
		}

		[Test]
		public void CreateExperimentRecordsAllPossibleOptionValues()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";

			//act
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var possibleOptionValues = exp.Element("PossibleOptionValues");
			possibleOptionValues.ShouldNotBeNull();
			possibleOptionValues.Elements("Option").Count().ShouldEqual(2);
			possibleOptionValues.Elements("Option").ElementAt(0).Value.ShouldEqual("Foo");
			possibleOptionValues.Elements("Option").ElementAt(1).Value.ShouldEqual("Bar");
		}

		[Test]
		public void CreateExperimentSavesExperimentWhenItIsntAlreadySaved()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKey = "CONVERT_ON_ME";

			//act
			_commands.GetOrCreateExperiment(experimentName, convKey, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var experiments = xml.Root.Elements("Experiment");
			experiments.Count().ShouldEqual(1);

			var exp = experiments.Single();

			var nameAtt = exp.Attribute("Name");
			nameAtt.ShouldNotBeNull();
			nameAtt.Value.ShouldEqual(experimentName);

			var convAtt = exp.Attribute("ConversionKeyword");
			convAtt.ShouldNotBeNull();
			convAtt.Value.ShouldEqual(convKey);
		}

		[Test]
		public void GetOrCreateParticipationRecordCreatesRecordAndReturnsItIfNotFound()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//act
			var result = _commands.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//assert
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var records = exp.Element("Participants").Elements("Participant");
			records.Count().ShouldEqual(1);

			var record = records.Single();
			record.Value.ShouldEqual(assignedOption);

			var userIdAtt = record.Attribute("Id");
			userIdAtt.ShouldNotBeNull();
			userIdAtt.Value.ShouldEqual(userId);
		}

		[Test]
		public void GetOrCreateParticipationRecordDoesNotMarkRecordAsConvertedUponFirstCreation()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//act
			var result = _commands.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//assert
			result.HasConverted.ShouldBeFalse();

			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();
			record.Attribute("HasConverted").ShouldBeNull();
			record.Attribute("DateConverted").ShouldBeNull();
		}

		[Test]
		public void GetOrCreateParticipationRecordReturnsRecordWithPreviouslyAssignedOptionIfAlreadyExists()
		{
			//arrange
			_commands.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });
			_commands.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//act
			var result = _commands.GetOrCreateParticipationRecord(experimentName, () => "Bar", userId);

			//assert
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var records = exp.Element("Participants").Elements("Participant");
			records.Count().ShouldEqual(1);

			var record = records.Single();
			record.Value.ShouldEqual(assignedOption);

			var userIdAtt = record.Attribute("Id");
			userIdAtt.ShouldNotBeNull();
			userIdAtt.Value.ShouldEqual(userId);
		}

		[Test]
		public void EndExperimentMarksExperimentsEndDate()
		{
			//arrange
			const string experimentName = "Troy's Experiment";
			const string alwaysUseOption = "Bar";
			_commands.Reset();
			_commands.GetOrCreateExperiment(experimentName, new[]{ "Foo", "Bar" });
			var timestamp = new DateTime(2008, 5, 24);
			_commands.UtcNowFactory = () => timestamp;

			//act
			_commands.EndExperiment(experimentName, alwaysUseOption);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var endDateAtt = exp.Attribute("Ended");
			endDateAtt.ShouldNotBeNull();
			DateTime.Parse(endDateAtt.Value).ShouldEqual(timestamp);
		}

		[Test]
		public void EndExperimentShouldNotOverwriteExistingEndDate()
		{
			//arrange
			const string experimentName = "Troy's Experiment";
			const string alwaysUseOption = "Bar";
			_commands.Reset();
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });
			var timestamp1 = new DateTime(2008, 5, 24);
			_commands.UtcNowFactory = () => timestamp1;
			_commands.EndExperiment(experimentName, alwaysUseOption);

			//act
			var timestamp2 = new DateTime(2008, 5, 25);
			_commands.UtcNowFactory = () => timestamp2;
			_commands.EndExperiment(experimentName, alwaysUseOption);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			DateTime.Parse(exp.Attribute("Ended").Value).ShouldEqual(timestamp1);
		}

		[Test]
		public void EndExperimentMarksAlwaysUseOption()
		{
			//arrange
			const string experimentName = "Troy's Experiment";
			const string alwaysUseOption = "Bar";
			_commands.Reset();
			_commands.GetOrCreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//act
			_commands.EndExperiment(experimentName, alwaysUseOption);

			//assert
			var xml = XDocument.Parse(_commands.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var alwaysUseOptionAtt = exp.Attribute("AlwaysUseOption");
			alwaysUseOptionAtt.ShouldNotBeNull();
			alwaysUseOptionAtt.Value.ShouldEqual(alwaysUseOption);
		}
	}
}