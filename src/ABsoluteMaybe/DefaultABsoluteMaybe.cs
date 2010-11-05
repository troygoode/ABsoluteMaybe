using System;
using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public class DefaultABsoluteMaybe : IABsoluteMaybe
	{
		private readonly ITestsRepository _testsRepository;
		private readonly IUserIdentificationStrategy _userIdentificationStrategy;

		public DefaultABsoluteMaybe(IUserIdentificationStrategy userIdentificationStrategy, ITestsRepository testsRepository)
		{
			_userIdentificationStrategy = userIdentificationStrategy;
			_testsRepository = testsRepository;
		}

		#region IABsoluteMaybe Members

		public T Test<T>(string testName, IEnumerable<T> alternatives)
		{
			var userId = _userIdentificationStrategy.Identity;
			throw new NotImplementedException();
		}

		public void Convert(string testName)
		{
			var participationRecord = _testsRepository.GetOrCreateParticipationRecord(testName);
			participationRecord.HasConverted = true;
		}

		#endregion
	}
}