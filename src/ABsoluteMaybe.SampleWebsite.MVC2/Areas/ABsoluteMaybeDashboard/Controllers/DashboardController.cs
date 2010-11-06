using System.Web.Mvc;
using ABsoluteMaybe.Persistence;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Controllers
{
	public class DashboardController : Controller
	{
		private readonly IExperimentRepository _experimentRepository;

		public DashboardController()
		{
			
		}

		public DashboardController(IExperimentRepository experimentRepository)
		{
			_experimentRepository = experimentRepository;
		}

		// GET: /ABsoluteMaybeDashboard/Dashboard/
		public ViewResult Index()
		{
			return View();
		}
	}
}