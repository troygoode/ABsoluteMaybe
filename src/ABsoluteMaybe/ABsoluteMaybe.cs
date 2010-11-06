using System;
using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public static class ABsoluteMaybe
	{
		public static Func<IABsoluteMaybe> ABsoluteMaybeFactory = () => null;

		public static T Test<T>(string expirementName, IEnumerable<T> options)
		{
			return Test(expirementName, expirementName, options);
		}

		public static T Test<T>(string expirementName, string conversionKeyword, IEnumerable<T> options)
		{
			var ab = ABsoluteMaybeFactory();
			return ab.Test(expirementName, conversionKeyword, options);
		}

		public static bool Test(string expirementName)
		{
			return Test(expirementName, expirementName);
		}

		public static bool Test(string expirementName, string conversionKeyword)
		{
			return Test(expirementName, conversionKeyword, new[] {false, true});
		}

		public static void Convert(string conversionKeyword)
		{
			var ab = ABsoluteMaybeFactory();
			ab.Convert(conversionKeyword);
		}
	}
}