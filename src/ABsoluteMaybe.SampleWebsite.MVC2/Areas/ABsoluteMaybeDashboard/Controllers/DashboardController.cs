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
				.Select(exp => new DashboardIndexViewModel.ExperimentViewModel
								{
									Name = exp.Name,
									Results = new ABingoStyleFormatter(new ABsoluteMaybeStatistics(exp)).ToString(),
									TotalParticipants = exp.Options.Sum(o => o.Participants),
									TotalConversions = exp.Options.Sum(o => o.Conversions),
									Options = exp.Options.Select(o =>
										new DashboardIndexViewModel.ExperimentViewModel.OptionViewModel
											{
												Name = o.Name,
												Participants = o.Participants,
												Conversions = o.Conversions,
												ConversionRate = o.Participants > 0
													? (double)o.Conversions / o.Participants
													: (double?)null
											}
									)
								});
			return View(new DashboardIndexViewModel
			            	{
			            		Experiments = experiments
			            	});
		}
	}
}