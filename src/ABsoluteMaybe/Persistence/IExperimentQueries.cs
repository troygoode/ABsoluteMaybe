using System.Linq;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.Persistence
{
	public interface IExperimentQueries
	{
		IQueryable<Experiment> FindAllExperiments();
	}
}