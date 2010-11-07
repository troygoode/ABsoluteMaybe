using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public class EndedExperimentShortCircuiter : IShortCircuiter
	{
		#region IShortCircuiter Members

		public ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId)
		{
			return experimentSummary.HasEnded
				? new ShortCircuitResult(true, experimentSummary.AlwaysUseOption)
				: new ShortCircuitResult(false, null);
		}

		#endregion
	}
}