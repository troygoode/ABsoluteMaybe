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
	public class XmlExpirementRepositoryTests
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_repo = new XmlRepoStub();
		}

		#endregion

		private XmlRepoStub _repo;

		public class XmlRepoStub : XmlExpirementRepository
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
				       	? new XDocument(new XElement("Expirements"))
				       	: XDocument.Parse(SavedXml);
			}

			protected override void Save(XDocument xml)
			{
				SavedXml = xml.ToString();
			}
		}

		[Test]
		public void ConvertMarksOnlyExpirementsWithMatchingNameOrConversionKeyword()
		{
			//arrange
			_repo.Reset();
			const string userId = "USER_123";
			// - expirement one
			_repo.CreateExpirement("CORRECT_CONVERSION_KEYWORD");
			_repo.GetOrCreateParticipationRecord("CORRECT_CONVERSION_KEYWORD", () => "Expirement One", userId);
			// - expirement two
			_repo.CreateExpirement("Expirement2");
			_repo.GetOrCreateParticipationRecord("Expirement2", () => "Expirement Two", userId);
			// - expirement two
			_repo.CreateExpirement("Expirement3", "CORRECT_CONVERSION_KEYWORD");
			_repo.GetOrCreateParticipationRecord("Expirement3", () => "Expirement Three", userId);

			//act
			_repo.Convert("CORRECT_CONVERSION_KEYWORD", userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var expirements = xml.Root.Elements("Expirement");
			expirements.Count().ShouldEqual(3);

			var records = expirements.Aggregate(new List<XElement>(),
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
			const string expirementName = "Troy's Expirement";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName);
			_repo.GetOrCreateParticipationRecord(expirementName, () => assignedOption, userId);

			//act
			_repo.Convert(expirementName, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();

			var convertAtt = record.Attribute("HasConverted");
			convertAtt.ShouldNotBeNull();
			bool.Parse(convertAtt.Value).ShouldBeTrue();

			var dateConvertAtt = record.Attribute("DateConverted");
			dateConvertAtt.ShouldNotBeNull();
			DateTime.Parse(dateConvertAtt.Value).ShouldEqual(timestamp);
		}

		[Test]
		public void ConvertMarksParticipantsInRecordsWithConversionKeywordDifferentThanExpirementName()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			const string convKeyword = "CONVERT_ON_ME";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName, convKeyword);
			_repo.GetOrCreateParticipationRecord(expirementName, () => "Foo", userId);

			//act
			_repo.Convert(convKeyword, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

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
			const string expirementName = "Troy's Expirement";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName);
			_repo.GetOrCreateParticipationRecord(expirementName, () => assignedOption, userId);
			_repo.Convert(expirementName, userId);

			//act
			_repo.UtcNowFactory = () => secondTimestamp;
			_repo.Convert(expirementName, userId);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();

			var convertAtt = record.Attribute("HasConverted");
			convertAtt.ShouldNotBeNull();
			bool.Parse(convertAtt.Value).ShouldBeTrue();

			var dateConvertAtt = record.Attribute("DateConverted");
			dateConvertAtt.ShouldNotBeNull();
			DateTime.Parse(dateConvertAtt.Value).ShouldEqual(firstTimestamp);
		}

		[Test]
		public void CreateExpirementDoesNotRecordTimeThatExpirementEnded()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";

			//act
			_repo.CreateExpirement(expirementName);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

			exp.Attribute("Ended").ShouldBeNull();
		}

		[Test]
		public void CreateExpirementDoesntOverwriteExpirementIfItAlreadyExists()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Existing Expirement";
			const string convKeyOld = "CONVERT_ON_ME";
			_repo.CreateExpirement(expirementName, convKeyOld);
			_repo.GetOrCreateParticipationRecord(expirementName, () => "O1", "USER_1");

			//act
			_repo.CreateExpirement(expirementName, "CONV_KEY_NEW");

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var expirements = xml.Root.Elements("Expirement");
			expirements.Count().ShouldEqual(1);

			var exp = expirements.Single();

			var convKeyAtt = exp.Attribute("ConversionKeyword");
			convKeyAtt.ShouldNotBeNull();
			convKeyAtt.Value.ShouldEqual(convKeyOld);

			var records = exp.Element("Participants").Elements("Participant");
			records.Count().ShouldEqual(1);
		}

		[Test]
		public void CreateExpirementDoesntStoreConversionKeywordWhenItIsSameAsExpirementName()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			const string convKey = expirementName;

			//act
			_repo.CreateExpirement(expirementName, convKey);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();
			exp.Attribute("ConversionKeyword").ShouldBeNull();
		}

		[Test]
		public void CreateExpirementRecordsTimeOfExpirementCreation()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			var timestamp = new DateTime(2000, 1, 1);
			_repo.UtcNowFactory = () => timestamp;

			//act
			_repo.CreateExpirement(expirementName);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

			var created = exp.Attribute("Started");
			created.ShouldNotBeNull();

			var createdDate = DateTime.Parse(created.Value);
			createdDate.ShouldEqual(timestamp);
		}

		[Test]
		public void CreateExpirementSavesExpirementWhenItIsntAlreadySaved()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			const string convKey = "CONVERT_ON_ME";

			//act
			_repo.CreateExpirement(expirementName, convKey);

			//assert
			var xml = XDocument.Parse(_repo.SavedXml);
			var expirements = xml.Root.Elements("Expirement");
			expirements.Count().ShouldEqual(1);

			var exp = expirements.Single();

			var nameAtt = exp.Attribute("Name");
			nameAtt.ShouldNotBeNull();
			nameAtt.Value.ShouldEqual(expirementName);

			var convAtt = exp.Attribute("ConversionKeyword");
			convAtt.ShouldNotBeNull();
			convAtt.Value.ShouldEqual(convKey);
		}

		[Test]
		public void GetOrCreateParticipationRecordCreatesRecordAndReturnsItIfNotFound()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName);

			//act
			var result = _repo.GetOrCreateParticipationRecord(expirementName, () => assignedOption, userId);

			//assert
			result.ExpirementName.ShouldEqual(expirementName);
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

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
			const string expirementName = "Troy's Expirement";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName);

			//act
			var result = _repo.GetOrCreateParticipationRecord(expirementName, () => assignedOption, userId);

			//assert
			result.HasConverted.ShouldBeFalse();

			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

			var record = exp.Element("Participants").Elements("Participant").Single();
			record.Attribute("HasConverted").ShouldBeNull();
			record.Attribute("DateConverted").ShouldBeNull();
		}

		[Test]
		public void GetOrCreateParticipationRecordReturnsRecordWithPreviouslyAssignedOptionIfAlreadyExists()
		{
			//arrange
			_repo.Reset();
			const string expirementName = "Troy's Expirement";
			const string assignedOption = "Foo";
			const string userId = "USER_123";
			_repo.CreateExpirement(expirementName);
			_repo.GetOrCreateParticipationRecord(expirementName, () => assignedOption, userId);

			//act
			var result = _repo.GetOrCreateParticipationRecord(expirementName, () => "Bar", userId);

			//assert
			result.ExpirementName.ShouldEqual(expirementName);
			result.UserIdentifier.ShouldEqual(userId);
			result.AssignedOption.ShouldEqual(assignedOption);

			var xml = XDocument.Parse(_repo.SavedXml);
			var exp = xml.Root.Elements("Expirement").Single();

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