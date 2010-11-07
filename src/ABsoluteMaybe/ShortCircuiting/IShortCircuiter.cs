using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public interface IShortCircuiter
	{
		ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId);
	}
}