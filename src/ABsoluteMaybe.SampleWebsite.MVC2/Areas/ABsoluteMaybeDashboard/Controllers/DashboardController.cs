using System.Linq;
using System.Web.Mvc;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Persistence.Xml;
using ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models;
using ABsoluteMaybe.Statistics;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Controllers
{
	//[Authorize(Roles = "ABsoluteMaybe Test Administrator")]
	public class DashboardController : Controller
	{
		private readonly IExperimentCommands _experimentCommands;
		private readonly IExperimentQueries _experimentQueries;

		public DashboardController()
		{
			var storagePath = System.Web.HttpContext.Current.Server.MapPath(ABsoluteMaybeConfiguration.StoragePath);
			_experimentCommands = new XmlExperimentCommands(storagePath);
			_experimentQueries = new XmlExperimentQueries(storagePath);
		}

		public DashboardController(IExperimentCommands experimentCommands,
		                           IExperimentQueries experimentQueries)
		{
			_experimentCommands = experimentCommands;
			_experimentQueries = experimentQueries;
		}

		public ViewResult Index()
		{
			var experiments = _experimentQueries
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
			_experimentCommands.EndExperiment(experiment, option);
			TempData["Flash"] = "Experiment marked as ended.  All users will now see the chosen option.";
			return RedirectToAction("Index");
		}
	}
}