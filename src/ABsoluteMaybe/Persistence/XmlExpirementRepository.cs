using System;
using System.Collections.Generic;
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

		#region IExpirementRepository Members

		public Expirement GetOrCreateExpirement(string expirementName,
		                                        IEnumerable<string> options)
		{
			throw new NotImplementedException();
		}

		public ParticipationRecord GetOrCreateParticipationRecord(string expirementName,
		                                                          Func<string> chooseAnOptionForUser,
		                                                          string userId)
		{
			throw new NotImplementedException();
		}

		public void Convert(string expirementName,
		                    string userId)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}