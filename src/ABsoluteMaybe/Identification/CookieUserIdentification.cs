using System;
using System.Web;
using System.Web.Security;

namespace ABsoluteMaybe.Identification
{
	public class CookieUserIdentification : IUserIdentification
	{
		protected virtual HttpContextBase Context
		{
			get { return new HttpContextWrapper(HttpContext.Current); }
		}

		protected virtual DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}

		#region IUserIdentification Members

		public string Identity
		{
			get { return ReadFromCookie(); }
		}

		#endregion

		private string ReadFromCookie()
		{
			var cookie = Context.Request.Cookies["ABsoluteMaybe"];
			if (cookie == null)
				return GenerateIdAndSaveToCookie();

			var id = cookie.Value;
			return string.IsNullOrWhiteSpace(id)
			       	? GenerateIdAndSaveToCookie()
			       	: id;
		}

		private string GenerateIdAndSaveToCookie()
		{
			var id = GenerateUniqueId();
			var cookie = new HttpCookie("ABsoluteMaybe", id);

			//first remove, then add, in case we've already added this cookie as part of a previous save during this page load.
			Context.Response.Cookies.Remove("ABsoluteMaybe");
			Context.Response.Cookies.Add(cookie);

			//fix up the incoming cookie so that it will load correctly if we need it again during this page load.
			Context.Request.Cookies.Remove("ABsoluteMaybe");
			Context.Request.Cookies.Add(cookie);

			return id;
		}

		private string GenerateUniqueId()
		{
			var ip = Context.Request.UserHostAddress;
			var now = UtcNow.Ticks.ToString();
			return Hash(ip + now);
		}

		protected virtual string Hash(string text)
		{
			const string hashAlgo = "MD5";
			return FormsAuthentication.HashPasswordForStoringInConfigFile(text, hashAlgo);
		}
	}
}