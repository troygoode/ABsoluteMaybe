namespace ABsoluteMaybe.Domain
{
	public class ExperimentSummary
	{
		public ExperimentSummary(string name, string finalOption, bool isEnded)
		{
			Name = name;
			FinalOption = finalOption;
			IsEnded = isEnded;
		}

		public string Name { get; private set; }
		public string FinalOption { get; private set; }
		public bool IsEnded { get; private set; }
	}
}