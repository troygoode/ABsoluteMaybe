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
			//1 - Record test if it doesn't exist yet.
			//2 - Create participation record for user if he doesn't have one yet.
			//3 - return option assigned to user

			//var userId = _userIdentificationStrategy.Identity;
			throw new NotImplementedException();
		}

		public void Convert(string testName)
		{
			var userId = _userIdentificationStrategy.Identity;
			_testsRepository.Convert(testName, userId);
		}

		#endregion
	}
}