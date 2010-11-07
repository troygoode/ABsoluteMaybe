using System;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public class CookieShortCircuiter : IShortCircuiter
	{
		#region IShortCircuiter Members

		public ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}