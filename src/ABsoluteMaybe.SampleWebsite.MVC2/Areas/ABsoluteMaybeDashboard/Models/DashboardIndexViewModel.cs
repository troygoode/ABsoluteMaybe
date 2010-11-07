﻿using System.Collections.Generic;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Areas.ABsoluteMaybeDashboard.Models
{
	public class DashboardIndexViewModel
	{
		public IEnumerable<ExperimentViewModel> Experiments { get; set; }

		#region Nested type: ExperimentViewModel

		public class ExperimentViewModel
		{
			public string Name { get; set; }
			public string Results { get; set; }
			public int TotalParticipants { get; set; }
			public int TotalConversions { get; set; }
			public IEnumerable<OptionViewModel> Options { get; set; }

			#region Nested type: OptionViewModel

			public class OptionViewModel
			{
				public string Name { get; set; }
				public int Participants { get; set; }
				public int Conversions { get; set; }
				public double? ConversionRate { get; set; }
			}

			#endregion
		}

		#endregion
	}
}