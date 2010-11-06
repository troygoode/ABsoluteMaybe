using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;

namespace ABsoluteMaybe
{
	public class DefaultABsoluteMaybe : IABsoluteMaybe
	{
		private readonly IExpirementRepository _expirementRepository;
		private readonly IOptionChooser _optionChooser;
		private readonly IOptionSerializer _optionSerializer;
		private readonly IUserIdentification _userIdentification;

		public DefaultABsoluteMaybe(IExpirementRepository expirementRepository,
		                            IOptionChooser optionChooser,
		                            IOptionSerializer optionSerializer,
		                            IUserIdentification userIdentification
			)
		{
			_expirementRepository = expirementRepository;
			_optionChooser = optionChooser;
			_optionSerializer = optionSerializer;
			_userIdentification = userIdentification;
		}

		#region IABsoluteMaybe Members

		public T Test<T>(string expirementName,
		                 IEnumerable<T> options)
		{
			_expirementRepository.CreateExpirement(expirementName);

			var userId = _userIdentification.Identity;
			var optionsAsStrings = options.Select(_optionSerializer.Serialize).ToArray();
			var participationRecord = _expirementRepository.GetOrCreateParticipationRecord(expirementName, () => _optionChooser.Choose(optionsAsStrings), userId);

			return options.Single(option => _optionSerializer.Serialize(option) == participationRecord.AssignedOption);
		}

		public void Convert(string expirementName)
		{
			var userId = _userIdentification.Identity;
			_expirementRepository.Convert(expirementName, userId);
		}

		#endregion
	}
}