using System.Linq;
using System.Web;
using ABsoluteMaybe.Domain;

namespace ABsoluteMaybe.ShortCircuiting
{
	public class CookieShortCircuiter : IShortCircuiter
	{
		protected virtual HttpContextBase Context
		{
			get { return new HttpContextWrapper(HttpContext.Current); }
		}

		#region IShortCircuiter Members

		public ShortCircuitResult ShortCircuit(ExperimentSummary experimentSummary, string userId)
		{
			var cookie = Context.Request.Cookies["ABsoluteMaybe-ShortCircuit"];
			return cookie != null && cookie.Values.AllKeys.Contains(experimentSummary.Name)
					? new ShortCircuitResult(true, cookie.Values[experimentSummary.Name])
					: ShortCircuitResult.False;
		}

		#endregion
	}
}