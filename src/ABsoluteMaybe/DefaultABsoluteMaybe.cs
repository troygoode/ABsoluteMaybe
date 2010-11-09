using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;
using ABsoluteMaybe.ShortCircuiting;
using ABsoluteMaybe.UserFiltering;

namespace ABsoluteMaybe
{
	public class DefaultABsoluteMaybe : IABsoluteMaybe
	{
		private readonly IExperimentCommands _experimentCommands;
		private readonly IOptionChooser _optionChooser;
		private readonly IOptionSerializer _optionSerializer;
		private readonly IEnumerable<IShortCircuiter> _shortCircuiters;
		private readonly IEnumerable<IUserFilter> _userFilters;
		private readonly IUserIdentification _userIdentification;

		public DefaultABsoluteMaybe(IExperimentCommands experimentCommands,
		                            IOptionChooser optionChooser,
		                            IOptionSerializer optionSerializer,
		                            IEnumerable<IShortCircuiter> shortCircuiters,
		                            IEnumerable<IUserFilter> userFilters,
		                            IUserIdentification userIdentification
			)
		{
			_experimentCommands = experimentCommands;
			_optionChooser = optionChooser;
			_optionSerializer = optionSerializer;
			_shortCircuiters = shortCircuiters;
			_userFilters = userFilters;
			_userIdentification = userIdentification;
		}

		#region IABsoluteMaybe Members

		public T Test<T>(string experimentName,
		                 string conversionKeyword,
		                 IEnumerable<T> options)
		{
			var userId = _userIdentification.Identity;
			if (_userFilters.Any(filter => filter.FilterOut(userId)))
				return options.First();

			var optionsAsStrings = options.Select(_optionSerializer.Serialize).ToArray();
			var experiment = _experimentCommands.GetOrCreateExperiment(experimentName, conversionKeyword, optionsAsStrings);

			var shortCircuit = _shortCircuiters
				.AsQueryable()
				.Select(sc => sc.ShortCircuit(experiment, userId))
				.FirstOrDefault(scr => scr.ShouldShortCircuitRequest);
			if (shortCircuit != null && optionsAsStrings.Contains(shortCircuit.ShortCircuitTo))
				return options.SingleOrDefault(option => _optionSerializer.Serialize(option) == shortCircuit.ShortCircuitTo);

			var participationRecord = _experimentCommands.GetOrCreateParticipationRecord(experimentName,
			                                                                               () =>
			                                                                               _optionChooser.Choose(optionsAsStrings),
			                                                                               userId);

			return options.Single(option => _optionSerializer.Serialize(option) == participationRecord.AssignedOption);
		}

		public void Convert(string conversionKeyword)
		{
			var userId = _userIdentification.Identity;
			if (_userFilters.Any(filter => filter.FilterOut(userId)))
				return;

			_experimentCommands.Convert(conversionKeyword, userId);
		}

		#endregion
	}
}