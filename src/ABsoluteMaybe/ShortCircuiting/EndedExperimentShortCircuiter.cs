using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public class EndedExperimentShortCircuiter : IShortCircuiter
	{
		#region IShortCircuiter Members

		public ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId)
		{
			return experimentSummary.IsEnded
				? new ShortCircuitResult(true, experimentSummary.FinalOption)
				: new ShortCircuitResult(false, null);
		}

		#endregion
	}
}