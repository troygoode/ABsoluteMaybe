using System.Web;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard
{
	public static class ABsoluteMaybeConfiguration
	{
		static ABsoluteMaybeConfiguration()
		{
			var pathToXmlFile = HttpContext.Current.Server.MapPath("~/ABsoluteMaybe.xml");
			ABsoluteMaybe.ABsoluteMaybeFactory = new ABsoluteMaybeFactoryBuilder(pathToXmlFile)
				.Build();
		}
	}
}