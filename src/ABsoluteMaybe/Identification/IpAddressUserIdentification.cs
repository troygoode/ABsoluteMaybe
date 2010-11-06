using System.Web;

namespace ABsoluteMaybe.Identification
{
	public class IpAddressUserIdentification : IUserIdentification
	{
		#region IUserIdentification Members

		public string Identity
		{
			get { return HttpContext.Current.Request.UserHostAddress; }
		}

		#endregion
	}
}