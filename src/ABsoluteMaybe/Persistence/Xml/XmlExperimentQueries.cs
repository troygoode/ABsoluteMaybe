using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence.Xml
{
	public class XmlExperimentQueries : IExperimentQueries
	{
		private readonly string _pathToXmlStorage;

		public XmlExperimentQueries(string pathToXmlStorage)
		{
			_pathToXmlStorage = pathToXmlStorage;
		}

		protected virtual XDocument Load()
		{
			return File.Exists(_pathToXmlStorage)
			       	? XDocument.Load(_pathToXmlStorage)
			       	: new XDocument(new XElement("Experiments"));
		}

		#region IExperimentQueries Members

		public IQueryable<Experiment> FindAllExperiments()
		{
			var xml = Load();

			return xml.Root.Elements("Experiment")
				.Select(exp => new Experiment(
				               	exp.Attribute("Name").Value,
				               	exp.Attribute("ConversionKeyword") == null
				               		? exp.Attribute("Name").Value
				               		: exp.Attribute("ConversionKeyword").Value,
				               	exp.Attribute("AlwaysUseOption") == null
				               		? null
				               		: exp.Attribute("AlwaysUseOption").Value,
				               	DateTime.Parse(exp.Attribute("Started").Value),
				               	exp.Attribute("Ended") == null
				               		? (DateTime?)null
				               		: DateTime.Parse(exp.Attribute("Ended").Value),
				               	exp.Element("Participants") == null
				               		? Enumerable.Empty<ParticipationRecord>()
				               		: exp.Element("Participants")
				               		  	.Elements("Participant")
				               		  	.Select(p => new ParticipationRecord(
				               		  	             	p.Attribute("Id").Value,
				               		  	             	p.Value,
				               		  	             	p.Attribute("HasConverted") == null
				               		  	             		? false
				               		  	             		: bool.Parse(p.Attribute("HasConverted").Value),
				               		  	             	exp.Attribute("DateConverted") == null
				               		  	             		? (DateTime?)null
				               		  	             		: DateTime.Parse(
				               		  	             			p.Attribute("DateConverted").Value)
				               		  	             	)),
				               	exp.Element("PossibleOptionValues")
				               		.Elements("Option")
				               		.Select(pov => pov.Value)
				               	)).AsQueryable();
		}

		#endregion
	}
}