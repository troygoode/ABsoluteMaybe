using System.Collections.Generic;

namespace ABsoluteMaybe
{
	public interface IABsoluteMaybe
	{
		T Test<T>(string experimentName, string conversionKeyword, IEnumerable<T> options);
		void Convert(string conversionKeyword);
	}
}