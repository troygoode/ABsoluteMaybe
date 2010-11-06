using System.Linq;
using System.Web.Mvc;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models;
using ABsoluteMaybe.Statistics;

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
			var experiments = _experimentRepository
				.FindAllExperiments()
				.Select(exp => new ExperimentViewModel
				               	{
				               		Experiment = exp,
									Results = new ABingoStyleFormatter(new ABsoluteMaybeStatistics(exp)).ToString()
				               	});
			return View(experiments);
		}
	}
}