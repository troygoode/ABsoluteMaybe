using System;
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

		public void CreateExpirement(string expirementName)
		{
			var xml = Load();

			if (!xml.Root.Elements("Expirement").Any(x => x.Attribute("Name").Value == expirementName))
				xml.Root.Add(new XElement("Expirement",
				                          new XAttribute("Name", expirementName),
				                          new XAttribute("Started", UtcNow)
				             	));

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
				       		ExpirementName = expirementName,
				       		UserIdentifier = existingRecord.Attribute("Id").Value,
				       		AssignedOption = existingRecord.Value,
				       		HasConverted = false
				       	};

			var assignedOption = chooseAnOptionForUser();
			expirement.Element("Participants").Add(new XElement("Participant",
			                                                    new XAttribute("Id", userId),
			                                                    new XCData(assignedOption)));

			Save(xml);
			return new ParticipationRecord
			       	{
			       		ExpirementName = expirementName,
			       		UserIdentifier = userId,
			       		AssignedOption = assignedOption,
			       		HasConverted = false
			       	};
		}

		public void Convert(string expirementName,
		                    string userId)
		{
			var xml = Load();

			var expirement = xml.Root.Elements("Expirement").Single(x => x.Attribute("Name").Value == expirementName);
			var participants = expirement.Element("Participants");
			var participant = participants.Elements("Participant").Single(x => x.Attribute("Id").Value == userId);

			if (participant.Attribute("HasConverted") != null)
				return;

			participant.Add(new XAttribute("HasConverted", true));
			participant.Add(new XAttribute("DateConverted", UtcNow));

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