using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public interface IABsoluteMaybe
	{
		T Test<T>(string testName, IEnumerable<T> alternatives);
		void Convert(string testName);
	}
}