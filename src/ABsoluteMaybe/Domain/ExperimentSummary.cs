namespace ABsoluteMaybe.Domain
{
	public class ExperimentSummary
	{
		public ExperimentSummary(string name, string alwaysUseOption, bool hasEnded)
		{
			Name = name;
			AlwaysUseOption = alwaysUseOption;
			HasEnded = hasEnded;
		}

		public string Name { get; private set; }
		public string AlwaysUseOption { get; private set; }
		public bool HasEnded { get; private set; }
	}
}