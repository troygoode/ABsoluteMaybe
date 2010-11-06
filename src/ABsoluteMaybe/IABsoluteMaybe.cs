using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public interface IABsoluteMaybe
	{
		T Test<T>(string expirementName, string conversionKeyword, IEnumerable<T> options);
		void Convert(string conversionKeyword);
	}
}