using System;
using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public static class ABsoluteMaybe
	{
		public static Func<IABsoluteMaybe> ABsoluteMaybeFactory = () => new DefaultABsoluteMaybe(null);

		public static T Test<T>(string testName, IEnumerable<T> alternatives)
		{
			return ABsoluteMaybeFactory().Test(testName, alternatives);
		}

		public static bool Test(string testName)
		{
			return Test(testName, new[] {false, true});
		}

		public static void Convert(string testName)
		{
			ABsoluteMaybeFactory().Convert(testName);
		}
	}
}