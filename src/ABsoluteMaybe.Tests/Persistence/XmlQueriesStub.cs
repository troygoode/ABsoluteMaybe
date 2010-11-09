using System;
using System.Xml.Linq;
using ABsoluteMaybe.Persistence.Xml;

namespace ABsoluteMaybe.Tests.Persistence
{
	public class XmlQueriesStub : XmlExperimentQueries
	{
		private string _xml;

		public XmlQueriesStub(string xml)
			: base(String.Empty)
		{
			Reset(xml);
		}

		public void Reset(string xml)
		{
			_xml = xml;
		}

		protected override XDocument Load()
		{
			return XDocument.Parse(_xml);
		}
	}
}