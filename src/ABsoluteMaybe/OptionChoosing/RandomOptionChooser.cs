using System;
using System.Collections.Generic;
using System.Linq;

namespace ABsoluteMaybe.OptionChoosing
{
	public class RandomOptionChooser : IOptionChooser
	{
		private static readonly Random RandomNumberGenerator = new Random();

		#region IOptionChooser Members

		public string Choose(IEnumerable<string> options)
		{
			var index = RandomNumberGenerator.Next(0, options.Count());
			return options.ElementAt(index);
		}

		#endregion
	}
}