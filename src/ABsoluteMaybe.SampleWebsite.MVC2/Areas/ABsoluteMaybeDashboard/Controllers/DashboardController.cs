using System.Web.Mvc;
using ABsoluteMaybe.Persistence;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Controllers
{
	public class DashboardController : Controller
	{
		private readonly IExperimentRepository _experimentRepository;

		public DashboardController()
		{
			var storagePath = System.Web.HttpContext.Current.Server.MapPath(ABsoluteMaybeConfiguration.StoragePath);
			_experimentRepository = new XmlExperimentRepository(storagePath);
		}

		public DashboardController(IExperimentRepository experimentRepository)
		{
			_experimentRepository = experimentRepository;
		}

		// GET: /ABsoluteMaybeDashboard/Dashboard/
		public ViewResult Index()
		{
			var experiments = _experimentRepository.FindAllExperiments();
			return View(experiments);
		}
	}
}