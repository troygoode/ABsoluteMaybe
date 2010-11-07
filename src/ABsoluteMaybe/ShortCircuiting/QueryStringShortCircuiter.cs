using System.Linq;
using System.Web;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public class QueryStringShortCircuiter : IShortCircuiter
	{
		protected virtual HttpContextBase Context
		{
			get { return new HttpContextWrapper(HttpContext.Current); }
		}

		#region IShortCircuiter Members

		public ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId)
		{
			return Context.Request.QueryString.AllKeys.Contains(experimentSummary.Name)
			       	? new ShortCircuitResult(true, Context.Request.QueryString[experimentSummary.Name])
			       	: ShortCircuitResult.False;
		}

		#endregion
	}
}