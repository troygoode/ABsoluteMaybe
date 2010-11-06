using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;
using ABsoluteMaybe.UserFiltering;

namespace ABsoluteMaybe
{
	public class DefaultABsoluteMaybe : IABsoluteMaybe
	{
		private readonly IExperimentRepository _experimentRepository;
		private readonly IOptionChooser _optionChooser;
		private readonly IOptionSerializer _optionSerializer;
		private readonly IEnumerable<IUserFilter> _userFilters;
		private readonly IUserIdentification _userIdentification;

		public DefaultABsoluteMaybe(IExperimentRepository experimentRepository,
		                            IOptionChooser optionChooser,
		                            IOptionSerializer optionSerializer,
		                            IUserIdentification userIdentification,
		                            IEnumerable<IUserFilter> userFilters
			)
		{
			_experimentRepository = experimentRepository;
			_optionChooser = optionChooser;
			_optionSerializer = optionSerializer;
			_userIdentification = userIdentification;
			_userFilters = userFilters;
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
			_experimentRepository.CreateExperiment(experimentName, conversionKeyword, optionsAsStrings);

			var participationRecord = _experimentRepository.GetOrCreateParticipationRecord(experimentName,
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

			_experimentRepository.Convert(conversionKeyword, userId);
		}

		#endregion
	}
}