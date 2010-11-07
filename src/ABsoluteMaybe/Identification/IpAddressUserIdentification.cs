using System.Web;

namespace ABsoluteMaybe.Identification
{
	public class IpAddressUserIdentification : IUserIdentification
	{
		protected virtual HttpContextBase Context
		{
			get { return new HttpContextWrapper(HttpContext.Current); }
		}

		#region IUserIdentification Members

		public string Identity
		{
			get { return Context.Request.UserHostAddress; }
		}

		#endregion
	}
}