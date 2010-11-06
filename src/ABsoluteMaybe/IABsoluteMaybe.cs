using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public interface IABsoluteMaybe
	{
		T Test<T>(string expirementName, IEnumerable<T> options);
		void Convert(string expirementName);
	}
}