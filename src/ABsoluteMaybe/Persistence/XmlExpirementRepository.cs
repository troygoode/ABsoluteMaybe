using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public class XmlExpirementRepository : IExpirementRepository
	{
		private readonly string _pathToXmlStorage;

		public XmlExpirementRepository(string pathToXmlStorage)
		{
			_pathToXmlStorage = pathToXmlStorage;
		}

		protected virtual DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}

		#region IExpirementRepository Members

		public IEnumerable<Expirement> FindAllExpirements()
		{
			var xml = Load();

			return xml.Root.Elements("Expirement")
				.Select(exp => new Expirement
								{
									Name = exp.Attribute("Name").Value,
									ConversionKeyword = exp.Attribute("ConversionKeyword") == null
															? exp.Attribute("Name").Value
															: exp.Attribute("ConversionKeyword").Value,
									DateCreated = DateTime.Parse(exp.Attribute("Started").Value),
									DateEnded = exp.Attribute("Ended") == null
													? (DateTime?)null
													: DateTime.Parse(exp.Attribute("Ended").Value),
									Participants = exp.Element("Participants") == null
													? Enumerable.Empty<ParticipationRecord>()
													: exp.Element("Participants")
														.Elements("Participant")
														.Select(p => new ParticipationRecord
																		{
																			UserIdentifier = p.Attribute("Id").Value,
																			AssignedOption = p.Value,
																			HasConverted = p.Attribute("HasConverted") == null
																							? false
																							: bool.Parse(p.Attribute("HasConverted").Value),
																			DateConverted = exp.Attribute("DateConverted") == null
																								? (DateTime?)null
																								: DateTime.Parse(
																									p.Attribute("DateConverted").Value)
																		})
								});
		}

		public void CreateExpirement(string expirementName)
		{
			CreateExpirement(expirementName, expirementName);
		}

		public void CreateExpirement(string expirementName, string conversionKeyword)
		{
			var xml = Load();

			if (xml.Root.Elements("Expirement").Any(x => x.Attribute("Name").Value == expirementName))
				return;

			var exp = new XElement("Expirement",
			                       new XAttribute("Name", expirementName),
			                       new XAttribute("Started", UtcNow)
				);
			if(expirementName != conversionKeyword)
				exp.Add(new XAttribute("ConversionKeyword", conversionKeyword));
			xml.Root.Add(exp);

			Save(xml);
		}

		public ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                          Func<string> chooseAnOptionForUser,
		                                                          string userId)
		{
			var xml = Load();

			var expirement = xml.Root.Elements("Expirement").Single(x => x.Attribute("Name").Value == expirementName);
			if (expirement.Element("Participants") == null)
				expirement.Add(new XElement("Participants"));

			var participants = expirement.Element("Participants");
			var existingRecord = participants.Elements("Participant").SingleOrDefault(x => x.Attribute("Id").Value == userId);
			if (existingRecord != null)
				return new ParticipationRecord
				       	{
				       		UserIdentifier = existingRecord.Attribute("Id").Value,
				       		AssignedOption = existingRecord.Value,
				       		HasConverted = existingRecord.Attribute("HasConverted") == null
				       		               	? false
				       		               	: bool.Parse(existingRecord.Attribute("HasConverted").Value),
				       		DateConverted = existingRecord.Attribute("DateConverted") == null
				       		                	? (DateTime?) null
				       		                	: DateTime.Parse(existingRecord.Attribute("DateConverted").Value)
				       	};

			var assignedOption = chooseAnOptionForUser();
			expirement.Element("Participants").Add(new XElement("Participant",
			                                                    new XAttribute("Id", userId),
			                                                    new XCData(assignedOption)));

			Save(xml);
			return new ParticipationRecord
			       	{
			       		UserIdentifier = userId,
			       		AssignedOption = assignedOption,
			       		HasConverted = false
			       	};
		}

		public void Convert(string conversionKeyword,
		                    string userId)
		{
			var xml = Load();

			var utcNow = UtcNow;
			var expirements = xml.Root.Elements("Expirement")
				.Where(x =>
				       x.Attribute("Name").Value == conversionKeyword ||
				       (x.Attribute("ConversionKeyword") != null && x.Attribute("ConversionKeyword").Value == conversionKeyword));
			foreach (var participant in expirements
				.Select(expirement => expirement.Element("Participants"))
				.Select(participants => participants.Elements("Participant").Single(x => x.Attribute("Id").Value == userId))
				.Where(participant => participant.Attribute("HasConverted") == null))
			{
				participant.Add(new XAttribute("HasConverted", true));
				participant.Add(new XAttribute("DateConverted", utcNow));
			}
			Save(xml);
		}

		#endregion

		protected virtual XDocument Load()
		{
			return File.Exists(_pathToXmlStorage)
			       	? XDocument.Load(_pathToXmlStorage)
			       	: new XDocument(new XElement("Expirements"));
		}

		protected virtual void Save(XDocument xml)
		{
			xml.Save(_pathToXmlStorage);
		}
	}
}