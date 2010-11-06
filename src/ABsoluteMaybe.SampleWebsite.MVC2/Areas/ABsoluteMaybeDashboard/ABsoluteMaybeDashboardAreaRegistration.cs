using System.Web.Mvc;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard
{
	public class ABsoluteMaybeDashboardAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "ABsoluteMaybeDashboard";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"ABsoluteMaybeDashboard_default",
				"ABsoluteMaybeDashboard/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
