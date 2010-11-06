using System;
using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public static class ABsoluteMaybe
	{
		public static Func<IABsoluteMaybe> ABsoluteMaybeFactory = () => null;

		public static T Test<T>(string experimentName, IEnumerable<T> options)
		{
			return Test(experimentName, experimentName, options);
		}

		public static T Test<T>(string experimentName, string conversionKeyword, IEnumerable<T> options)
		{
			var ab = ABsoluteMaybeFactory();
			return ab.Test(experimentName, conversionKeyword, options);
		}

		public static bool Test(string experimentName)
		{
			return Test(experimentName, experimentName);
		}

		public static bool Test(string experimentName, string conversionKeyword)
		{
			return Test(experimentName, conversionKeyword, new[] { false, true });
		}

		public static void Convert(string conversionKeyword)
		{
			var ab = ABsoluteMaybeFactory();
			ab.Convert(conversionKeyword);
		}
	}
}