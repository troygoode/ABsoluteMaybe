using System;
using System.Xml.Linq;
using ABsoluteMaybe.Persistence.Xml;

namespace ABsoluteMaybe.Tests.Persistence
{
	public class XmlCommandsStub : XmlExperimentCommands
	{
		public XmlCommandsStub()
			: base(String.Empty)
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
			return String.IsNullOrWhiteSpace(SavedXml)
			       	? new XDocument(new XElement("Experiments"))
			       	: XDocument.Parse(SavedXml);
		}

		protected override void Save(XDocument xml)
		{
			SavedXml = xml.ToString();
		}
	}
}