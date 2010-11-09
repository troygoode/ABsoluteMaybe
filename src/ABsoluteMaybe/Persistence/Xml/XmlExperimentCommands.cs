using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence.Xml
{
	public class XmlExperimentCommands : IExperimentCommands
	{
		private readonly string _pathToXmlStorage;

		public XmlExperimentCommands(string pathToXmlStorage)
		{
			_pathToXmlStorage = pathToXmlStorage;
		}

		protected virtual DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}

		#region IExperimentRepository Members

		public ExperimentSummary GetOrCreateExperiment(string experimentName, IEnumerable<string> options)
		{
			return GetOrCreateExperiment(experimentName, experimentName, options);
		}

		public ExperimentSummary GetOrCreateExperiment(string experimentName, string conversionKeyword,
		                                               IEnumerable<string> options)
		{
			var xml = Load();

			var existingExperiment = xml.Root.Elements("Experiment")
				.SingleOrDefault(x => x.Attribute("Name").Value == experimentName);
			if (existingExperiment != null)
				return new ExperimentSummary(
					existingExperiment.Attribute("Name").Value,
					existingExperiment.Attribute("AlwaysUseOption") == null
						? null
						: existingExperiment.Attribute("AlwaysUseOption").Value,
					existingExperiment.Attribute("Ended") != null
					);

			var exp = new XElement("Experiment",
			                       new XAttribute("Name", experimentName),
			                       new XAttribute("Started", UtcNow),
			                       new XElement("PossibleOptionValues",
			                                    options.Select(o => new XElement("Option", new XCData(o)))
			                       	)
				);
			if (experimentName != conversionKeyword)
				exp.Add(new XAttribute("ConversionKeyword", conversionKeyword));
			xml.Root.Add(exp);

			Save(xml);

			return new ExperimentSummary(experimentName, null, false);
		}

		public ParticipationRecord GetOrCreateParticipationRecord(string experimentName,
		                                                          Func<string> chooseAnOptionForUser,
		                                                          string userId)
		{
			var xml = Load();

			var experiment = xml.Root.Elements("Experiment").Single(x => x.Attribute("Name").Value == experimentName);
			if (experiment.Element("Participants") == null)
				experiment.Add(new XElement("Participants"));

			var participants = experiment.Element("Participants");
			var existingRecord = participants.Elements("Participant").SingleOrDefault(x => x.Attribute("Id").Value == userId);
			if (existingRecord != null)
				return new ParticipationRecord(
					existingRecord.Attribute("Id").Value,
					existingRecord.Value,
					existingRecord.Attribute("HasConverted") == null
						? false
						: bool.Parse(existingRecord.Attribute("HasConverted").Value),
					existingRecord.Attribute("DateConverted") == null
						? (DateTime?) null
						: DateTime.Parse(existingRecord.Attribute("DateConverted").Value)
					);

			var assignedOption = chooseAnOptionForUser();
			experiment.Element("Participants").Add(new XElement("Participant",
			                                                    new XAttribute("Id", userId),
			                                                    new XCData(assignedOption)));

			Save(xml);
			return new ParticipationRecord(userId, assignedOption, false, null);
		}

		public void Convert(string conversionKeyword,
		                    string userId)
		{
			var xml = Load();

			var utcNow = UtcNow;
			var experiments = xml.Root.Elements("Experiment")
				.Where(x =>
				       x.Attribute("Name").Value == conversionKeyword ||
				       (x.Attribute("ConversionKeyword") != null && x.Attribute("ConversionKeyword").Value == conversionKeyword));
			foreach (var participant in experiments
				.Select(experiment => experiment.Element("Participants"))
				.Select(participants => participants.Elements("Participant").Single(x => x.Attribute("Id").Value == userId))
				.Where(participant => participant.Attribute("HasConverted") == null))
			{
				participant.Add(new XAttribute("HasConverted", true));
				participant.Add(new XAttribute("DateConverted", utcNow));
			}
			Save(xml);
		}

		public void EndExperiment(string experimentName, string alwaysUseOption)
		{
			var xml = Load();

			var experiment = xml.Root.Elements("Experiment").Single(exp => exp.Attribute("Name").Value == experimentName);
			if (experiment.Attribute("Ended") != null)
				return;

			experiment.Add(new XAttribute("Ended", UtcNow));
			experiment.Add(new XAttribute("AlwaysUseOption", alwaysUseOption));
			Save(xml);
		}

		#endregion

		protected virtual XDocument Load()
		{
			return File.Exists(_pathToXmlStorage)
			       	? XDocument.Load(_pathToXmlStorage)
			       	: new XDocument(new XElement("Experiments"));
		}

		protected virtual void Save(XDocument xml)
		{
			xml.Save(_pathToXmlStorage);
		}
	}
}