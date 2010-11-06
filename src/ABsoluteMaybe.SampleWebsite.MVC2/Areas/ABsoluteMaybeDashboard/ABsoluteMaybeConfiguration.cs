using System.Web;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard
{
	public static class ABsoluteMaybeConfiguration
	{
		public static string StoragePath = @"~\App_Data\ABsoluteMaybe.xml";

		public static void Configure()
		{
			var pathToXmlFile = HttpContext.Current.Server.MapPath(StoragePath);
			ABsoluteMaybe.ABsoluteMaybeFactory = new ABsoluteMaybeFactoryBuilder(pathToXmlFile)
				.Build();
		}
	}
}