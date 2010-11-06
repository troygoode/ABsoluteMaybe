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
		private readonly IExpirementRepository _expirementRepository;
		private readonly IOptionChooser _optionChooser;
		private readonly IOptionSerializer _optionSerializer;
		private readonly IEnumerable<IUserFilter> _userFilters;
		private readonly IUserIdentification _userIdentification;

		public DefaultABsoluteMaybe(IExpirementRepository expirementRepository,
		                            IOptionChooser optionChooser,
		                            IOptionSerializer optionSerializer,
		                            IUserIdentification userIdentification,
		                            IEnumerable<IUserFilter> userFilters
			)
		{
			_expirementRepository = expirementRepository;
			_optionChooser = optionChooser;
			_optionSerializer = optionSerializer;
			_userIdentification = userIdentification;
			_userFilters = userFilters;
		}

		#region IABsoluteMaybe Members

		public T Test<T>(string expirementName,
		                 string conversionKeyword,
		                 IEnumerable<T> options)
		{
			var userId = _userIdentification.Identity;
			if (_userFilters.Any(filter => filter.FilterOut(userId)))
				return options.First();

			_expirementRepository.CreateExpirement(expirementName, conversionKeyword);

			var optionsAsStrings = options.Select(_optionSerializer.Serialize).ToArray();
			var participationRecord = _expirementRepository.GetOrCreateParticipationRecord(expirementName,
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

			_expirementRepository.Convert(conversionKeyword, userId);
		}

		#endregion
	}
}