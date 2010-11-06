using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;

namespace ABsoluteMaybe
{
	public class DefaultABsoluteMaybe : IABsoluteMaybe
	{
		private readonly IExpirementRepository _expirementRepository;
		private readonly IOptionSerializer _optionSerializer;
		private readonly IUserIdentification _userIdentification;

		public DefaultABsoluteMaybe(IExpirementRepository expirementRepository,
		                            IOptionSerializer optionSerializer,
		                            IUserIdentification userIdentification
			)
		{
			_expirementRepository = expirementRepository;
			_optionSerializer = optionSerializer;
			_userIdentification = userIdentification;
		}

		#region IABsoluteMaybe Members

		public T Test<T>(string expirementName, IEnumerable<T> options)
		{
			var optionsAsStrings = options.Select(_optionSerializer.Serialize).ToArray();
			var exp = _expirementRepository.GetOrCreateExpirement(expirementName, optionsAsStrings);

			var userId = _userIdentification.Identity;
			var participationRecord = _expirementRepository.GetOrCreateParticipationRecord(exp, userId);

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