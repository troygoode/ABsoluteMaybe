using System.Collections.Generic;

namespace ABsoluteMaybe.OptionChoosing
{
	public interface IOptionChooser
	{
		string Choose(IEnumerable<string> options);
	}
}