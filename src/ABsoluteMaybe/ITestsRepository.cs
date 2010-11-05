namespace ABsoluteMaybe
{
	public interface ITestsRepository
	{
		ParticipationRecord GetOrCreateParticipationRecord(string testName);
	}
}