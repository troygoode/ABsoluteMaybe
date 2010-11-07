using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ABsoluteMaybe.UserFiltering
{
	public class SpiderFilter : IUserFilter
	{
		private readonly IEnumerable<string> _bots;

		public SpiderFilter() : this(new[]
		                             	{
		                             		"Googlebot",
		                             		"Slurp",
		                             		"msnbot",
		                             		"nagios",
		                             		"Baiduspider",
		                             		"Sogou",
		                             		"SiteUptime.com",
		                             		"Python",
		                             		"DotBot",
		                             		"Feedfetcher",
		                             		"Jeeves",
		                             	})
		{
		}

		public SpiderFilter(IEnumerable<string> bots)
		{
			_bots = bots;
		}

		protected virtual HttpContextBase Context
		{
			get { return new HttpContextWrapper(HttpContext.Current); }
		}

		#region IUserFilter Members

		public bool FilterOut(string userId)
		{
			if (string.IsNullOrWhiteSpace(Context.Request.UserAgent))
				return true;

			var userAgent = HttpContext.Current.Request.UserAgent;
			return userAgent == null || _bots.Any(userAgent.Contains);
		}

		#endregion
	}
}