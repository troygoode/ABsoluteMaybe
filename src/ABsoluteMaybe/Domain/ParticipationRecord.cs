namespace ABsoluteMaybe.Domain
{
	public class ParticipationRecord
	{
		public string UserIdentifier { get; set; }
		public string ExpirementName { get; set; }
		public string AssignedOption { get; set; }
		public bool HasConverted { get; set; }
	}
}