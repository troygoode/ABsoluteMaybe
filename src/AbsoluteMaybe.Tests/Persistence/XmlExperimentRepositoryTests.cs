using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ABsoluteMaybe.Persistence;
using NUnit.Framework;
using Should;

namespace AbsoluteMaybe.Tests.Persistence
{
	[TestFixture]
	public class XmlExperimentRepositoryTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_repo = new XmlRepoStub();
		}

		#endregion

		private XmlRepoStub _repo;

		public class XmlRepoStub : XmlExperimentRepository
		{
			public XmlRepoStub() : base(string.Empty)
			{
				Reset();
			}

			public string SavedXml { get; set; }
			public Func<DateTime> UtcNowFactory { get; set; }

			protected override DateTime UtcNow
			{
				get { return UtcNowFactory().ToUniversalTime(); }
			}

			public void Reset()
			{
				UtcNowFactory = () => DateTime.UtcNow;
				SavedXml = null;
			}

			protected override XDocument Load()
			{
				return string.IsNullOrWhiteSpace(SavedXml)
				       	? new XDocument(new XElement("Experiments"))
				       	: XDocument.Parse(SavedXml);
			}

			protected override void Save(XDocument xml)
			{
				SavedXml = xml.ToString();
			}
		}

		[Test]
		public void FindAllExperimentsReturnsAllExperiments()
		{
			//arrange
			_repo.Reset();
			// - experiment one
			_repo.CreateExperiment("Experiment1", new[] { "Experiment One", "Bar" });
			_repo.GetOrCreateParticipationRecord("Experiment1", () => "Experiment One", "User 1");
			// - experiment two
			_repo.CreateExperiment("Experiment2", new[] { "Foo", "Experiment Two" });
			_repo.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", "User 1");
			_repo.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", "User 2");
			// - experiment three
			_repo.CreateExperiment("Experiment3", new[] { "Foo", "Bar" });

			//act
			var result = _repo.FindAllExperiments();

			//assert
			result.Count().ShouldEqual(3);
			result.ElementAt(0).Participants.Count().ShouldEqual(1);
			result.ElementAt(1).Participants.Count().ShouldEqual(2);
			result.ElementAt(2).Participants.Count().ShouldEqual(0);
		}

		[Test]
		public void ConvertMarksOnlyExperimentsWithMatchingNameOrConversionKeyword()
		{
			//arrange
			_repo.Reset();
			const string userId = "USER_123";
			// - experiment one
			_repo.CreateExperiment("CORRECT_CONVERSION_KEYWORD", new[] { "Experiment One", "Bar" });
			_repo.GetOrCreateParticipationRecord("CORRECT_CONVERSION_KEYWORD", () => "Experiment One", userId);
			// - experiment two
			_repo.CreateExperiment("Experiment2", new[] { "Foo", "Experiment Two" });
			_repo.GetOrCreateParticipationRecord("Experiment2", () => "Experiment Two", userId);
			// - experiment three
			_repo.CreateExperiment("Experiment3", "CORRECT_CONVERSION_KEYWORD", new[] { "Experiment Three", "Bar" });
			_repo.GetOrCreateParticipationRecord("Experiment3", () => "Experiment Three", userId);

			//act
			_repo.Convert("CORRECT_CONVERSION_KEYWORD", userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			_repo.UtcNowFactory = () => timestamp;
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });
			_repo.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//act
			_repo.Convert(experimentName, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKeyword = "CONVERT_ON_ME";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, convKeyword, new[] { "Foo", "Bar" });
			_repo.GetOrCreateParticipationRecord(experimentName, () => "Foo", userId);

			//act
			_repo.Convert(convKeyword, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			_repo.UtcNowFactory = () => firstTimestamp;
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, new[]{ "Foo", "Bar" });
			_repo.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);
			_repo.Convert(experimentName, userId);

			//act
			_repo.UtcNowFactory = () => secondTimestamp;
			_repo.Convert(experimentName, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";

			//act
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			exp.Attribute("Ended").ShouldBeNull();
		}

		[Test]
		public void CreateExperimentDoesntOverwriteExperimentIfItAlreadyExists()
		{
			//arrange
			_repo.Reset();
			const string experimentName = "Existing Experiment";
			const string convKeyOld = "CONVERT_ON_ME";
			_repo.CreateExperiment(experimentName, convKeyOld, new[] { "Foo", "Bar" });
			_repo.GetOrCreateParticipationRecord(experimentName, () => "Foo", "USER_1");

			//act
			_repo.CreateExperiment(experimentName, "CONV_KEY_NEW", new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKey = experimentName;

			//act
			_repo.CreateExperiment(experimentName, convKey, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();
			exp.Attribute("ConversionKeyword").ShouldBeNull();
		}

		[Test]
		public void CreateExperimentRecordsTimeOfExperimentCreation()
		{
			//arrange
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			var timestamp = new DateTime(2000, 1, 1);
			_repo.UtcNowFactory = () => timestamp;

			//act
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";

			//act
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string convKey = "CONVERT_ON_ME";

			//act
			_repo.CreateExperiment(experimentName, convKey, new[] { "Foo", "Bar" });

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//act
			var result = _repo.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//assert
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_repo.SavedXml);
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
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });

			//act
			var result = _repo.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//assert
			result.HasConverted.ShouldBeFalse();

			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();
			record.Attribute("HasConverted").ShouldBeNull();
			record.Attribute("DateConverted").ShouldBeNull();
		}

		[Test]
		public void GetOrCreateParticipationRecordReturnsRecordWithPreviouslyAssignedOptionIfAlreadyExists()
		{
			//arrange
			_repo.Reset();
			const string experimentName = "Troy's Experiment";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExperiment(experimentName, new[] { "Foo", "Bar" });
			_repo.GetOrCreateParticipationRecord(experimentName, () => assignedOption, userId);

			//act
			var result = _repo.GetOrCreateParticipationRecord(experimentName, () => "Bar", userId);

			//assert
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Experiment").Single();

			var records = exp.Element("Participants").Elements("Participant");
			records.Count().ShouldEqual(1);

			var record = records.Single();
			record.Value.ShouldEqual(assignedOption);

			var userIdAtt = record.Attribute("Id");
			userIdAtt.ShouldNotBeNull();
			userIdAtt.Value.ShouldEqual(userId);
		}
	}
}