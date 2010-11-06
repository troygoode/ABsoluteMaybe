using System;
using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public static class ABsoluteMaybe
	{
		public static Func<IABsoluteMaybe> ABsoluteMaybeFactory = () => null;

		public static T Test<T>(string expirementName, IEnumerable<T> options)
		{
			var ab = ABsoluteMaybeFactory();
			return ab.Test(expirementName, options);
		}

		public static bool Test(string expirementName)
		{
			return Test(expirementName, new[] {false, true});
		}

		public static void Convert(string expirementName)
		{
			var ab = ABsoluteMaybeFactory();
			ab.Convert(expirementName);
		}
	}
}