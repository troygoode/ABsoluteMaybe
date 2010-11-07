using System.Linq;
using System.Web.Mvc;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models;
using ABsoluteMaybe.Statistics;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Controllers
{
	//[Authorize(Roles = "ABsoluteMaybe Test Administrator")]
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

		public ViewResult Index()
		{
			var experiments = _experimentRepository
				.FindAllExperiments()
				.Select(exp => new DashboardIndexViewModel.ExperimentViewModel
								{
									Name = exp.Name,
									Results = new ABingoStyleFormatter(new ABsoluteMaybeStatistics(exp)).ToString(),
									Started = exp.DateStarted,
									Ended = exp.DateEnded,
									IsEnded = exp.DateEnded != null,
									TotalParticipants = exp.Options.Sum(o => o.Participants),
									TotalConversions = exp.Options.Sum(o => o.Conversions),
									Options = exp.Options.Select(o =>
										new DashboardIndexViewModel.ExperimentViewModel.OptionViewModel
											{
												Name = o.Name,
												IsAlwaysUseOption = o.Name == exp.AlwaysUseOption,
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

		[HttpPost, ValidateAntiForgeryToken]
		public RedirectToRouteResult EndExperiment(string experiment, string option)
		{
			_experimentRepository.EndExperiment(experiment, option);
			TempData["Flash"] = "Experiment marked as ended.  All users will now see the chosen option.";
			return RedirectToAction("Index");
		}
	}
}